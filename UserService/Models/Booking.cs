using System;

namespace UserService.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GymClassId { get; set; }

        public DateTime BookingDate { get; set; }
    }
}