using AutoMapper;
using UsersMessages.Db;
using UsersMessages.Dto;

namespace UsersMessages.Mapper;

public class MappingProFile : Profile
{
    public MappingProFile()
    {
        CreateMap<Message, MessageDto>(MemberList.Destination).ReverseMap();
    }
}