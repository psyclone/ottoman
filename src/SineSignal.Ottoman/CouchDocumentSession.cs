using System;
using System.Collections.Generic;
using System.Reflection;

using SineSignal.Ottoman.Exceptions;

namespace SineSignal.Ottoman
{
	public class CouchDocumentSession
	{
		public IDocumentConvention DocumentConvention { get; private set; }
		private Dictionary<string, object> IdentityMap { get; set; }
		
		public CouchDocumentSession(IDocumentConvention documentConvention)
		{
			DocumentConvention = documentConvention;
			IdentityMap = new Dictionary<string, object>();
		}
		
		public void Store(object entity)
		{
			Type entityType = entity.GetType();
			PropertyInfo identityProperty = DocumentConvention.GetIdentityPropertyFor(entityType);
			
			object id = null;
			if (identityProperty != null)
			{
				id = GetIdentityValueFor(entity, identityProperty);
				
				if (id == null)
				{
					id = DocumentConvention.GenerateIdentityFor(identityProperty.PropertyType);
					identityProperty.SetValue(entity, id, null);
				}
			}
			
			if (id != null)
			{
				if (IdentityMap.ContainsKey(id.ToString()))
				{
					if (ReferenceEquals(IdentityMap[id.ToString()], entity))
						return;
					
					throw new NonUniqueEntityException("Attempted to associate a different entity with id '" + id + "'.");
				}
				
				IdentityMap[id.ToString()] = entity;
			}
		}
		
		public T Load<T>(string id)
		{
			object existingEntity;
		    if(IdentityMap.TryGetValue(id, out existingEntity))
		    {
		        return (T)existingEntity;
		    }
			
			return default(T);
		}
		
		private static object GetIdentityValueFor(object entity, PropertyInfo identityProperty)
		{
			object id = identityProperty.GetValue(entity, null);
			
			Type propertyType = identityProperty.PropertyType;
			if (propertyType == typeof(Guid))
			{
				if ((Guid)id == Guid.Empty)
					id = null;
			}
			
			return id;
		}
	}
}
