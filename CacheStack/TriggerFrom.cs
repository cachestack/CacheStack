using System;
using System.Linq;
using System.Linq.Expressions;

namespace CacheStack
{
	public class TriggerFrom
	{
		/// <summary>
		/// Watches when any object of the specified type is updated
		/// </summary>
		/// <typeparam name="T">Type of object to watch for updates</typeparam>
		/// <returns></returns>
		public static ICacheTriggerWatcher Any<T>()
		{
			return Name(typeof(T).FullName + CacheContext.AnySuffix);
		}

		/// <summary>
		/// Watches when an object of the specified type, with the specified id is updated
		/// </summary>
		/// <typeparam name="T">Type of object to watch for updates</typeparam>
		/// <param name="id">Specific object id to watch for updates</param>
		/// <returns></returns>
		public static ICacheTriggerWatcher Id<T>(object id)
		{
			return Name(typeof(T).FullName + CacheContext.IdSuffix + id);
		}

		/// <summary>
		/// Watches when a property, that references another table, changes
		/// </summary>
		/// <typeparam name="T">Type of object to watch for updates</typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ICacheTriggerWatcher Relation<T, TValue>(Expression<Func<T, TValue>> property, object value)
		{
			var member = property.Body as MemberExpression ?? ((UnaryExpression) property.Body).Operand as MemberExpression;

			if (member == null)
				throw new Exception("Unable to get property name. Type: " + typeof(T).FullName + " Property: " + property.Name);

			var name = typeof(T).FullName + "__" + member.Member.Name;
			return Name(name + CacheContext.IdSuffix + value);
		}

		/// <summary>
		/// Watches when a specific cache key is triggered
		/// </summary>
		/// <param name="name">Cache key to watch</param>
		/// <returns></returns>
		public static ICacheTriggerWatcher Name(string name)
		{
			return new CacheTriggerWatcher
				       {
					       Name = name
				       };
		}
	}
}