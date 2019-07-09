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
        private readonly IDateLogicService _dateLogicService;
        private readonly IBookingService _bookingService;
        private readonly PagingOptions _defaultPagingOptions;

        public RoomsController(IRoomService roomService, IOpeningService openingService, IOptions<PagingOptions> defaultPagingOptionsWrapper, IDateLogicService dateLogicService, IBookingService bookingService)
        {
            _roomService = roomService;
            _openingService = openingService;
            _dateLogicService = dateLogicService;
            _bookingService = bookingService;
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
        [ResponseCache(Duration = 30, //pri nastaveni AddResponseCaching zabezpeci cachovanie aj na strane servera po dobu 30s
            VaryByQueryKeys = new[] { "offset", "limit", "orderBy", "search" })]  //urcenie ze cachovanie sa bude lisit podla query stringu
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

        //TODO authentication
        //POST /rooms/{roomId}/bookings
        [HttpPost("{roomId}/bookings", Name = nameof(CreateBookingForRoom))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> CreateBookingForRoom(Guid roomId, [FromBody] BookingForm bookingForm) //object je bindovany z body POST requestu, asp.net core robi model validaciu automaticky
        {
            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }

            var minimumStay = _dateLogicService.GetMinimumStay();
            bool tooShort = (bookingForm.EndAt.Value - bookingForm.StartAt.Value) < minimumStay;
            if (tooShort)
            {
                return BadRequest(new ApiError($"The minimum booking duration is {minimumStay.TotalHours} hours."));
            }

            var conflictedSlots = await _openingService.GetConflictingSlots(roomId, bookingForm.StartAt.Value, bookingForm.EndAt.Value);
            if (conflictedSlots.Any())
            {
                return BadRequest(new ApiError("This time conflicts with an existing booking."));
            }

            //TODO: Get the current user
            var userId = Guid.NewGuid();

            var bookingId = await _bookingService.CreateBookingAsync(userId, roomId, bookingForm.StartAt.Value, bookingForm.EndAt.Value);
            return Created(Url.Link(nameof(BookingsController.GetBookingById), new { bookingId }), null);
        }
    }
}
