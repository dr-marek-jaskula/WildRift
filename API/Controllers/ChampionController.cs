﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    [Route("api/[controller]")]
    [Authorize]
    public class ChampionController : ControllerBase
    {
        private readonly IChampionService _championtService;

        public ChampionController(IChampionService championService)
        {
            _championtService = championService;
        }

        [HttpGet("{name}/{property}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> GetProperty([FromRoute] string name, [FromRoute] string property)
        {
            var championProperyDto = _championtService.GetProperty(name, property);
            return Ok(championProperyDto);
        }

        [HttpGet("{name}/{spellType}/{property}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> GetProperty([FromRoute] string name, [FromRoute] string spellType, [FromRoute] string property)
        {
            var championProperyDto = _championtService.GetProperty(name, spellType, property);
            return Ok(championProperyDto);
        }

        [HttpGet("{name}", Name = "GetChampionByName")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChampionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ChampionDto> GetChampionByName([FromRoute] string name)
        {
            var championDto = _championtService.GetByName(name);
            return Ok(championDto);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ChampionDto>))]
        public ActionResult<IEnumerable<ChampionDto>> GetAll([FromQuery] ChampionQuery query)
        {
            var championsDtos = _championtService.GetAll(query);
            return Ok(championsDtos);
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete([FromRoute] string name)
        {
            _championtService.Delete(name);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateChampion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Create([FromBody] CreateChampion createChampion)
        {
            _championtService.Create(createChampion);
            return CreatedAtAction(nameof(GetChampionByName), new { name = createChampion.CreateChampionDto.Name }, createChampion);
        }

        [HttpPut("{name}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update([FromRoute] string name, [FromBody] UpdateChampion updateChampion)
        {
            _championtService.Update(name, updateChampion);
            return Ok();
        }
    }
}