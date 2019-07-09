using LandonApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi
{
    public class HotelApiDbContext : IdentityDbContext<UserEntity, UserRoleEntity, Guid> //vyuzitie IdentityDbContext zabezpeci vytvorenie potrebnych tabuliek pre Identity, nie je potrebne vytvarat manualne
    {
        /*Pre vyuzitie Entity framework core je treba:
         * - definovanie triedy implementujucej DbContext
         * - vytvorenie kontruktura primajuceho DbContextOptions ktory vola base konstruktor
         * - vytvorenie potrebnych DbSet
         * - referencovanie servisu v Startup.cs 
         */

        public HotelApiDbContext(DbContextOptions options) : base(options) { }

        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<BookingEntity> Bookings { get; set; }
    }
}
