using System;
using System.Collections.Generic;

namespace Xbim.Common
{
	public interface IEntityFactory 
	{
		T New<T>(IModel model, int entityLabel, bool activated) where T: IInstantiableEntity;

		T New<T>(IModel model, Action<T> init, int entityLabel, bool activated) where T: IInstantiableEntity;

		IInstantiableEntity New(IModel model, Type t, int entityLabel, bool activated);

		IInstantiableEntity New(IModel model, string typeName, int entityLabel, bool activated);
		
		IInstantiableEntity New(IModel model, int typeId, int entityLabel, bool activated);

		IExpressValueType New(string typeName);

		IEnumerable<string> SchemasIds { get; }
	}
}