using AutoMapper;
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
    public class ChampionController : ControllerBase 
	{
        private readonly IChampionService _championtService;

        public ChampionController(IChampionService championService)
        {
            _championtService = championService;
        }

        [HttpGet("{name}")]
        [AllowAnonymous]
        public ActionResult<ChampionDto> Get([FromRoute] string name)
        {
            var championDto = _championtService.GetByName(name);
            return Ok(championDto);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ChampionDto>> GetAll([FromQuery] ChampionQuery query)
        {
            var championsDtos = _championtService.GetAll(query);
            return Ok(championsDtos);
        }


        [HttpDelete("{name}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] string name)
        {
            _championtService.Delete(name);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([FromBody] CreateChampion createChampion)
        {
            _championtService.Create(createChampion);
            return Created($"/api/champion/{createChampion.CreateChampionDto.Name}", null);
        }

        [HttpPut("{name}")]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult Update([FromRoute] string name, [FromBody] UpdateChampion updateChampion)
        {
            _championtService.Update(name, updateChampion);
            return Ok();
        }

    }
}
