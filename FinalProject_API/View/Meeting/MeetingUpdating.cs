﻿using System.ComponentModel.DataAnnotations;

namespace FinalProject_API.View.Meeting
{
    public class MeetingUpdating
    {
        [Required(ErrorMessage = "Không xác định được cuộc họp")]
        public string id { get; set; }
        public string meeting_title { get; set; }
        public string? meeting_description { get; set; }
        public string? location { get; set; }
        public DateTime starttime { get; set; }
        public int duration { get; set; }
    }
}
