using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WildRiftWebAPI
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiVersion("1.0")]
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
        public async Task<ActionResult<string>> GetProperty([FromRoute] string name, [FromRoute] string property)
        {
            var championProperyDto = await _championtService.GetProperty(name, property);
            return Ok(championProperyDto);
        }

        [HttpGet("{name}/{spellType}/{property}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetProperty([FromRoute] string name, [FromRoute] string spellType, [FromRoute] string property)
        {
            var championProperyDto = await _championtService.GetProperty(name, spellType, property);
            return Ok(championProperyDto);
        }

        [HttpGet("{name}", Name = "GetChampionByName")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChampionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChampionDto>> GetChampionByName([FromRoute] string name)
        {
            var championDto = await _championtService.GetByName(name);
            return Ok(championDto);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ChampionDto>))]
        public async Task<ActionResult<IEnumerable<ChampionDto>>> GetAll([FromQuery] ChampionQuery query)
        {
            var championsDtos = await _championtService.GetAll(query);
            return Ok(championsDtos);
        }

        [HttpGet("Names")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
        public ActionResult<IEnumerable<string>> GetAllNames()
        {
            var championNames = _championtService.GetAllNames();
            return Ok(championNames);
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