using System;
using System.Reflection;
using Caliburn.Micro;
using Castle.DynamicProxy;
using NLog;

namespace Kogler.Framework.NLog
{
    public class NLogProxy
    {
        static NLogProxy()
        {
            generator = new ProxyGenerator();
            interceptor = new LogInterceptor();
            hook = new LoggerProxyHook();
            interfacesToProxy = new[] { typeof(ILog) };
            proxyType = typeof(Logger);
        }

        private static readonly ProxyGenerator generator;
        private static readonly LogInterceptor interceptor;
        private static readonly LoggerProxyHook hook;
        private static readonly Type[] interfacesToProxy;
        private static readonly Type proxyType;

        private class LogInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                switch (invocation.Method.Name)
                {
                    case "Error":
                    {
                        ((Logger) invocation.Proxy).Error((Exception) invocation.Arguments[0]);
                        break;
                    }
                }
            }
        }

        private class LoggerProxyHook : IProxyGenerationHook
        {
            public void MethodsInspected() { }

            public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo) { }

            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return methodInfo.DeclaringType == typeof (ILog);
            }

            public override bool Equals(object obj)
            {
                return GetType() == obj.GetType();
            }

            public override int GetHashCode()
            {
                return GetType().GetHashCode();
            }
        }

        public static Func<Type, Logger> GetLogger => type => global::NLog.LogManager.GetLogger(type.Name);

        public static Func<Type, ILog> GetCaliburnMicroILogFromType => type => GetCaliburnMicroILogFromLogger(GetLogger(type));

        public static Func<Logger, ILog> GetCaliburnMicroILogFromLogger => logger =>
        {
            var options = new ProxyGenerationOptions(hook);
            options.AddMixinInstance(logger);
            var proxy = (ILog)generator.CreateClassProxy(proxyType, interfacesToProxy, options, interceptor);
            var initMethod = typeof(Logger).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic);
            var configMethod = typeof(LogFactory).GetMethod("GetConfigurationForLogger", BindingFlags.Instance | BindingFlags.NonPublic);
            var config = configMethod.Invoke(logger.Factory, new object[] {logger.Name, logger.Factory.Configuration});
            initMethod.Invoke(proxy, new object[] {logger.Name, config, logger.Factory});
            return proxy;
        };
    }
}