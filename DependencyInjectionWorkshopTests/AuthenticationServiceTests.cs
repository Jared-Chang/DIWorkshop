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

        [Test]
        public void is_valid()
        {
            GivenPasswordFromDb(DefaultAccountId, DefaultHashedPassword);
            GivenHashedPassword(DefaultPassword, DefaultHashedPassword);
            GivenOtp(DefaultAccountId, DefaultOtp);

            var valid = _authenticationService.Verify(DefaultAccountId, DefaultPassword, DefaultOtp);

            Assert.AreEqual(true, valid);
        }
    }
}