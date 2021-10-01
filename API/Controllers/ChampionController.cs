using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<string> GetProperty([FromRoute] string name, [FromRoute] string property)
        {
            var championProperyDto = _championtService.GetProperty(name, property);
            return Ok(championProperyDto);
        }

        [HttpGet("{name}/{spellType}/{property}")]
        [AllowAnonymous]
        public ActionResult<string> GetProperty([FromRoute] string name, [FromRoute] string spellType, [FromRoute] string property)
        {
            var championProperyDto = _championtService.GetProperty(name, spellType, property);
            return Ok(championProperyDto);
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
        [Authorize(Roles = "Admin")]
        public ActionResult Update([FromRoute] string name, [FromBody] UpdateChampion updateChampion)
        {
            _championtService.Update(name, updateChampion);
            return Ok();
        }
    }
}