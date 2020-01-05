using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop
{
    public class AuditLogDecorator : AuthenticationDecoratorBase
    {
        private readonly ILogger _logger;
        private readonly IContext _context;

        public AuditLogDecorator(IAuthenticationService authenticationService, ILogger logger, IContext context) : base(authenticationService)
        {
            _logger = logger;
            _context = context;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            string userName = _context.GetUser().Name;
            _logger.Info($"Audit Log -> user:{userName}, parameters:{accountId} | {password} | {otp}");

            var isValid = base.Verify(accountId, password, otp);

            _logger.Info($"return value: {isValid}");

            return isValid;
        }
    }
}