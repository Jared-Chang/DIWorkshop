using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop
{
    public class LogMethodDecorator: AuthenticationDecoratorBase
    {
        private readonly ILogger _logger;

        public LogMethodDecorator(IAuthenticationService authenticationService, ILogger logger) : base(authenticationService)
        {
            _logger = logger;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var valid = base.Verify(accountId, password, otp);

            _logger.Info(password);

            return valid;
        }
    }
}