using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kogler.Framework
{
    public static class ExpressionExtension
    {
        public static string ToPropertyName<TSource, TTarget>(this Expression<Func<TSource, TTarget>> property)
        {
            Expression expBody = property.Body;
            List<string> properties = new List<string>();
            while (expBody is MemberExpression)
            {
                MemberExpression expMember = expBody as MemberExpression;
                properties.Add(expMember.Member.Name);
                expBody = expMember.Expression;
            }
            return string.Join(".", properties);
        }

        public static string ToPropertyName<TProperty>(this Expression<Func<TProperty>> property)
        {
            MemberExpression memberExpression;
            if (property.Body is UnaryExpression) memberExpression = ((UnaryExpression)property.Body).Operand as MemberExpression;
            else memberExpression = property.Body as MemberExpression;
            return memberExpression?.Member.Name;
        }

        public static PropertyInfo ToPropertyInfo<TGeneric>(this Expression<TGeneric> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            return body?.Member as PropertyInfo;
        }

        public static PropertyInfo ToPropertyInfo(this Expression expression)
        {
            MemberExpression body = expression as MemberExpression;
            return body?.Member as PropertyInfo;
        }
    }
}