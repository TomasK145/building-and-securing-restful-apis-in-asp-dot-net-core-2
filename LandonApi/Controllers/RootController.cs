using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/")] //URL na ktorej bude tento kontroller volany
    [ApiController] //info ze controller je urceny pre API, pridava feature ako automaticku model validaciu
    public class RootController : ControllerBase //pre web api je vhodne dedit od ControllerBase --> chybaju funkcionality pre view, razor, ... ktore su definovane v triede Controller (aj tato dedi od ControllerBase)
    {
        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var response = new
            {
                href = Url.Link(nameof(GetRoot), null) //generovanie absolute path
            };
            return Ok(response);
        }
    }
}
