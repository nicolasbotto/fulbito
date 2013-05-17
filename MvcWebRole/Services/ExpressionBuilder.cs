using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace MvcWebRole.Services
{
    public class ExpressionBuilder : ExpressionVisitor
    {
        private StringBuilder sb;

        public ExpressionBuilder()
        {
            sb = new StringBuilder();
        }

        public string ProcessExpression(Expression expr)
        {
            this.Visit(expr);
            return sb.ToString().Trim();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            this.Visit(node.Left);

            sb.AppendFormat("{0} ", GetOperation(node));

            this.Visit(node.Right);

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            sb.AppendFormat("{0} ", GetConstantValue(node.Value));
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // If it's a property get its name, else execute the expression to the value
            if (node.Member.MemberType == System.Reflection.MemberTypes.Property)
            {
                sb.AppendFormat("{0} ", node.Member.Name);
            }
            else if (node.Member.MemberType == System.Reflection.MemberTypes.Field)
            {
                var memberValue = Expression.Lambda(node).Compile().DynamicInvoke();
                sb.AppendFormat("{0} ", GetConstantValue(memberValue));
            }

            return node;
        }

        private static string GetConstantValue(object item)
        {
            if (item.GetType() == typeof(string))
            {
                return string.Format("'{0}'", item);
            }
            else
            {
                return item.ToString();
            }
        }

        private static string GetOperation(BinaryExpression type)
        {
            switch (type.NodeType)
            {
                case ExpressionType.LessThan:
                    return QueryComparisons.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return QueryComparisons.LessThanOrEqual;
                case ExpressionType.Equal:
                    return QueryComparisons.Equal;
                case ExpressionType.GreaterThan:
                    return QueryComparisons.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return QueryComparisons.GreaterThanOrEqual;
                case ExpressionType.NotEqual:
                    return QueryComparisons.NotEqual;
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                case ExpressionType.AndAssign:
                    return "and";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.OrAssign:
                    return "or";
                default:
                    return string.Empty;
            }
        }
    }
}