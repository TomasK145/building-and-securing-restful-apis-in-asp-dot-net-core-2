using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/")] //URL na ktorej bude tento kontroller volany
    [ApiController] //info ze controller je urceny pre API, pridava feature ako automaticku model validaciu
    [ApiVersion("1.0")] //definovanie verzie API
    public class RootController : ControllerBase //pre web api je vhodne dedit od ControllerBase --> chybaju funkcionality pre view, razor, ... ktore su definovane v triede Controller (aj tato dedi od ControllerBase)
    {
        [HttpGet(Name = nameof(GetRoot))]
        [ProducesResponseType(200)] //nie je potrebny atribut, vhodny ale pre pouzitie pre Swagger
        public IActionResult GetRoot()
        {
            //vyuzivana ION JSON specifikacia (https://ionspec.org/)
            var response = new
            {
                href = Url.Link(nameof(GetRoot), null), //generovanie absolute path
                rooms = new
                {
                    href = Url.Link(nameof(RoomsController.GetRooms), null)
                },
                info = new
                {
                    href = Url.Link(nameof(InfoController.GetInfo), null)
                }
            };
            return Ok(response);
        }
    }
}
