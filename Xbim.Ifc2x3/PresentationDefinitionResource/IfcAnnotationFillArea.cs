#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAnnotationFillArea.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    [IfcPersistedEntityAttribute]
    public class IfcAnnotationFillArea : IfcRepresentationItem
    {
        #region Fields

        private IfcCurve _outerBoundary;
        private CurveSet _innerBoundaries;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCurve OuterBoundary
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _outerBoundary;
            }
            set { this.SetModelValue(this, ref _outerBoundary, value, v => OuterBoundary = v, "OuterBoundary"); }
        }

        [IfcAttribute(2, IfcAttributeState.Optional, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public CurveSet InnerBoundaries
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _innerBoundaries;
            }
            set
            {
                this.SetModelValue(this, ref _innerBoundaries, value, v => InnerBoundaries = v,
                                           "InnerBoundaries");
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _outerBoundary = (IfcCurve) value.EntityVal;
                    break;
                case 1:
                    if (_innerBoundaries == null) _innerBoundaries = new CurveSet(this);
                    _innerBoundaries.Add((IfcCurve)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Methods

        public void AddInnerBoundary(IfcCurve inner)
        {
            if (_innerBoundaries == null)
                this.SetModelValue(this, ref _innerBoundaries, new CurveSet(this), v => _innerBoundaries = v,
                                           "InnerBoundaries");
            _innerBoundaries.Add(inner);
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}