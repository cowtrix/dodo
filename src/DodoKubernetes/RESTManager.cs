using Common.Extensions;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Http;
using REST.Serializers;
using System;
using System.Collections.Generic;

namespace REST
{
	public delegate void RequestReceivedDelegate(HttpRequest request);
	public abstract class RESTManager
	{
		protected List<Route> Routes = new List<Route>();
		protected RequestReceivedDelegate OnMsgReceieved;
		private List<RESTHandler> m_handlers = new List<RESTHandler>();

		public RESTManager()
		{
			var customSerializers = ReflectionExtensions.GetChildClasses<ICustomBsonSerializer>();
			foreach(var customSer in customSerializers)
			{
				var newSerializer = Activator.CreateInstance(customSer) as IBsonSerializer;
				try
				{
					BsonSerializer.RegisterSerializer(newSerializer.ValueType, newSerializer);
				}
				catch { }	// TODO: catch these failures better, they happen when testing
			}

			var handlers = ReflectionExtensions.GetChildClasses<RESTHandler>();
			foreach (var handlerType in handlers)
			{
				var handler = Activator.CreateInstance(handlerType) as RESTHandler;
				m_handlers.Add(handler);
				handler.AddRoutes(Routes);
			}
		}
	}
}
