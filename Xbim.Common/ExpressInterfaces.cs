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

    public interface IExpressBinaryType
    {
        byte[] Value { get; }
    }

    public interface IExpressBooleanType
    {
        bool Value { get; }
    }

    public interface IExpressIntegerType
    {
        long Value { get; }
    }

    public interface IExpressLogicalType
    {
        bool? Value { get; }
    }

    public interface IExpressNumberType
    {
        double Value { get; }
    }

    public interface IExpressRealType
    {
        double Value { get; }
    }

    public interface IExpressStringType
    {
        string Value { get; }
    }
}