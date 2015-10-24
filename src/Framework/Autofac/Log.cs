using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Caliburn.Micro;
using NLog;
using Parameter = Autofac.Core.Parameter;

namespace Kogler.Framework.Autofac
{
    public static class Log
    {
        public static Func<IComponentContext, IEnumerable<Parameter>, ILog> ILogRegistration => (c, p) => Caliburn.Micro.LogManager.GetLog(GetType(p));
        public static Func<IComponentContext, IEnumerable<Parameter>, ILogger> ILoggerRegistration => (c, p) => global::NLog.LogManager.GetLogger(GetType(p)?.FullName);

        public static Type GetType(IEnumerable<Parameter> parameters)
        {
            return parameters.OfType<TypedParameter>().Where(w => w.Type == typeof(Type)).Select(w => (Type)w.Value).FirstOrDefault();
        }

        public static void PreparingILog(object sender, PreparingEventArgs args)
        {
            var forType = args.Component.Activator.LimitType;
            var typeParam = TypedParameter.From(forType);

            var logParameter = new ResolvedParameter(
                (p, c) => p.ParameterType == typeof(ILog),
                (p, c) => c.Resolve<ILog>(typeParam));

            args.Parameters = args.Parameters.Union(new Parameter[] { typeParam, logParameter });
        }

        public static void PreparingILogger(object sender, PreparingEventArgs args)
        {
            var forType = args.Component.Activator.LimitType;
            var typeParam = TypedParameter.From(forType);

            var loggerParameter = new ResolvedParameter(
                (p, c) => p.ParameterType == typeof(ILogger),
                (p, c) => c.Resolve<ILogger>(TypedParameter.From(forType)));

            args.Parameters = args.Parameters.Union(new Parameter[] { typeParam, loggerParameter });
        }
    }
}