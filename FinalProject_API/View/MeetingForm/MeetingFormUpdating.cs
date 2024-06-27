using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.MeetingForm
{
    public class MeetingFormUpdating : MeetingFormCreating
    {
        [Required(ErrorMessage = "Không xác định được lịch họp")]
        public string id { get; set; }
    }
}
