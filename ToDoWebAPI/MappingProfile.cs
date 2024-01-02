using AutoMapper;
using ToDoWebAPI.Dtos;
using ToDoWebAPI.Models;

namespace ToDoWebAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Todo,TodoGetResponse>();
            CreateMap<Todo,TodoGetAllResponse>();
            CreateMap<TodoInfo,TodoInfoResponse>();
        }
    }
}