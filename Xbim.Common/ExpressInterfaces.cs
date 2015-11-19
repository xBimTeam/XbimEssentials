using System;
using System.Collections;
using System.Collections.Generic;

namespace Xbim.Common
{
	public interface IExpressValueType : IPersist
    {
        Type UnderlyingSystemType { get; }
        object Value { get; }
    }

	public interface IExpressSelectType : IPersist
    {
    }

	public interface IExpressHeaderType
    {
    }

	public interface IExpressEnumerable : IEnumerable
    {
    }

	public interface IExpressComplexType : IExpressValueType
    {
        IEnumerable<object> Properties { get; }
    }
}