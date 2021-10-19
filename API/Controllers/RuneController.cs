﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    [Route("api/[controller]")]
    [Authorize]
    public class RuneController : ControllerBase
    {
        private readonly IRuneService _runeService;

        public RuneController(IRuneService runeService)
        {
            _runeService = runeService;
        }

        [HttpGet("{name}/{property}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> GetProperty([FromRoute] string name, [FromRoute] string property)
        {
            var runeProperyDto = _runeService.GetProperty(name, property);
            return Ok(runeProperyDto);
        }

        [HttpGet("{name}", Name = "GetRuneByName")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RuneDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RuneDto> GetRuneByName([FromRoute] string name)
        {
            var runeDto = _runeService.GetByName(name);
            return Ok(runeDto);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RuneDto>))]
        public ActionResult<IEnumerable<RuneDto>> GetAll([FromQuery] RuneQuery query)
        {
            var runeDtos = _runeService.GetAll(query);
            return Ok(runeDtos);
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete([FromRoute] string name)
        {
            _runeService.Delete(name);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateRuneDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Create([FromBody] CreateRuneDto createRune)
        {
            _runeService.Create(createRune);
            return CreatedAtAction(nameof(GetRuneByName), new { name = createRune.Name }, createRune);
        }

        [HttpPut("{name}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update([FromRoute] string name, [FromBody] UpdateRuneDto updateRune)
        {
            _runeService.Update(name, updateRune);
            return Ok();
        }
    }
}