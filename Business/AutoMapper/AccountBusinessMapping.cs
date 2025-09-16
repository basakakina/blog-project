using AutoMapper;
using ApplicationCore.UserEntites.Concrete;   
using DTO.Concrete.AccountDTO;

namespace Business.AutoMapper
{
    public class AccountBusinessMapping : Profile
    {
        public AccountBusinessMapping()
        {
           
            CreateMap<RegisterDTO, AppUser>(MemberList.None)
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.SecurityStamp, o => o.Ignore())
                .ForMember(d => d.ConcurrencyStamp, o => o.Ignore());
            

            CreateMap<EditUserDTO, AppUser>(MemberList.None)
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.SecurityStamp, o => o.Ignore())
                .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
                .ReverseMap();

    
            CreateMap<AppUser, GetUserDTO>(MemberList.None);
        }
    }
}
