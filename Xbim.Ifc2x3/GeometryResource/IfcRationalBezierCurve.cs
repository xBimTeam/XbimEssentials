#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRationalBezierCurve.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class IfcRationalBezierCurve : IfcBezierCurve
    {
        public IfcRationalBezierCurve()
        {
                _weightsData = new XbimList<double>(this);
        }

        private XbimList<double> _weightsData;

        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public XbimList<double> WeightsData
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _weightsData;
            }
            set { this.SetModelValue(this, ref _weightsData, value, v => WeightsData = v, "WeightsData"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _weightsData.Add(value.RealVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value);
                    break;
            }
        }
    }
}