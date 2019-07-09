using AspNet.Security.OpenIdConnect.Primitives;
using LandonApi.Models;
using LandonApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/[controller]")]
    [Authorize]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserInfoController(IUserService userService)
        {
            _userService = userService;
        }

        //GET /userinfo
        [HttpGet(Name = nameof(UserInfo))]
        [ProducesResponseType(401)]
        public async Task<ActionResult<UserinfoResponse>> UserInfo()
        {
            var user = await _userService.GetUserAsync(User); //objekt User je k dispozicii vramci kontextu requestu
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The user does not exists."
                });
            }

            var userId = _userService.GetUserIdAsync(User);

            return new UserinfoResponse
            {
                Self = Link.To(nameof(UserInfo)),
                GivenName = user.FirstName,
                FamilyName = user.LastName,
                Subject = Url.Link(nameof(UsersController.GetUserById), new { userId })
            };
        }
    }
}
