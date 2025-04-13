using AutoMapper;
using TodoApp.Models;
using TodoApp.DTOs.Todo;

namespace TodoApp.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Todo, TodoDto>().ReverseMap();
            CreateMap<CreateTodoDto, Todo>();
            CreateMap<UpdateTodoDto, Todo>();
        }
    }

}
