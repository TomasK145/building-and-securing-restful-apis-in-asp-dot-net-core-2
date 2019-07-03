using LandonApi.Models;
using System;
using System.Collections.Generic;

namespace LandonApi.Services
{
    public interface IDateLogicService
    {
        DateTimeOffset AlignStartTime(DateTimeOffset date);

        TimeSpan GetMinimumStay();

        DateTimeOffset FurthestPossibleBooking(DateTimeOffset now);

        IEnumerable<BookingRange> GetAllSlots(
            DateTimeOffset start,
            DateTimeOffset? end = null);

        bool DoesConflict(BookingRange b, DateTimeOffset start, DateTimeOffset end);
    }
}
