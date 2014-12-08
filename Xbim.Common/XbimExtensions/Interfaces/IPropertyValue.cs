#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IPropertyValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IPropertyValue
    {
        bool BooleanVal { get; }
        string EnumVal { get; }
        object EntityVal { get; }
        long HexadecimalVal { get; }
        long IntegerVal { get; }
        double NumberVal { get; }
        double RealVal { get; }
        string StringVal { get; }
        IfcParserType Type { get; }
    }
}