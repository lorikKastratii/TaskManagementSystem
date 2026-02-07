using AutoMapper;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mappings;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        // TaskItem to TaskDto - map StatusId/PriorityId Guids to integer enum values
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => (int)src.StatusEnum))
            .ForMember(dest => dest.PriorityId, opt => opt.MapFrom(src => (int)src.PriorityEnum));

        // CreateTaskDto to TaskItem - map integer IDs to Guid foreign keys
        CreateMap<CreateTaskDto, TaskItem>()
            .ForMember(dest => dest.PriorityId, opt => opt.MapFrom(src => TaskLookupIds.GetPriorityIdFromInt(src.PriorityId)))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => TaskLookupIds.GetStatusIdFromInt(src.StatusId)))
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Priority, opt => opt.Ignore());

        // UpdateTaskDto to TaskItem - map integer IDs to Guid foreign keys when not null
        CreateMap<UpdateTaskDto, TaskItem>()
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom((src, dest) => 
                src.StatusId.HasValue ? TaskLookupIds.GetStatusIdFromInt(src.StatusId.Value) : dest.StatusId))
            .ForMember(dest => dest.PriorityId, opt => opt.MapFrom((src, dest) => 
                src.PriorityId.HasValue ? TaskLookupIds.GetPriorityIdFromInt(src.PriorityId.Value) : dest.PriorityId))
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Priority, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

