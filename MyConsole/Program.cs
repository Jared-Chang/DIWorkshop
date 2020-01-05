using System;
using Autofac;
using Autofac.Extras.DynamicProxy;
using DependencyInjectionWorkshop;
using DependencyInjectionWorkshop.Models;

namespace MyConsole
{
    internal class Program
    {
        private static IContainer _container;

        private static void Main(string[] args)
        {
            RegisterContainer();

            var authenticationService = _container.Resolve<IAuthenticationService>();

            Login("joey", "91");

            var isValid = authenticationService.Verify("joey", "abc", "wrong otp");

            Console.WriteLine($"result:{isValid}");
        }

        private static void Login(string name, string password)
        {
            var context = _container.Resolve<IContext>();
            context.SetUser(name);
        }

        private static void RegisterContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<FakeProfile>().As<IProfile>();
            containerBuilder.RegisterType<FakeLogger>().As<ILogger>();
            containerBuilder.RegisterType<FakeSlack>().As<INotification>();
            containerBuilder.RegisterType<FakeFailedCounter>().As<IFailedCounter>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(AuditLogInterceptor));

            containerBuilder.RegisterType<FakeHash>().As<IHash>();
            containerBuilder.RegisterType<FakeOtp>().As<IOtpService>();

            containerBuilder.RegisterType<FakeContext>().As<IContext>().SingleInstance();
            containerBuilder.RegisterType<AuditLogInterceptor>();

            containerBuilder.RegisterType<AuthenticationService>().As<IAuthenticationService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(AuditLogInterceptor));

            containerBuilder.RegisterDecorator<FailedCountDecorator, IAuthenticationService>();
            containerBuilder.RegisterDecorator<LogDecorator, IAuthenticationService>();
            containerBuilder.RegisterDecorator<NotificationDecorator, IAuthenticationService>();
            //containerBuilder.RegisterDecorator<LogMethodDecorator, IAuthenticationService>();
            //containerBuilder.RegisterDecorator<AuditLogDecorator, IAuthenticationService>();

            _container = containerBuilder.Build();
        }
    }

    public class FakeContext : IContext
    {
        private User _user;

        public User GetUser()
        {
            return _user;
        }

        public void SetUser(string name)
        {
            _user = new User {Name = name};
        }
    }

    internal class FakeLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"logger: {message}");
        }
    }

    internal class FakeSlack : INotification
    {
        public void Notify(string accountId, string message)
        {
            PushMessage($"{nameof(Notify)}, accountId:{accountId}, message:{message}");
        }

        public void PushMessage(string message)
        {
            Console.WriteLine(message);
        }
    }

    internal class FakeFailedCounter : IFailedCounter
    {
        public void Reset(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Reset)}({accountId})");
        }

        public void Increase(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Increase)}({accountId})");
        }

        public bool IsLocked(string accountId)
        {
            return IsAccountLocked(accountId);
        }

        public int FailedCount(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(FailedCount)}({accountId})");
            return 91;
        }

        public bool IsAccountLocked(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(IsAccountLocked)}({accountId})");
            return false;
        }
    }

    internal class FakeOtp : IOtpService
    {
        public string CurrentOtp(string accountId)
        {
            Console.WriteLine($"{nameof(FakeOtp)}.{nameof(CurrentOtp)}({accountId})");
            return "123456";
        }
    }

    internal class FakeHash : IHash
    {
        public string ComputeHash(string plainText)
        {
            Console.WriteLine($"{nameof(FakeHash)}.{nameof(ComputeHash)}({plainText})");
            return "my hashed password";
        }
    }

    internal class FakeProfile : IProfile
    {
        public string Password(string accountId)
        {
            Console.WriteLine($"{nameof(FakeProfile)}.{nameof(Password)}({accountId})");
            return "my hashed password";
        }
    }
}