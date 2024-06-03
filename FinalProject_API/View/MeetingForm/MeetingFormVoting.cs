using FinalProject_Data.Model;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.MeetingForm
{
    public class MeetingFormVoting
    {
        [Required(ErrorMessage = "Meeting schedule not found")]
        public string meetingform_id { get; set; }
        public List<string> meetingtime_ids { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }
    }
}
