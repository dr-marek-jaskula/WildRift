using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildRiftWebAPI;
using Microsoft.AspNetCore.Mvc;

namespace WildRiftWebAPI
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("[action]")]
        public ActionResult Register([FromBody] RegisterUserDto dto)
        {
            _accountService.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("[action]")]
        public ActionResult Login([FromBody] LoginDto dto)
        {
            string token = _accountService.GenerateJwt(dto);
            return Ok(token);
        }
    }
}
