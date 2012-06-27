using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace ComplexWpfChatClientExample.Core
{
    /// <summary>
    /// Staticka trida, ktera rozsiruje tridy o nove metody.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Konvertuje vyrad na <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="expression">Vyraz ke konvertrovani</param>
        /// <returns>Udaje o clenu</returns>
        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member;
        }
    }
}
