#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProfileDef.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProfileResource
{
    /// <summary>
    ///   The IfcProfileDef is the supertype of all definitions of standard and arbitrary profiles within IFC.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcProfileDef is the supertype of all definitions of standard and arbitrary profiles within IFC. It is used to define a standard set of commonly used profiles by their parameters or by their explicit curve geometry. Those profile definitions are used within the geometry and geometric model resource to create either swept surfaces, swept area solids, or sectioned spines. 
    ///   The purpose of the profile definition within the swept surfaces or swept area solids is to define a uniform cross section being swept:
    ///   along a line (extrusion) using IfcSurfaceOfLinearExtrusion or IfcExtrudedAreaSolid 
    ///   along a circular arc (revolution) using IfcSurfaceOfRevolution or IfcRevolvedAreaSolid 
    ///   along a directrix lying on a reference surface using IfcSurfaceCurveSweptAreaSolid 
    ///   The purpose fo the profile definition within the sectioned spine is to define a varying cross sections at several positions along a spine curve. The subtype IfcDerivedProfileDef is particularly suited to provide the consecutive profiles to be based on transformations of the start profile and thus maintaining the identity of vertices and edges. 
    ///   NOTE: Subtypes of the IfcProfileDef contain parameterized profiles (as subtypes of IfcParameterizedProfileDef) which establish their own 2D position coordinate system, profiles given by explicit curve geometry (either open or closed profiles) and two special types for composite profiles and derived profiles, based on a 2D Cartesian transformation. 
    ///   Parameterized profiles are 2D primitives, which are used within the industry to describe cross sections by a description of its parameters. 
    ///   Arbitrary profiles are cross sections defined by an outer boundary as bounded curve, which may also include holes, defined by inner boundaries. 
    ///   Derived profiles, based on a transformation of a parent profile, are also part of the profile definitions available. 
    ///   In addition composite profiles can be defined, which include two or more profile definitions to define the resulting profile. 
    ///   An IfcProfileDef is treated as bounded area if it is used within swept area solids. In this case, the inside of the profile is part of the profile. The attribute ProfileType is set to AREA. An IfcProfileDef is treated as a curve if it is used within swept surfaces. In this case, the inside of the profile (if the curve is closed) is not part of the profile. The attribute ProfileType is set to CURVE. The optional attribute ProfileName can be used to designate a standard profile type as e.g. given in profile tables for steel profiles.
    ///   HISTORY: New class in IFC Release 1.5, the capabilities have been extended in IFC Release 2x. Profiles can now support swept surfaces and swept area solids with inner boundaries. It had been renamed from IfcAttDrivenProfileDef.
    ///   Illustration:
    ///   Position 
    ///   The IfcProfileDef is defined within the underlying coordinate system taht is defined by the swept surface or swept area solid that uses the profile definition. It is the xy plane of either: 
    ///   IfcSweptSurface.Position 
    ///   IfcSweptAreaSolid.Position 
    ///   or in case of sectioned spines the xy plane of each list member of IfcSectionedSpine.CrossSectionPositions 
    ///   Note: The parameterized profile definition defines a 2D position coordinate system, relative to the underlying coordinate system of the IfcProfileDef.
    ///  
    ///   Sweeping
    ///   In the later use of the IfcProfileDef within the swept surface or swept area solid,  e.g. the IfcExtrudedAreaSolid (here used as an example), the profile boundaries (here based on the 2D position coordinate system of IfcParameterizedProfileDef) are placed within the xy plane of the 3D position coordinate system of the swept surface or swept area solid. 
    ///   The profile is inserted into the underlying coordinate system either:
    ///   directly in case of using IfcArbitraryClosedProfileDef and IfcArbitraryOpenProfileDef, 
    ///   through an intermediate position coordinate system in case of using IfcParameterizedProfileDef. 
    ///   through an 2D Cartesian transformation operator (applied directly to the curve position when using arbitrary profile definitions, or applied to the position coordinate system when using parameterized profile definitions) in case of using IfcDerivedProfileDef. 
    ///   when using IfcCompositeProfileDef the insertion depends on the subtype of the included sub-profiles.
    ///  
    ///   Table: Use of parameterized profiles within the swept area solid 
    ///   Use cases: 
    ///   Results of the different usage of the ProfileType attribute are demonstrated here. The ProfileType defines whether the inside (the bounded area) is part of the profile definition (Area) or not (Curve). 
    ///   ProfileType = AREA 
    ///   ProfileType = CURVE  
    ///   Table: Resulting area or curve depending on ProfileType
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcProfileDef : IPersistIfcEntity, ISupportChangeNotification, INotifyPropertyChanged,
                                          INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcProfileDef root = (IfcProfileDef)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcProfileDef left, IfcProfileDef right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcProfileDef left, IfcProfileDef right)
        {
            return !(left == right);
        }
        #region IPersistIfcEntity Members

        private int _entityLabel;
		bool _activated;

        private IModel _model;

        public IModel ModelOf
        {
            get { return _model; }
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _activated=activated;
			_model = model;
            _entityLabel = entityLabel;
        }

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        public int EntityLabel
        {
            get { return _entityLabel; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock(this) { if (_model != null && !_activated) _activated = _model.Activate(this, false)>0;  }
            if (write) _model.Activate(this, write);
        }

        #endregion

        #region Fields

        private IfcProfileTypeEnum _profileType;
        private IfcLabel? _profileName;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Defines the type of geometry into which this profile definition shall be resolved, either a curve or a surface area. In case of curve the profile should be referenced by a swept surface, in case of area the profile should be referenced by a swept area solid.
        /// </summary>
      
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcProfileTypeEnum ProfileType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _profileType;
            }
            set { this.SetModelValue(this, ref _profileType, value, v => ProfileType = v, "ProfileType"); }
        }


        /// <summary>
        ///   Optional. Name of the profile type according to some standard profile table.
        /// </summary>

        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcLabel? ProfileName
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _profileName;
            }
            set { this.SetModelValue(this, ref _profileName, value, v => ProfileName = v, "ProfileName"); }
        }

        public virtual void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _profileType = (IfcProfileTypeEnum) Enum.Parse(typeof (IfcProfileTypeEnum), value.EnumVal, true);
                    break;
                case 1:
                    _profileName = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        void ISupportChangeNotification.NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        [field: NonSerialized] //don't serialize events
            private event PropertyChangingEventHandler PropertyChanging;

        event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }

        #endregion

        #region ISupportChangeNotification Members

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public abstract string WhereRule();

        #endregion
    }
}