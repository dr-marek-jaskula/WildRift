using System;
using System.Collections.Generic;

namespace WildRiftWebAPI
{
    public class LoginDto
    {
        //mo�naby tu doda� atrybuty walidacji, albo wykorzysta� osobny validator (ale dla uproszczenia zostawiamy tak jak jest)
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
