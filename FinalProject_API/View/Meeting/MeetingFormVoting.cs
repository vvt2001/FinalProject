using FinalProject_Data.Model;

namespace FinalProject_API.View.Meeting
{
    public class MeetingFormVoting
    {
        public string meetingform_id { get; set; }
        public List<string> meetingtime_ids { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }
}
