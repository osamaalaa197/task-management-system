using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using TaskManagement.Core.Models;
using TaskManagement.Models;
using TaskManagement.ViewModels;

namespace TaskManagement.Mapping
{
    public class MappingProfile:Profile 
    {
        public MappingProfile() 
        {
            CreateMap<Assignment,AssignmentViewModel>().ForMember(dest=>dest.Key,opt=>opt.MapFrom(src=>src.Id)).ReverseMap();
            CreateMap<Team, TeamViewModel>().ReverseMap();
            CreateMap<ApplicationUser, UserDataViewModel>().ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.TeamName)).ReverseMap();
            CreateMap<Team, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.TeamId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.TeamName));

            CreateMap<Assignment,RawData>().ReverseMap();
        }
    }
}
