using LandonApi.Models;
using LandonApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/[controller]")] //[controller] matchne nazov kontrolera s odstranenym slova "controller"
    [ApiController] //zabezpecuje napr ze nie je treba kazdu action zacat: if (!ModelState.Valid) { return BadRequest(); } ...
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IOpeningService _openingService;
        private readonly PagingOptions _defaultPagingOptions;

        public RoomsController(IRoomService roomService, IOpeningService openingService, IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _roomService = roomService;
            _openingService = openingService;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        [HttpGet(Name = nameof(GetAllRooms))]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Room>>> GetAllRooms([FromQuery]SortOptions<Room, RoomEntity> sortOptions, [FromQuery] SearchOptions<Room, RoomEntity> searchOptions)
        {
            var rooms = await _roomService.GetRoomsAsync(sortOptions, searchOptions);

            var collection = new Collection<Room>
            {
                Self = Link.ToCollection(nameof(GetAllRooms)),
                Value = rooms.ToArray()
            };
            return collection;
        }

        // GET /rooms/openings
        [HttpGet("openings", Name = nameof(GetAllRoomOpenings))]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Opening>>> GetAllRoomOpenings(
            [FromQuery]PagingOptions pagingOptions = null) //zadefinovanie ze hodnoty sa nachadzaju v query stringu
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var openings = await _openingService.GetOpeningsAsync(pagingOptions);

            var collection = PagedCollection<Opening>.Create(
                                                                Link.ToCollection(nameof(GetAllRoomOpenings)),
                                                                openings.Items.ToArray(),
                                                                openings.TotalSize,
                                                                pagingOptions
                                                            );

            return collection;
        }

        // GET /rooms/{roomId}
        [HttpGet("{roomId}", Name = nameof(GetRoomById))] //postacuje definovanie Route len pre action, pretoze controller uz cast definuje (/rooms/...)
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Room>> GetRoomById(Guid roomId)
        {
            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }
            return room; //automaticky nastavuje response code na 200
        }

        //POST /rooms/{roomId}/bookings
        [HttpPost("{roomId}/bookings", Name = nameof(CreateBookingForRoom))]
        public async Task<ActionResult> CreateBookingForRoom(Guid roomId, [FromBody] BookingForm bookingForm) //object je bindovany z body POST requestu
        {
            throw new NotImplementedException();
            //2:58
        }
    }
}
