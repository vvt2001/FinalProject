using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.Meeting
{
    public class MeetingFormUpdating : MeetingFormCreating
    {
        [Required(ErrorMessage = "Không xác định được cuộc họp")]
        public string id { get; set; }
    }
}
