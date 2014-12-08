#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPreDefinedCurveFont.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcPreDefinedCurveFont : IfcPreDefinedItem, IfcCurveStyleFontSelect,
                                                   IfcCurveFontOrScaledCurveFontSelect
    {
    }
}