using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Core;
using Caliburn.Micro;
using NLog;

namespace Kogler.Framework.Autofac
{
    public class LoggerModule : global::Autofac.Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Preparing += OnPreparing;
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        private void OnPreparing(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(
                new[]
                {
                    new ResolvedParameter(
                        (p, i) => p.ParameterType == typeof (ILog) || p.ParameterType == typeof (ILogger) || p.ParameterType == typeof(Logger),
                        (p, i) =>
                        {
                            var type = p.Member.DeclaringType ?? e.Component.Activator.LimitType;
                            var loggerNames = new List<string>(3) {type.Name};
                            foreach (var la in new[] {GetLA(type), GetLA(p)}.Where(la => la != null))
                            {
                                if (!la.StackNames) loggerNames.Clear();
                                loggerNames.Add(string.IsNullOrEmpty(la.Name) ? type.Name : la.Name);
                            }
                            return NLog.LogManager.GetLogger(string.Join("+", loggerNames), typeof (Logger));
                        })
                });
        }

        private LoggerAttribute GetLA(ParameterInfo param)
        {
            return param.GetCustomAttributes(typeof(LoggerAttribute), true).OfType<LoggerAttribute>().FirstOrDefault();
        }

        private LoggerAttribute GetLA(MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(LoggerAttribute), true).OfType<LoggerAttribute>().FirstOrDefault();
        }
    }
}