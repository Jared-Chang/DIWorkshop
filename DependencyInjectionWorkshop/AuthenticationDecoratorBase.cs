using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop
{
    public abstract class AuthenticationDecoratorBase : IAuthenticationService
    {
        private readonly IAuthenticationService _authenticationService;

        protected AuthenticationDecoratorBase(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public virtual bool Verify(string accountId, string password, string otp)
        {
            return _authenticationService.Verify(accountId, password, otp);
        }
    }
}