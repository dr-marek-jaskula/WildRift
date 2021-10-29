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
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet("{name}/{property}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetProperty([FromRoute] string name, [FromRoute] string property)
        {
            var itemProperyDto = await _itemService.GetProperty(name, property);
            return Ok(itemProperyDto);
        }

        [HttpGet("{name}", Name = "GetItemByName")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemDto>> GetItemByName([FromRoute] string name)
        {
            var itemDto = await _itemService.GetByName(name);
            return Ok(itemDto);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ItemDto>))]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll([FromQuery] ItemQuery query)
        {
            var itemDtos = await _itemService.GetAll(query);
            return Ok(itemDtos);
        }

        [HttpDelete("{name}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete([FromRoute] string name)
        {
            _itemService.Delete(name);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Create([FromBody] CreateItemDto createItem)
        {
            _itemService.Create(createItem);
            return CreatedAtAction(nameof(GetItemByName), new { name = createItem.Name }, createItem);
        }

        [HttpPut("{name}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update([FromRoute] string name, [FromBody] UpdateItemDto updateItem)
        {
            _itemService.Update(name, updateItem);
            return Ok();
        }
    }
}
