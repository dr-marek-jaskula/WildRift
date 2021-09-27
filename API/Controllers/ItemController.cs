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
    public class ItemController : ControllerBase
	{
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet("{name}/{property}")]
        [AllowAnonymous]
        public ActionResult<string> GetProperty([FromRoute] string name, [FromRoute] string property)
        {
            var itemProperyDto = _itemService.GetProperty(name, property);
            return Ok(itemProperyDto);
        }

        [HttpGet("{name}")]
        [AllowAnonymous]
        public ActionResult<ChampionDto> Get([FromRoute] string name)
        {
            var itemDto = _itemService.GetByName(name);
            return Ok(itemDto);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ChampionDto>> GetAll([FromQuery] ItemQuery query)
        {
            var itemDtos = _itemService.GetAll(query);
            return Ok(itemDtos);
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] string name)
        {
            _itemService.Delete(name);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([FromBody] CreateItemDto createItem)
        {
            _itemService.Create(createItem);
            return Created($"/api/champion/{createItem.Name}", null);
        }

        [HttpPut("{name}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Update([FromRoute] string name, [FromBody] UpdateItemDto updateItem)
        {
            _itemService.Update(name, updateItem);
            return Ok();
        }
    }
}
