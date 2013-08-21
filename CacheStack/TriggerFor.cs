using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CacheStack
{
	public class TriggerFor
	{
		// Key: Object type being updated
		// Value: List of class types with properties that reference the key
		// {
		//   user: { 
		//    project: ["createdBy","modifiedBy"],
		//    template: ["createdBy","modifiedBy"]
		//   }
		// }
		private static readonly Lazy<Dictionary<Type, IDictionary<Type, IList<string>>>> ObjectReferences = new Lazy<Dictionary<Type, IDictionary<Type, IList<string>>>>(() =>
			{
				var references = new Dictionary<Type, IDictionary<Type, IList<string>>>();
				// Get a list of all ReferencesAttributes used in the model classes
				var classTypes = typeof(TriggerFor).Assembly.GetTypes().Where(x => x.IsPublic && !x.IsAbstract && x.IsClass && x.Namespace != null && x.Namespace.StartsWith("Quad.Core.Models."));
				foreach (var classType in classTypes)
				{
					var propertiesWithReferencesAttribute = classType.GetProperties().Where(prop => prop.IsDefined(typeof(ReferencesAttribute), true)).ToList();
					foreach (var property in propertiesWithReferencesAttribute)
					{
						var attribute = property.GetCustomAttributes(typeof(ReferencesAttribute), true).FirstOrDefault() as ReferencesAttribute;
						if (attribute == null)
							throw new Exception("Property does not have a ReferencesAttribute applied. Type: " + classType + ", Property: " + property.Name);
						if (!references.ContainsKey(attribute.Type))
						{
							references.Add(attribute.Type, new Dictionary<Type, IList<string>>());
						}

						if (!references[attribute.Type].ContainsKey(classType))
						{
							references[attribute.Type].Add(classType, new List<string>());
						}
						references[attribute.Type][classType].Add(property.Name);
					}
				}
				return references;
			});

		/// <summary>
		/// Triggers for the object's id and other object types with a relation on this object type and this object's value.  
		/// WARNING: This is a big cache clearing hammer, use wisely.  
		/// For example, calling this on a user object will clear the cache from every object that references that user, unnecessarily for most of those objects
		/// </summary>
		/// <typeparam name="T">Type of the object</typeparam>
		/// <param name="item">Item to trigger cache invalidation for</param>
		/// <returns></returns>
		public static ICacheTrigger[] ObjectAndRelations<T>(T item)
		{
			var triggers = new List<ICacheTrigger>();

			var type = item.GetType();
			var idProperty = type.GetProperty("Id");
			if (idProperty == null)
				throw new Exception("Object does not have Id property. Type: " + type);

			var value = idProperty.GetValue(item);
			triggers.Add(Id(type, value));

			if (ObjectReferences.Value.ContainsKey(type))
			{
				var classesWithReferences = ObjectReferences.Value[type];
				foreach (var classReferences in classesWithReferences)
				{
					var classReference = classReferences.Key;
					foreach (var property in classReferences.Value)
					{
						triggers.Add(Relation(classReference, property, value));
					}
				}
			}
			return triggers.ToArray();
		}

		/// <summary>
		/// Triggers cache invalidation for the object with the specified id.  Also will trigger for anything listening for .Any&gt;T&lt;()
		/// </summary>
		/// <typeparam name="T">Type of object to invalidate cache for</typeparam>
		/// <param name="id">Id of the object that has changed</param>
		/// <returns></returns>
		public static ICacheTrigger Id<T>(object id)
		{
			return Id(typeof(T), id);
		}
		private static ICacheTrigger Id(Type type, object id)
		{
			var name = type.FullName;
			return new CacheTrigger
					   {
						   CacheKeyForAnyItem = name + CacheContext.AnySuffix,
						   CacheKeyForIndividualItem = name + CacheContext.IdSuffix + id
					   };
		}

		/// <summary>
		/// Triggers cache invalidation of the specified object type, based on the property and value of the property specified.
		/// </summary>
		/// <typeparam name="T">Type of object to invalidate cache for</typeparam>
		/// <typeparam name="TValue">Property to watch</typeparam>
		/// <param name="property">Property to watch</param>
		/// <param name="value">Value of property that has changed</param>
		/// <returns></returns>
		public static ICacheTrigger Relation<T, TValue>(Expression<Func<T, TValue>> property, object value)
		{
			var member = property.Body as MemberExpression ?? ((UnaryExpression) property.Body).Operand as MemberExpression;

			if (member == null)
				throw new Exception("Unable to get property name. Type: " + typeof(T).FullName + " Property: " + property.Name);

			return Relation(typeof(T), member.Member.Name, value);
		}

		private static ICacheTrigger Relation(Type type, string referencedColumnName, object value)
		{
			var name = type.FullName + "__" + referencedColumnName;
			return new CacheTrigger
					   {
						   CacheKeyForAnyItem = type.FullName + CacheContext.AnySuffix,
						   CacheKeyForIndividualItem = name + CacheContext.IdSuffix + value
					   };
		}

		/// <summary>
		/// Triggers cache invalidation for a specific cache key
		/// </summary>
		/// <param name="name">Key to invalidate from cache</param>
		/// <returns></returns>
		public static ICacheTrigger Name(string name)
		{
			return new CacheTrigger
					   {
						   CacheKeyForIndividualItem = name
					   };
		}
	}
}