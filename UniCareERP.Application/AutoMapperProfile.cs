using AutoMapper;
using UniCareERP.Application.DTOs.Schedules;
using UniCareERP.Domain.Entities.Schedules;

namespace UniCareERP.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Schedule, ScheduleDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"));
            CreateMap<ScheduleDto, Schedule>();
        }
    }
}
