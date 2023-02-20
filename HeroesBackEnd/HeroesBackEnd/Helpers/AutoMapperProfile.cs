namespace MindBackEnd.Helpers;

using AutoMapper;
using Mind.Entity.Account;
using Mind.Entity.SecurityAccount;


public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Account, AccountResponse>();

        CreateMap<Account, AuthenticateResponse>();

        CreateMap<RegisterRequest, Account>();

        CreateMap<CreateRequest, Account>();

        CreateMap<UpdateRequest, Account>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                    return true;
                }
            ));
    }
}
