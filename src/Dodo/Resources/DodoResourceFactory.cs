using REST;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.Resources
{
	public abstract class DodoResourceFactory<TResult, TSchema>
		: ResourceFactory<TResult, TSchema>
		where TResult : class, IRESTResource
		where TSchema : ResourceSchemaBase
	{
	}
}
