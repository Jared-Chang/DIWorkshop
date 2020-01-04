using DependencyInjectionWorkshop.Models;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            _profileDao = Substitute.For<IProfile>();
            _hash = Substitute.For<IHash>();
            _logger = Substitute.For<ILogger>();
            _otpService = Substitute.For<IOtpService>();
            _notification = Substitute.For<INotification>();
            _failedCounter = Substitute.For<IFailedCounter>();

            _authenticationService =
                new AuthenticationService(_profileDao, _hash, _otpService, _notification, _failedCounter, _logger);
        }

        private const string DefaultAccountId = "joey";
        private const string DefaultHashedPassword = "hashed password";
        private const string DefaultPassword = "1234";
        private const string DefaultOtp = "otp";

        private IProfile _profileDao;
        private IHash _hash;
        private ILogger _logger;
        private IOtpService _otpService;
        private INotification _notification;
        private IFailedCounter _failedCounter;
        private AuthenticationService _authenticationService;

        private void GivenOtp(string accountId, string returnThis)
        {
            _otpService.CurrentOtp(accountId).Returns(returnThis);
        }

        private void GivenHashedPassword(string password, string hashedPassword)
        {
            _hash.ComputeHash(password).Returns(hashedPassword);
        }

        private void GivenPasswordFromDb(string accountId, string hashedPassword)
        {
            _profileDao.Password(accountId).Returns(hashedPassword);
        }

        private void ShouldBeInvalid(string defaultAccountId, string defaultPassword, string otp)
        {
            var valid = _authenticationService.Verify(defaultAccountId, defaultPassword, otp);

            Assert.AreEqual(false, valid);
        }

        private void ShouldBeValid(string defaultAccountId, string defaultPassword, string defaultOtp)
        {
            var valid = _authenticationService.Verify(defaultAccountId, defaultPassword, defaultOtp);

            Assert.AreEqual(true, valid);
        }

        private void WhenValid()
        {
            GivenPasswordFromDb(DefaultAccountId, DefaultHashedPassword);
            GivenHashedPassword(DefaultPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            _authenticationService.Verify(DefaultAccountId, DefaultPassword, DefaultOtp);
        }

        private void ShouldResetFailedCount(string accountId)
        {
            _failedCounter.Received(1).Reset(accountId);
        }

        [Test]
        public void is_invalid()
        {
            GivenPasswordFromDb(DefaultAccountId, DefaultHashedPassword);
            GivenHashedPassword(DefaultPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            ShouldBeInvalid(DefaultAccountId, DefaultPassword, "wrong otp");
        }

        [Test]
        public void is_valid()
        {
            GivenPasswordFromDb(DefaultAccountId, DefaultHashedPassword);
            GivenHashedPassword(DefaultPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            ShouldBeValid(DefaultAccountId, DefaultPassword, DefaultOtp);
        }

        [Test]
        public void reset_failed_count_when_valid()
        {
            WhenValid();

            ShouldResetFailedCount(DefaultAccountId);
        }
    }
}