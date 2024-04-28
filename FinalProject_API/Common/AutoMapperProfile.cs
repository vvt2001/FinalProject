using AutoMapper;
using FinalProject_API.View.Meeting;
using FinalProject_Data.Model;
using Newtonsoft.Json;

namespace eArchive.Service.Common
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MeetingForm, Meeting>();
        }
    }
}