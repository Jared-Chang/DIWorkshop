using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshopTests
{
    public class NotificationDecorator : AuthenticationDecoratorBase
    {
        private readonly INotification _notification;

        public NotificationDecorator(IAuthenticationService authenticationService, INotification notification) : base(
            authenticationService)
        {
            _notification = notification;
        }

        public override bool Verify(string accountId, string password, string otp)
        {
            var valid = base.Verify(accountId, password, otp);

            if (!valid) _notification.Notify(accountId, $"account:{accountId} try to login failed");

            return valid;
        }
    }
}