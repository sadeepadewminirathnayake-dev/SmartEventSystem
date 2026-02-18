using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SmartEventSystem.Models;

namespace SmartEventSystem.ViewModels
{
    public class EventSearchViewModel
    {
        // Search / Filter inputs
        public string SearchName { get; set; }                  // free text search in event name
        public int? CategoryID { get; set; }                    // dropdown
        public int? VenueID { get; set; }                       // dropdown
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MaxPrice { get; set; }                  // show events <= this price

        // Results
        public List<EventListItemViewModel> Events { get; set; } = new List<EventListItemViewModel>();

        // For dropdowns
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Venue> Venues { get; set; }

        // Optional: paging info
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }

    public class EventListItemViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string CategoryName { get; set; }
        public string VenueName { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }     // short version maybe
        public bool IsAvailable { get; set; }       // you can compute this later
        public string AvailabilityStatus { get; set; }

    }
}
