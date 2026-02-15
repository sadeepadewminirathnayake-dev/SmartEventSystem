using System.Collections.Generic;

namespace SmartEventSystem.Models
{
    public class Venue
    {
        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public string Address { get; set; }
        public int Capacity { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
