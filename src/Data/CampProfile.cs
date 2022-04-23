using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                .ForMember(m => m.Venue, c => c.MapFrom(c => c.Location.VenueName)).ReverseMap();
            this.CreateMap<Talk, TalkModel>().ReverseMap()
                .ForMember(t => t.Camp, options => options.Ignore())
                .ForMember(t => t.Speaker, options => options.Ignore());
            this.CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
