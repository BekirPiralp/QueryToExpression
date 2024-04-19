using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryToExpression.Extensions
{
	internal static class PropertyInfoExtension
	{
		public static async Task<Expression<Func<TEntity,object>>> SearchProperty<TEntity>(this PropertyInfo property)
			where TEntity : class
		{
			if (property == null) 
				throw new ArgumentNullException(nameof(property));

			Expression<Func<TEntity, object>> result;

			ScriptOptions opt = ScriptOptions.Default.AddReferences(typeof(TEntity).Assembly);
			result = await CSharpScript.EvaluateAsync<Expression<Func<TEntity, object>>>($"p=>p.{property.Name}",options:opt);

			return result;
		}

		public static PropertyInfo GetProperty(this PropertyInfo[] properties, string orderString)
		{
			PropertyInfo result;
			PropertyInfo? reponse;

			reponse = (properties.Where(p => p.Name.ToLower() == orderString.ToLower().Trim())).FirstOrDefault();

			if (reponse == null)
				throw new NullReferenceException(nameof(PropertyInfo)+" "+nameof(reponse)+" ");

			result = reponse;

			return result;
		}
	}
}
