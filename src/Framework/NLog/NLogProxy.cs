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
        }

        public static Func<Type, ILog> GetCaliburnMicroLog => type =>
        {
            var options = new ProxyGenerationOptions(hook);
            options.AddMixinInstance(global::NLog.LogManager.GetLogger(type.Name));
            var proxy = (ILog) generator.CreateClassProxy(proxyType, interfacesToProxy, options, interceptor);
            return proxy;
        };
    }
}