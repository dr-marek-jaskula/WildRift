using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        private readonly WildRiftDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ChampionService> _logger;

        public ChampionService(WildRiftDbContext dbContex, IMapper mapper, ILogger<ChampionService> logger)
        {
            _context = dbContex;
            _mapper = mapper;
            _logger = logger;
        }

        public ChampionDto GetByName(string name)
        {
            string approximatedName = Helpers.ApproximateName(name, _context.Champions);

            return ((Policy)PollyRegister.registry["CacheStrategy"]).Execute(context =>
            {
                if (approximatedName is "")
                    throw new NotFoundException("Champion not found");

                var champions = _context.Champions
                    .AsNoTracking()
                    .Include(r => r.ChampionSpells)
                    .Include(r => r.ChampionPassive).Where(r => r.Name.Contains(name) || r.Name.Contains(approximatedName));

                var champion = champions.FirstOrDefault(r => r.Name.Contains(name)) is not null ? champions.FirstOrDefault(r => r.Name.Contains(name)) : champions.FirstOrDefault(r => r.Name.Contains(approximatedName));

                champion.ChampionSpells = champion.ChampionSpells.OrderBy(ch => "QWER".IndexOf(ch.Id.Last())).ToList();
                var result = _mapper.Map<ChampionDto>(champion);
                return result;
            }, new Context($"{approximatedName}"));
        }

        public PageResult<ChampionDto> GetAll(ChampionQuery query)
        {
            return ((Policy)PollyRegister.registry["CacheStrategy"]).Execute(context => 
            {
                var baseQuery = _context.Champions
                    .AsNoTracking()
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

                foreach (var champion in champions)
                    champion.ChampionSpells = champion.ChampionSpells.OrderBy(ch => "QWER".IndexOf(ch.Id.Last())).ToList();

                var championsDtos = _mapper.Map<List<ChampionDto>>(champions);

                var result = new PageResult<ChampionDto>(championsDtos, totalItemsCount, query.PageSize, query.PageNumber);

                return result;

            }, new Context($"{query}"));
        }

        public void Delete(string name)
        {
            _logger.LogWarning($"Item with name: {name}. Delete action invoked");

            var champion = _context.Champions.FirstOrDefault(ch => ch.Name == name);

            if (champion is null)
                throw new NotFoundException("Champion not found");

            var championPassive = _context.Champions_Passives.FirstOrDefault(ch => ch.Id.Contains(name));
            var championSpells = _context.Champions_Spells.Where(ch => ch.Id.Contains(name));

            _context.Champions_Passives.Remove(championPassive);
            _context.Champions_Spells.RemoveRange(championSpells);
            _context.Champions.Remove(champion);
            _context.SaveChanges();
        }

        public void Create(CreateChampion createChampion)
        {
            var champion = _mapper.Map<Champion>(createChampion.CreateChampionDto);
            var championPassive = _mapper.Map<ChampionPassive>(createChampion.CreateChampionPassiveDto);
            var championSpells = _mapper.Map<List<ChampionSpell>>(createChampion.CreateChampionSpellDtos);

            _context.Champions.Add(champion);
            _context.SaveChanges();
            _context.Champions_Passives.Add(championPassive);
            _context.SaveChanges();
            _context.Champions_Spells.AddRange(championSpells);
            _context.SaveChanges();

        }

        public void Update(string name, UpdateChampion updateChampion)
        {
            var champion = _context.Champions.FirstOrDefault(r => r.Name == name);
            var championPassive = _context.Champions_Passives.FirstOrDefault(r => r.Id.Contains(name));
            var championSpells = _context.Champions_Spells.Where(r => r.Id.Contains(name)).ToList();

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

            _context.SaveChanges();
        }

        public string GetProperty(string name, string property)
        {
            return ((Policy)PollyRegister.registry["CacheStrategy"]).Execute(context => 
            { 
                var champion = _context.Champions.Include(r => r.ChampionSpells).Include(r => r.ChampionPassive).FirstOrDefault(r => r.Name == name);

                if (champion is null)
                    throw new NotFoundException("Champion not found");

                Helpers.Capitalize(ref property);
                var foundProperty = typeof(Champion).GetProperty(property);

                if (foundProperty is null)
                    throw new NotFoundException("Property not found");

                var result = foundProperty.GetValue(champion).ToString();
                return result;
            }, new Context($"{name} {property}"));
        }

        public string GetProperty(string name, string spellType, string property)
        {
            return ((Policy)PollyRegister.registry["CacheStrategy"]).Execute(context => 
            { 
                var champion = _context.Champions.Include(r => r.ChampionSpells).Include(r => r.ChampionPassive).FirstOrDefault(r => r.Name == name);

                if (champion is null)
                    throw new NotFoundException("Champion not found");

                Helpers.Capitalize(ref property);
                Helpers.Capitalize(ref spellType);

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

                    var spellResult = spellProperty.GetValue(champion.ChampionSpells[spellType switch
                    {
                        "E" => 0,
                        "Q" => 1,
                        "R" => 2,
                        "W" => 3,
                        _ => throw new NotFoundException("Spell type not found. It has to be \"Passive\" or \"Q\", \"W\", \"E\", \"R\"."),
                    }]).ToString();
                    return spellResult;
                }
            }, new Context($"{name} {spellType} {property}"));
        }
    }
}