using LandonApi.Infrastructure;
using LandonApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly HotelInfo _hotelInfo;

        public InfoController(IOptions<HotelInfo> hotelInfoWrapper) //wrapper dat ktore su pushnute do service containera
        {
            _hotelInfo = hotelInfoWrapper.Value;
        }

        [HttpGet(Name = nameof(GetInfo))]
        [ProducesResponseType(200)]
        [ProducesResponseType(304)]
        //[ResponseCache(Duration = 86400)] //nastavenie trvania response cache --> zabezpeci pridanie custom headeru do response
        //[ResponseCache(CacheProfileName = "Static")] //nastavenie cachovania je mozne aj aplikovanim Cache profile name (vytvoreny profile v Startup.cs)
        [Etag] //custom generovanie ETag headeru v response --> pozri vyuzitie Interface IEtaggable
        public ActionResult<HotelInfo> GetInfo() //vyuzitie ActionResult namiesto IActionResult definuje ze bude navratenu z metody strongly-typed model
        {
            //_hotelInfo.Self = Url.Link(nameof(GetInfo), null);
            _hotelInfo.Href = Url.Link(nameof(GetInfo), null);

            if (!Request.GetEtagHandler().NoneMatch(_hotelInfo)) //porovanie requestu, ak je ETag zhodny, je response 304 - NotModified
            {
                return StatusCode(304, _hotelInfo);
            }

            return _hotelInfo;
        }
    }
}
