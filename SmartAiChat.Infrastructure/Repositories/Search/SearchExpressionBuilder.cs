using System.Linq.Expressions;

namespace SmartAiChat.Infrastructure.Repositories.Search
{
    public static class SearchExpressionBuilder
    {
        public static Expression<Func<T, bool>> Build<T>(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return entity => true;
            }

            var parameter = Expression.Parameter(typeof(T), "entity");
            Expression? combinedExpression = null;

            var properties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                var propertyExpression = Expression.Property(parameter, property);
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var toLowerExpression = Expression.Call(propertyExpression, toLowerMethod);

                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchTermExpression = Expression.Constant(searchTerm.ToLower());
                var containsExpression = Expression.Call(toLowerExpression, containsMethod, searchTermExpression);

                if (combinedExpression == null)
                {
                    combinedExpression = containsExpression;
                }
                else
                {
                    combinedExpression = Expression.OrElse(combinedExpression, containsExpression);
                }
            }

            if (combinedExpression == null)
            {
                return entity => true;
            }

            return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        }
    }
}
