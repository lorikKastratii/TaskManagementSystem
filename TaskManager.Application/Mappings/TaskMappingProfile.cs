using AutoMapper;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mappings;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskItem, TaskDto>();
        CreateMap<CreateTaskDto, TaskItem>();
        CreateMap<UpdateTaskDto, TaskItem>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
