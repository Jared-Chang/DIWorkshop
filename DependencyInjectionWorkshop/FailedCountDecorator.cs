using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop
{
    public class FailedCountDecorator : AuthenticationDecoratorBase   
    {
        private readonly IFailedCounter _failedCounter;

        public FailedCountDecorator(IAuthenticationService authenticationService, IFailedCounter failedCounter) : base(authenticationService)
        {
            _failedCounter = failedCounter;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            if (_failedCounter.IsLocked(accountId)) throw new FailedTooManyTimesException {AccountId = accountId};

            var valid = base.Verify(accountId, password, otp);

            if (valid)
                _failedCounter.Reset(accountId);
            else
                _failedCounter.Increase(accountId);

            return valid;
        }
    }
}