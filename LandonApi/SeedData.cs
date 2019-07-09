using LandonApi.Models;
using LandonApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            await AddTestUsers(services.GetRequiredService<RoleManager<UserRoleEntity>>(), services.GetRequiredService<UserManager<UserEntity>>());

            await AddTestDataAsync(services.GetRequiredService<HotelApiDbContext>(),
                                    services.GetRequiredService<IDateLogicService>()); //priklad injektovania potrebnej dependency
        }

        public static async Task AddTestDataAsync(HotelApiDbContext context, IDateLogicService dateLogicService) //zabezpecuje seedovanie DB
        {
            if (context.Rooms.Any())
            {
                //already has data
                return;
            }

            var oxford = context.Rooms.Add(new RoomEntity
            {
                Id = Guid.Parse("301df04d-8679-4b1b-ab92-0a586ae53d08"),
                Name = "Oxford Suite",
                Rate = 10119,
            }).Entity;

            context.Rooms.Add(new RoomEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195976"),
                Name = "Driscoll Suite",
                Rate = 23959
            });

            var today = DateTimeOffset.Now;
            var start = dateLogicService.AlignStartTime(today);
            var end = start.Add(dateLogicService.GetMinimumStay());

            context.Bookings.Add(new BookingEntity
            {
                Id = Guid.Parse("2eac8dea-2749-42b3-9d21-8eb2fc0fd6bd"),
                Room = oxford,
                CreatedAt = DateTimeOffset.UtcNow,
                StartAt = start,
                EndAt = end,
                Total = oxford.Rate,
            });

            await context.SaveChangesAsync();
        }

        private static async Task AddTestUsers(RoleManager<UserRoleEntity> roleManager, UserManager<UserEntity> userManager)
        {
            var dataExists = roleManager.Roles.Any() || userManager.Users.Any();
            if (dataExists)
            {
                return;
            }

            //add a test role
            await roleManager.CreateAsync(new UserRoleEntity("Admin"));

            //add a test user
            var user = new UserEntity
            {
                Email = "admin@landon.local",
                UserName = "admin@landon.local",
                FirstName = "Admin",
                LastName = "Tester",
                CreatedAt = DateTimeOffset.UtcNow
            };
            await userManager.CreateAsync(user, "123456?Aa");

            //put the user in the admin role
            await userManager.AddToRoleAsync(user, "Admin");
            
            await userManager.UpdateAsync(user);
        }
    }
}
