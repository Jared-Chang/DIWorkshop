using Autofac.Extras.DynamicProxy;

namespace DependencyInjectionWorkshop.Models
{
    public interface IAuthenticationService
    {
        bool Verify(string accountId, string password, string otp);
    }

    [Intercept(typeof(AuditLogInterceptor))]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHash _hash;
        private readonly IOtpService _otpService;
        private readonly IProfile _profileDao;

        public AuthenticationService(IProfile profileDao, IHash hash, IOtpService otpService)
        {
            _profileDao = profileDao;
            _hash = hash;
            _otpService = otpService;
        }

        [AuditLog]
        public bool Verify(string accountId, string password, string otp)
        {
            var passwordFromDb = _profileDao.Password(accountId);
            var hashedPassword = _hash.ComputeHash(password);
            var currentOtp = _otpService.CurrentOtp(accountId);

            return passwordFromDb == hashedPassword && currentOtp == otp;
        }
    }
}