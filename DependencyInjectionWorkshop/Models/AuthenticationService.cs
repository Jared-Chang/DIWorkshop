using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly IProfile _profileDao;
        private readonly IHash _hash;
        private readonly IOtpService _otpService;
        private readonly INotification _notification;
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public AuthenticationService(IProfile profileDao, IHash hash, IOtpService otpService, INotification notification, IFailedCounter failedCounter, ILogger logger)
        {
            _profileDao = profileDao;
            _hash = hash;
            _otpService = otpService;
            _notification = notification;
            _failedCounter = failedCounter;
            _logger = logger;
        }

        public AuthenticationService()
        {
            _profileDao = new ProfileDao();
            _hash = new Sha256Adapter();
            _otpService = new OtpService();
            _notification = new Notification();
            _failedCounter = new FailedCounter();
            _logger = new NLogAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            if (_failedCounter.IsLocked(accountId))
            {
                throw new FailedTooManyTimesException(){AccountId = accountId};
            }

            var passwordFromDb = _profileDao.Password(accountId);
            var hashedPassword = _hash.ComputeHash(password);
            var currentOtp = _otpService.CurrentOtp(accountId);

            if (passwordFromDb == hashedPassword && currentOtp == otp)
            {
                _failedCounter.Reset(accountId);

                return true;
            }
            else
            {
                _failedCounter.Increase(accountId);
                LogFailedCount(accountId);
                _notification.Notify(accountId);

                return false;
            }
        }

        private void LogFailedCount(string accountId)
        {
            var failedCount = _failedCounter.FailedCount(accountId);
            _logger.Info($"accountId:{accountId} failed times:{failedCount}");
        }
    }
}