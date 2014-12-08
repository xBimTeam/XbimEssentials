#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsPathElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsPathElements : IfcRelConnectsElements
    {
        public IfcRelConnectsPathElements()
        {
            _relatingPriorities = new XbimList<long>(this);
            _relatedPriorities = new XbimList<long>(this);
        }

        #region Part 21 Step file Parse routines

        private XbimList<long> _relatingPriorities;
        private XbimList<long> _relatedPriorities;
        private IfcConnectionTypeEnum _relatedConnectionType = IfcConnectionTypeEnum.NOTDEFINED;
        private IfcConnectionTypeEnum _relatingConnectionType = IfcConnectionTypeEnum.NOTDEFINED;

        /// <summary>
        ///   Priorities for connection. It refers to the layers of the RelatingObject.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory, IfcAttributeType.List)]
        public XbimList<long> RelatingPriorities
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingPriorities;
            }
            set
            {
                this.SetModelValue(this, ref _relatingPriorities, value, v => RelatingPriorities = v,
                                           "RelatingPriorities");
            }
        }

        /// <summary>
        ///   Priorities for connection. It refers to the layers of the RelatedObject.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory, IfcAttributeType.List)]
        public XbimList<long> RelatedPriorities
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedPriorities;
            }
            set
            {
                this.SetModelValue(this, ref _relatedPriorities, value, v => RelatedPriorities = v,
                                           "RelatedPriorities");
            }
        }

        /// <summary>
        ///   Indication of the connection type in relation to the path of the RelatingObject.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcConnectionTypeEnum RelatedConnectionType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedConnectionType;
            }
            set
            {
                this.SetModelValue(this, ref _relatedConnectionType, value, v => RelatedConnectionType = v,
                                           "RelatedConnectionType");
            }
        }

        /// <summary>
        ///   Indication of the connection type in relation to the path of the RelatingObject.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Mandatory)]
        public IfcConnectionTypeEnum RelatingConnectionType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingConnectionType;
            }
            set
            {
                this.SetModelValue(this, ref _relatingConnectionType, value, v => RelatingConnectionType = v,
                                           "RelatingConnectionType");
            }
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
                case 5:
                case 6:
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _relatingPriorities.Add(value.IntegerVal);
                    break;
                case 8:
                    _relatedPriorities.Add(value.IntegerVal);
                    break;
                case 9:
                    _relatedConnectionType =
                        (IfcConnectionTypeEnum) Enum.Parse(typeof (IfcConnectionTypeEnum), value.EnumVal, true);
                    break;
                case 10:
                    _relatingConnectionType =
                        (IfcConnectionTypeEnum) Enum.Parse(typeof (IfcConnectionTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Derived. Number of layers of the RelatedObjectDerived. Number of layers of the RelatingObject.Number of layers of the RelatingObject..
        /// </summary>
        public int RelatedLayerCount
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///   Derived. Number of layers of the RelatingObject.
        /// </summary>
        public int RelatingLayerCount
        {
            get { throw new NotImplementedException(); }
        }
    }
}