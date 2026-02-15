using System;

namespace SmartEventSystem.Models
{
    public class Event
    {
        public int EventID { get; set; }

        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public TimeSpan EventTime { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }


    }
}

