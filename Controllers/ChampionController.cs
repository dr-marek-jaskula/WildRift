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
    [ApiController]
    public class ChampionController : ControllerBase 
	{
        private readonly IChampionService _championtService;

        public ChampionController(IChampionService championService)
        {
            _championtService = championService;
        }

        [HttpGet("{name}")]
        public ActionResult<ChampionDto> Get([FromRoute] string name)
        {
            var champion = _championtService.GetByName(name);
            return Ok(champion);
        }

        [HttpGet]
        public ActionResult<IEnumerable<ChampionDto>> GetAll([FromQuery] ChampionQuery query)
        {
            var championsDtos = _championtService.GetAll(query);
            return Ok(championsDtos);
        }

    }
}
