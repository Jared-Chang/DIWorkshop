using DependencyInjectionWorkshop.Models;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        [Test]
        public void is_valid()
        {
            var profile = Substitute.For<IProfile>();
            var hash = Substitute.For<IHash>();
            var logger = Substitute.For<ILogger>();
            var otpService = Substitute.For<IOtpService>();
            var notification = Substitute.For<INotification>();
            var failedCounter = Substitute.For<IFailedCounter>();

            profile.Password("joey").Returns("hashed password");
            hash.ComputeHash("1234").Returns("hashed password");
            otpService.CurrentOtp("joey").Returns("otp");

            var authenticationService =
                new AuthenticationService(profile, hash, otpService, notification, failedCounter, logger);

            var valid = authenticationService.Verify("joey", "1234", "otp");

            Assert.AreEqual(true, valid);
        }
    }
}