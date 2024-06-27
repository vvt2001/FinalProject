using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.Meeting
{
    public class MeetingNote
    {
        [Required(ErrorMessage = "Meeting not found")]
        public string meeting_id { get; set; }
        public string? content { get; set; } 
    }
}
