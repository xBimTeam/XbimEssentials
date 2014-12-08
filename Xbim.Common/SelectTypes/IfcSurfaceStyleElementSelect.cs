#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceStyleElementSelect.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

using Xbim.XbimExtensions.Interfaces;
namespace Xbim.XbimExtensions.SelectTypes
{
    public interface IfcSurfaceStyleElementSelect : ExpressSelectType, IPersistIfcEntity, ISupportChangeNotification
    {
    }
}