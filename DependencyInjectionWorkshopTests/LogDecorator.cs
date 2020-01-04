using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshopTests
{
    public class LogDecorator : AuthenticationDecoratorBase
    {
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public LogDecorator(IAuthenticationService authenticationService, IFailedCounter failedCounter, ILogger logger) : base(authenticationService)
        {
            _failedCounter = failedCounter;
            _logger = logger;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var valid = base.Verify(accountId, password, otp);
            if (!valid) LogFailedCount(accountId);

            return valid;
        }

        private void LogFailedCount(string accountId)
        {
            var failedCount = _failedCounter.FailedCount(accountId);
            _logger.Info($"accountId:{accountId} failed times:{failedCount}");
        }
    }
}