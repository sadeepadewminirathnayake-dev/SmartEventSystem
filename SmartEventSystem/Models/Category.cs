using System.Collections.Generic;

namespace SmartEventSystem.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}

