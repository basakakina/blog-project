using DTO.Concrete.AccountDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Validation
{
    public class LoginDTOValidation: AbstractValidator<LoginDTO>
    {
        public LoginDTOValidation() 
        {
            RuleFor(x => x.UserName)
           .NotEmpty().WithMessage("Lütfen kullanıcı adını giriniz.")
           .MinimumLength(3).WithMessage("Kullanıcı en az 3 karakterden oluşmak zorunda.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Lütfen şifre giriniz.")
                .MinimumLength(6).WithMessage("Şifreniz en az 6 karakterden oluşmak zorunda.");
        }
    }
}
