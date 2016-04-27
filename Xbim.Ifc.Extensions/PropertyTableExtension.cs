#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    PropertyTableExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class PropertyTableExtension
    {
        public static Dictionary<IfcValue, IfcValue> GetAsDictionary(this IfcPropertyTableValue table)
        {
            var result = new Dictionary<IfcValue, IfcValue>();
            var definingValues = table.DefiningValues;
            var definedValues = table.DefinedValues;

            if (definedValues == null || definingValues == null) return result;
            var count = definingValues.Count;

            if (count != definedValues.Count)
                throw new Exception(
                    "Inconsistent properties table. Number of defined and defining values are different.");

            for (var i = 0; i < count; i++)
            {
                result.Add(definingValues[i], definedValues[i]);
            }
            return result;
        }
    }
}