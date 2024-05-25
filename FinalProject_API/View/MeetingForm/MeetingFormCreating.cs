using FinalProject_Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject_API.View.MeetingForm
{
    public class MeetingFormCreating
    {
        public string meeting_title { get; set; }
        public string? meeting_description { get; set; }
        public string? location { get; set; }
        public List<DateTime> times { get; set; }
        public int duration { get; set; }
        public int platform { get;set; }
    }
}
