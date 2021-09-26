using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Web;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Google.Protobuf.WellKnownTypes;
using Eltin_Buchard_Keller_Algorithm;

namespace WildRiftWebAPI
{
    public interface IChampionService
    {
        ChampionDto GetByName(string name);
        PageResult<ChampionDto> GetAll(ChampionQuery query);
        void Delete(string name);
        void Create(CreateChampion dto);
        void Update(string name, UpdateChampion updateChampion);
        string GetProperty(string name, string property);
        string GetProperty(string name, string spellType, string property);
    }

    public class ChampionService : IChampionService
    {
        private readonly ChampionDbContext _dbContex;
        private readonly IMapper _mapper;
        private readonly ILogger<ChampionService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public ChampionService(ChampionDbContext dbContex, IMapper mapper, ILogger<ChampionService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContex = dbContex;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public ChampionDto GetByName(string name)
        {
            var championNames = _dbContex.Champions.Select(ch => ch.Name).ToList();
            List<string> championNamesTree = new();

            foreach (var element in championNames)
                championNamesTree.AddRange(element.Split(new char[] { ' ', '\'' }));

            championNamesTree.AddRange(championNames);

            BKTree tree = new(new(name));
            tree.AddMultiple(championNamesTree);
            string approximatedName = tree.FindBestNodeWithDistance(name);

            if (approximatedName is "")
                throw new NotFoundException("Champion not found");

            var champions = _dbContex.Champions.Include(r => r.ChampionSpells).Include(r => r.ChampionPassive).Where(r => r.Name.Contains(name) || r.Name.Contains(approximatedName));

            var champion = champions.FirstOrDefault(r => r.Name.Contains(name)) is not null ? champions.FirstOrDefault(r => r.Name.Contains(name)) : champions.FirstOrDefault(r => r.Name.Contains(approximatedName));

            champion.ChampionSpells = champion.ChampionSpells.OrderBy(ch => "QWER".IndexOf(ch.Id.Last())).ToList();
            var result = _mapper.Map<ChampionDto>(champion);
            return result;
        }

        public PageResult<ChampionDto> GetAll(ChampionQuery query)
        {
            var baseQuery = _dbContex.Champions
                .Include(r => r.ChampionSpells)
                .Include(r => r.ChampionPassive)
                .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) || r.Title.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Champion, object>>>
                {
                    { nameof(Champion.Name), r => r.Name },
                    { nameof(Champion.Title), r => r.Title },
                };

                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var champions = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            int totalItemsCount = baseQuery.Count();

            var championsDtos = _mapper.Map<List<ChampionDto>>(champions);

            foreach (var championDto in championsDtos)
                championDto.ChampionSpells = championDto.ChampionSpells.OrderBy(ch => "QWER".IndexOf(ch.Id.Last())).ToList();

            var result = new PageResult<ChampionDto>(championsDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public void Delete(string name)
        {
            _logger.LogWarning($"Restaurant with id: {name} Delete action invoked");

            var champion = _dbContex.Champions.FirstOrDefault(ch => ch.Name == name);

            if (champion is null)
                throw new NotFoundException("Champion not found");

            var championPassive = _dbContex.Champions_Passives.FirstOrDefault(ch => ch.Id.Contains(name));
            var championSpells = _dbContex.Champions_Spells.Where(ch => ch.Id.Contains(name));

            _dbContex.Champions.Remove(champion);
            _dbContex.Champions_Passives.Remove(championPassive);
            _dbContex.Champions_Spells.RemoveRange(championSpells);
            _dbContex.SaveChanges();
        }
        
        public void Create(CreateChampion createChampion)
        {
            var champion = _mapper.Map<Champion>(createChampion.CreateChampionDto);
            var championPassive = _mapper.Map<ChampionPassive>(createChampion.CreateChampionPassiveDto);
            var championSpells = _mapper.Map<List<ChampionSpell>>(createChampion.CreateChampionSpellDtos);

            _dbContex.Champions_Passives.Add(championPassive);
            _dbContex.SaveChanges();
            _dbContex.Champions_Spells.AddRange(championSpells);
            _dbContex.SaveChanges();
            _dbContex.Champions.Add(champion);
            _dbContex.SaveChanges();
        }
        
        public void Update(string name, UpdateChampion updateChampion)
        {
            var champion = _dbContex.Champions.FirstOrDefault(r => r.Name == name);
            var championPassive = _dbContex.Champions_Passives.FirstOrDefault(r => r.Id.Contains(name));
            var championSpells = _dbContex.Champions_Spells.Where(r => r.Id.Contains(name)).ToList();

            if (champion is null) 
                throw new NotFoundException("Champion not found");

            foreach (var property in updateChampion.UpdateChampionDto.GetType().GetProperties())
                if (property.GetValue(updateChampion.UpdateChampionDto) is not null)
                    typeof(Champion).GetProperty(property.Name).SetValue(champion, property.GetValue(updateChampion.UpdateChampionDto));

            foreach (var property in updateChampion.UpdateChampionPassiveDto.GetType().GetProperties())
                if (property.GetValue(updateChampion.UpdateChampionPassiveDto) is not null)
                    typeof(ChampionPassive).GetProperty(property.Name).SetValue(championPassive, property.GetValue(updateChampion.UpdateChampionPassiveDto));

            foreach (var spell in updateChampion.UpdateChampionSpellDtos)
                if (spell.Char is null) continue;
                else foreach (var property in spell.GetType().GetProperties())
                    if (property.GetValue(spell) is not null && property.Name != "Char")
                        typeof(ChampionSpell).GetProperty(property.Name).SetValue(championSpells.Where(s => s.Id.Last() == spell.Char).First(), property.GetValue(spell));

            _dbContex.SaveChanges();
        }

        public string GetProperty(string name, string property)
        {
            var champion = _dbContex.Champions.Include(r => r.ChampionSpells).Include(r => r.ChampionPassive).FirstOrDefault(r => r.Name == name);

            if (champion is null)
                throw new NotFoundException("Champion not found");

            Capitalize(ref property);
            var foundProperty = typeof(Champion).GetProperty(property);

            if (foundProperty is null)
                throw new NotFoundException("Property not found");

            var result = foundProperty.GetValue(champion).ToString();
            return result;
        }

        public string GetProperty(string name, string spellType, string property)
        {
            var champion = _dbContex.Champions.Include(r => r.ChampionSpells).Include(r => r.ChampionPassive).FirstOrDefault(r => r.Name == name);

            if (champion is null)
                throw new NotFoundException("Champion not found");

            Capitalize(ref property);
            Capitalize(ref spellType);

            if (spellType is "Passive")
            {
                var passiveProperty = typeof(ChampionPassive).GetProperty(property);
                if (passiveProperty is null)
                    throw new NotFoundException("Property not found");
                var passiveResult = passiveProperty.GetValue(champion.ChampionPassive).ToString();
                return passiveResult;
            }
            else
            {
                var spellProperty = typeof(ChampionSpell).GetProperty(property);

                if (spellProperty is null)
                    throw new NotFoundException("Property not found");

                var spellResult = spellProperty.GetValue(champion.ChampionSpells[spellType switch {
                    "E"  => 0,
                    "Q"  => 1,
                    "R"  => 2,
                    "W"  => 3,
                    _ => throw new NotFoundException("Spell type not found. It has to be \"Passive\" or \"Q\", \"W\", \"E\", \"R\""),
                    }]).ToString();
                return spellResult;
            }
        }

        private static void Capitalize(ref string input)
        {
            input = $"{input[0].ToString().ToUpper()}{input[1..].ToLower()}"; //substring to jest input[1..]
        }
    }
}