﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using LandonApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Services
{
    public class DefaultRoomService : IRoomService
    {
        private readonly HotelApiDbContext _context;
        private readonly IConfigurationProvider _mappingConfiguration;

        public DefaultRoomService(HotelApiDbContext context, IConfigurationProvider mappingConfiguration)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
        }

        public async Task<Room> GetRoomAsync(Guid id)
        {
            var entity = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return null;
            }

            //return new Room
            //{
            //    Href = null, // Url.Link(nameof(GetRoomById), new { roomId = entity.Id }),
            //    Name = entity.Name,
            //    Rate = entity.Rate / 100.0m //v EF modely ulozene ako INT 
            //};
            var mapper = _mappingConfiguration.CreateMapper();
            return mapper.Map<Room>(entity);
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync(SortOptions<Room, RoomEntity> sortOptions, SearchOptions<Room, RoomEntity> searchOptions)
        {
            IQueryable<RoomEntity> query = _context.Rooms;
            query = searchOptions.Apply(query); //zabezpecenie searchovania 
            query = sortOptions.Apply(query); //zabezpecenie sortovania
            return await query.ProjectTo<Room>(_mappingConfiguration).ToArrayAsync();
        }
    }
}
