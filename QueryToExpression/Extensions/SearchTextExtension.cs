using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Linq.Expressions;
using System.Reflection;


namespace QueryToExpression.Extensions
{
	internal static class SearchTextExtension
	{
		internal static async Task<Expression<Func<TEntity,bool>>> SearchTextConverter<TEntity>(this string searchText)
			where TEntity : class
		{
			return await searchText.SearchTextConverter<TEntity>(null);
		}

		internal static async Task<Expression<Func<TEntity, bool>>> SearchTextConverter<TEntity>(this string searchText, params string[]? removeColums)
			where TEntity : class
		{
			if (searchText == null || searchText.Length <= 0 || searchText.Trim().Length <= 0)
				throw new ArgumentNullException(nameof(searchText));

			Type type = typeof(TEntity);
			IEnumerable<PropertyInfo> properties = type.GetRuntimeProperties();

			properties = RemovedColums(properties, removeColums);

			string expressionString = SearchTextConvertExpression(properties, searchText);

			Expression<Func<TEntity, bool>>? result;

			result = await expressionString.ConvertToExpressionFilterAsync<TEntity>();

			if (result == null)
				throw new ArgumentNullException(nameof(result));

			return result;
		}

		private static IEnumerable<PropertyInfo> RemovedColums(IEnumerable<PropertyInfo> properties, string[]? removeColums)
		{
			if (removeColums != null && removeColums.Length > 0)
			{
				foreach (var removeColum in removeColums)
				{
					if (properties.Count() > 0)
						properties = properties.Where(P => P.Name.ToLower() != removeColum.ToLower());
				}
			}
			return properties;
		}

		private static string SearchTextConvertExpression(IEnumerable<PropertyInfo> properties, string searchText)
		{
			string? result;
			if (properties != null && properties.Count() > 0)
			{
				result = "p => ";
				char karakter = '"';
				foreach (var property in properties)
				{
					if (property.PropertyType == typeof(string))
						result += "p." + property.Name + ".ToLower().TrimStart().TrimEnd().Contains(" + karakter + searchText.TrimStart().TrimEnd().ToLower() + karakter + ") ||";
					else
						result += "p." + property.Name + ".ToString().ToLower().TrimStart().TrimEnd().Contains(" + karakter + searchText.TrimStart().TrimEnd().ToLower() + karakter + ") ||";
				}
				result = result.Substring(0, result.Length - 2); // en son daki ya da karakterini kaldırılıyor.
			}
			else
				throw new ArgumentNullException(nameof(properties));

			return result!;
		}

		public async static Task<Expression<Func<T, bool>>?> ConvertToExpressionFilterAsync<T>(this string filterText)
			where T : class
		{
			if (filterText == null || filterText.Trim().Length <= 0)
				throw new ArgumentNullException(nameof(filterText));
			Expression<Func<T, bool>>? result;

			var opt = ScriptOptions.Default.AddReferences(typeof(T).Assembly);
			result = await CSharpScript.EvaluateAsync<Expression<Func<T, bool>>?>(filterText, opt);

			return result;
		}
	}
}
