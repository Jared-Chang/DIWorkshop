using System;
using System.Linq;
using Castle.DynamicProxy;
using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop
{
    public class AuditLogInterceptor : IInterceptor
    {
        private readonly ILogger _logger;
        private readonly IContext _context;

        public AuditLogInterceptor(ILogger logger, IContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!(Attribute.GetCustomAttribute(invocation.Method, typeof(AuditLogAttribute)) is AuditLogAttribute))
            {
                invocation.Proceed();
            }
            else
            {
                var currentUser = _context.GetUser();
                var parameters = string.Join("|", invocation.Arguments.Select(p => p.ToString()));
                _logger.Info($"[Audit] user:{currentUser.Name} invoke with parameter {parameters}");

                invocation.Proceed();

                var returnValue = invocation.ReturnValue;
                _logger.Info($"[Audit] Return value:{returnValue}");
            }
        }
    }
}