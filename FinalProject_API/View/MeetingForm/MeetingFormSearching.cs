using FinalProject_API.Filter;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.MeetingForm
{
    public class MeetingFormSearching : PaginationFilter
    {
        public string? meeting_title { get; set; }
    }
}
