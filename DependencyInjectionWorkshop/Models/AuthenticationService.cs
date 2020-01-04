using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly ProfileDao _profileDao;
        private readonly Sha256Adapter _sha256Adapter;
        private readonly OtpService _otpService;
        private readonly SlackAdapter _slackAdapter;
        private readonly FailedCounter _failedCounter;
        private readonly NLogAdapter _nLogAdapter;

        public AuthenticationService()
        {
            _profileDao = new ProfileDao();
            _sha256Adapter = new Sha256Adapter();
            _otpService = new OtpService();
            _slackAdapter = new SlackAdapter();
            _failedCounter = new FailedCounter();
            _nLogAdapter = new NLogAdapter();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            if (_failedCounter.IsLocked(accountId))
            {
                throw new FailedTooManyTimesException(){AccountId = accountId};
            }

            var passwordFromDb = _profileDao.PasswordFromDb(accountId);
            var hashedPassword = _sha256Adapter.HashedPassword(password);
            var currentOtp = _otpService.CurrentOtp(accountId, otp);

            if (passwordFromDb == hashedPassword && currentOtp == otp)
            {
                _failedCounter.Reset(accountId);

                return true;
            }
            else
            {
                _failedCounter.Increase(accountId);
                LogFailedCount(accountId);
                _slackAdapter.Notify(accountId);

                return false;
            }
        }

        private void LogFailedCount(string accountId)
        {
            var failedCount = _failedCounter.FailedCount(accountId);
            _nLogAdapter.Info($"accountId:{accountId} failed times:{failedCount}");
        }
    }
}