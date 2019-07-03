using LandonApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LandonApi.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetRoomsAsync(SortOptions<Room, RoomEntity> sortOptions, SearchOptions<Room, RoomEntity> searchOptions);
        Task<Room> GetRoomAsync(Guid id);
    }
}
