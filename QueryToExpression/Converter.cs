using QueryToExpression.Extensions;
using System.Linq.Expressions;

namespace QueryToExpression
{
	public class Converter<TEntity>
		where TEntity : class, new()
	{
		public static async Task<Expression<Func<TEntity,bool>>> SearchExpressionConverter (string searchText, params string[] removeColums)
		{
			Expression<Func<TEntity, bool>> result;

			if (searchText != null && searchText.Trim().Length > 0)
			{
				try
				{
					result = await searchText.SearchTextConverter<TEntity>(removeColums);
				}
				catch (Exception)
				{

					throw new NullReferenceException(nameof(result)+" object is equals to null!");
				}
			}
			else
				throw new ArgumentNullException(paramName: nameof(searchText));

			return result;
		}

		public static async Task<Expression<Func<TEntity,object>>> OrderExpressionConverter(string orderPropertyName)
		{
			Expression<Func<TEntity, object>> result;

			if( orderPropertyName != null && orderPropertyName.Trim().Length > 0)
			{
				try
				{
					result = await typeof(TEntity).GetProperties().GetProperty(orderPropertyName).SearchProperty<TEntity>();
				}
				catch (Exception)
				{

					throw new NullReferenceException(nameof(result) + " object is equals to null!");
				}
			}
			else
				throw new ArgumentNullException(paramName: nameof(orderPropertyName));

			return result;
		}
	}
}
