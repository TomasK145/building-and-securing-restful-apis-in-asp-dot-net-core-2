using LandonApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi
{
    public class HotelApiDbContext : DbContext
    {
        /*Pre vyuzitie Entity framework core je treba:
         * - definovanie triedy implementujucej DbContext
         * - vytvorenie kontruktura primajuceho DbContextOptions ktory vola base konstruktor
         * - vytvorenie potrebnych DbSet
         * - referencovanie servisu v Startup.cs 
         */

        public HotelApiDbContext(DbContextOptions options) : base(options) { }

        public DbSet<RoomEntity> Rooms { get; set; }

    }
}
