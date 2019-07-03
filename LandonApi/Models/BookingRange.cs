using System;

namespace LandonApi.Models
{
    public class BookingRange
    {
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
    }
}
