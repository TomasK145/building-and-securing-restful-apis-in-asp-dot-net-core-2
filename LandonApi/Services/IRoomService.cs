using LandonApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LandonApi.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetRoomsAsync([FromQuery]SortOptions<Room, RoomEntity> sortOptions);
        Task<Room> GetRoomAsync(Guid id);
    }
}
