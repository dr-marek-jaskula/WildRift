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
    //[Route("api/restaurant")]  //bez Routa ścieżka jest określana bazowo, jesli jest odpowiedni plik setting (nie wiem jak go zrobic)
    [Route("api/[controller]")]  //robi ze siezka jest "api/restaurant". Działa też "api/Restaurant"
    [Authorize]
    [ApiController]
    public class RestaurantController : ControllerBase //nazwa pozinna konczyc sie na "Controller"
	{
        private readonly IRestaurantService _restaurantService;

        //wstrzykujemy abstrakcje restaurantServicea (nie trzeba dbContext ani abstrakcji mappera bo jest w service)
        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        //[Authorize(Policy = "HasNationality")]
        //[Authorize(Policy = "Atleast20")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll([FromQuery] RestaurantQuery query)
        {
            //on tu poprzez parametry rozumie parametry obiektu (ciekawe). Można normalnie, ze 3 parametry
            var restaurantsDtos = _restaurantService.GetAll(query);
			return Ok(restaurantsDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);
            return Ok(restaurant);
        }

        //jak jest tylko Action Result to nic nie zwraca poza statusem
        //dodaje przez ciało zapytania 
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            int id = _restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantService.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CreatedAtLeast2Restaurants")]
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {
            _restaurantService.Update(id, dto);
            return Ok();
        }

        [HttpPost("done")]
        //[ActionName("Thumbnail")] // cos to nie dziala
        [AllowAnonymous]
        [Consumes("text/json")] //requested in heading in "Content-Type" as "text/json". Aczkolwiek jeśli nie ma header to tez przepuszcza, trzeba wymusic, zeby był header "Content-Type"
        [ProducesResponseType(StatusCodes.Status200OK)] //informuje o statusach jakie oddaje. Aczkolwiek jako, że sie robi statusy przez exceptions, to średnie to. Można tylko określić co zwróci sama akcja.
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public ActionResult ExampleMethod()
        {
            //dzieki temu wymusza aby był nagłówek o nazwie "Content-Type"
            if (!HttpContext.Request.Headers.ContainsKey("Content-Type")) return BadRequest();
            return Ok();
        }

    }
}
