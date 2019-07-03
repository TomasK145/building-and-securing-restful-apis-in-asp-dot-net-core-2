using LandonApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LandonApi.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetRoomsAsync();
        Task<Room> GetRoomAsync(Guid id);
    }
}
