using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using UniCareERP.Application.DTOs.Schedules;
using UniCareERP.Domain.Entities.Schedules;
using UniCareERP.Infrastructure.Repositories.Schedules;

namespace UniCareERP.Application.Services.Schedules
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;

        public ScheduleService(IScheduleRepository scheduleRepository, IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
        }

        public async Task<ScheduleDto> CreateScheduleAsync(ScheduleDto scheduleDto)
        {
            var schedule = _mapper.Map<Schedule>(scheduleDto);
            await _scheduleRepository.AddAsync(schedule);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task DeleteScheduleAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule != null)
            {
                _scheduleRepository.Remove(schedule);
            }
        }

        public async Task<ScheduleDto> GetScheduleByIdAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<IEnumerable<ScheduleDto>> GetSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task UpdateScheduleAsync(Guid id, ScheduleDto scheduleDto)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule != null)
            {
                _mapper.Map(scheduleDto, schedule);
                _scheduleRepository.Update(schedule);
            }
        }
    }
}
