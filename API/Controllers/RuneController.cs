using AutoMapper;
using Eltin_Buchard_Keller_Algorithm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WildRiftWebAPI
{
    [Route("api/[controller]")] 
    [Authorize]
    [ApiController]
    public class RuneController : ControllerBase
	{
        private readonly IRuneService _runeService;

        public RuneController(IRuneService runeService)
        {
            _runeService = runeService;
        }

        [HttpGet("{name}/{property}")]
        [AllowAnonymous]
        public ActionResult<string> GetProperty([FromRoute] string name, [FromRoute] string property)
        {
            var runeProperyDto = _runeService.GetProperty(name, property);
            return Ok(runeProperyDto);
        }

        [HttpGet("{name}")]
        [AllowAnonymous]
        public ActionResult<ChampionDto> Get([FromRoute] string name)
        {
            var runeDto = _runeService.GetByName(name);
            return Ok(runeDto);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ChampionDto>> GetAll([FromQuery] RuneQuery query)
        {
            var runeDtos = _runeService.GetAll(query);
            return Ok(runeDtos);
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] string name)
        {
            _runeService.Delete(name);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([FromBody] CreateRuneDto createRune)
        {
            _runeService.Create(createRune);
            return Created($"/api/champion/{createRune.Name}", null);
        }

        [HttpPut("{name}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Update([FromRoute] string name, [FromBody] UpdateRuneDto updateRune)
        {
            _runeService.Update(name, updateRune);
            return Ok();
        }
    }
}
