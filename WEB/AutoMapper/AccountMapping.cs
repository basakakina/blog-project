using ApplicationCore.UserEntites.Concrete;
using AutoMapper;
using DTO.Concrete.AccountDTO;
using WEB.Models.Account;

namespace WEB.AutoMapper
{
    public class AccountMapping : Profile
    {
        public AccountMapping()
        {
            CreateMap<LoginVM, LoginDTO>().ReverseMap();
            CreateMap<RegisterVM, RegisterDTO>().ReverseMap();
        }
    }
}
