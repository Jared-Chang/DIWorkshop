using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshopTests
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