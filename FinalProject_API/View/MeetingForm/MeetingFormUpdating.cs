using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.MeetingForm
{
    public class MeetingFormUpdating : MeetingFormCreating
    {
        [Required(ErrorMessage = "Meeting schedule not found")]
        public string id { get; set; }
    }
}
