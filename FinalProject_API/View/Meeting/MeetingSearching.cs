using FinalProject_API.Filter;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.Meeting
{
    public class MeetingSearching : PaginationFilter
    {
        public string? meeting_title { get; set; }
    }
}
