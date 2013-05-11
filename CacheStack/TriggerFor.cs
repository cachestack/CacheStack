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
		private static readonly Lazy<Dictionary<Type, IList<Type>>> ObjectReferences = new Lazy<Dictionary<Type, IList<Type>>>(() =>
			{
				var references = new Dictionary<Type, IList<Type>>();
				// Get a list of all ReferencesAttributes used in the model classes
				var classTypes = CacheStackSettings.TypesForObjectRelations;
				if (classTypes == null)
					return references;

				foreach (var classType in classTypes)
				{
					foreach (var property in classType.GetProperties())
					{
						// Dynamic trick to get Spruce or ormlite references attribute.
						var attribute = property.GetCustomAttributes(true).FirstOrDefault(attr => attr.GetType().Name == "ReferencesAttribute") as dynamic;
						if (attribute == null)
							continue;

						if (!references.ContainsKey(attribute.Type))
						{
							references.Add(attribute.Type, new List<Type>());
						}
						if (references[attribute.Type].Contains(classType))
							continue;
						references[attribute.Type].Add(classType);
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
				foreach (var classType in classesWithReferences)
				{
					triggers.Add(Relation(classType, type, value));
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
				throw new Exception("Specified property does not have a ReferencesAttribute applied. Type: " + typeof(T).FullName + " Property: " + property.Name);

			// Dynamic trick to get Spruce or ormlite references attribute.
			var referenceAttribute = member.Member.GetCustomAttributes(true).FirstOrDefault(attr => attr.GetType().Name == "ReferencesAttribute") as dynamic;
			if (referenceAttribute == null)
				throw new Exception("Specified property does not have a ReferencesAttribute applied. Type: " + typeof(T).FullName + ", Property: " + property.Name);

			return Relation(typeof(T), referenceAttribute.Type, value);
		}
		private static ICacheTrigger Relation(Type type, Type referencedType, object value)
		{
			var name = type.FullName + "__" + referencedType.FullName;
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