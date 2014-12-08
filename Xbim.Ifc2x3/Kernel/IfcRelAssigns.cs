#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssigns.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcRelAssigns : IfcRelationship
    {
        public IfcRelAssigns()
        {
            _relatedObjects = new ObjectDefinitionSet(this);
        }

        #region Fields

        private readonly ObjectDefinitionSet _relatedObjects;
        private IfcObjectType? _relatedObjectsType;

        #endregion

        #region Ifc Properties

        /// <summary>
        ///   Related objects, which are assigned to a single object. The type of the single (or relating) object is defined 
        ///   in the subtypes of IfcRelAssigns.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        [IndexedProperty]
        public ObjectDefinitionSet RelatedObjects
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedObjects;
            }
        }

        /// <summary>
        ///   Particular type of the assignment relationship. It can constrain the applicable object types, used within the role of RelatedObjects.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcObjectType? RelatedObjectsType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedObjectsType;
            }
            set
            {
                this.SetModelValue(this, ref _relatedObjectsType, value, v => RelatedObjectsType = v,
                                           "RelatedObjectsType");
            }
        }

        #endregion

        #region Part 21 Step file Parse routines

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _relatedObjects.Add((IfcObjectDefinition)value.EntityVal);
                    break;
                case 5:
                    _relatedObjectsType = (IfcObjectType) Enum.Parse(typeof (IfcObjectType), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (!_relatedObjectsType.HasValue) return "";
            int relCount = _relatedObjects.Count;
            string err =
                "WR1 RelAssigns: Assigned related objects must comply with the contraint given by the RelatedObjectsType.\n";
            switch (RelatedObjectsType.Value)
            {
                case IfcObjectType.Product:
                    return _relatedObjects.OfType<IfcProduct>().Count() == relCount ? "" : err;

                case IfcObjectType.Process:
                    return _relatedObjects.OfType<IfcProcess>().Count() == relCount ? "" : err;

                case IfcObjectType.Control:
                    return _relatedObjects.OfType<IfcControl>().Count() == relCount ? "" : err;

                case IfcObjectType.Resource:
                    return _relatedObjects.OfType<IfcResource>().Count() == relCount ? "" : err;

                case IfcObjectType.Actor:
                    return _relatedObjects.OfType<IfcActor>().Count() == relCount ? "" : err;

                case IfcObjectType.Group:
                    return _relatedObjects.OfType<IfcGroup>().Count() == relCount ? "" : err;

                case IfcObjectType.Project:
                    return _relatedObjects.OfType<IfcProject>().Count() == relCount ? "" : err;

                case IfcObjectType.NotDefined:
                    return "";
                default:
                    return "WR1 RelAssigns: Unknown ObjectType\n";
            }
        }

        #endregion

        #region Properties

        #endregion

        #region Add Methods

        /// <summary>
        ///   Adds a Related object if it's type obeys the constraint set by RelatedObjectType or no constraint has been set
        /// </summary>
        public bool AddRelatedObject(IfcObjectDefinition obj)
        {
            if (_relatedObjectsType.HasValue == false) //you can add it, no contraints applied
            {
                _relatedObjects.Add(obj);
                return true;
            }
            bool addIt = false;
            switch (_relatedObjectsType.Value)
            {
                case IfcObjectType.Product:
                    addIt = (obj is IfcProduct);
                    break;
                case IfcObjectType.Process:
                    addIt = (obj is IfcProcess);
                    break;
                case IfcObjectType.Control:
                    addIt = (obj is IfcControl);
                    break;
                case IfcObjectType.Resource:
                    addIt = (obj is IfcResource);
                    break;
                case IfcObjectType.Actor:
                    addIt = (obj is IfcActor);
                    break;
                case IfcObjectType.Group:
                    addIt = (obj is IfcGroup);
                    break;
                case IfcObjectType.Project:
                    addIt = (obj is IfcProject);
                    break;
                case IfcObjectType.NotDefined:
                    addIt = true;
                    break;
                default: //invalid constraint so add it
                    addIt = true;
                    break;
            }
            if (addIt) _relatedObjects.Add(obj);
            return addIt;
        }

        #endregion
    }
}