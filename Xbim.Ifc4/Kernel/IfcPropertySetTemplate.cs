// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.Kernel;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcPropertySetTemplate
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcPropertySetTemplate : IIfcPropertyTemplateDefinition
	{
		IfcPropertySetTemplateTypeEnum? @TemplateType { get; }
		IfcIdentifier? @ApplicableEntity { get; }
		IEnumerable<IIfcPropertyTemplate> @HasPropertyTemplates { get; }
		IEnumerable<IIfcRelDefinesByTemplate> @Defines {  get; }
	
	}
}

namespace Xbim.Ifc4.Kernel
{
	[ExpressType("IfcPropertySetTemplate", 1232)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcPropertySetTemplate : IfcPropertyTemplateDefinition, IInstantiableEntity, IIfcPropertySetTemplate, IEquatable<@IfcPropertySetTemplate>
	{
		#region IIfcPropertySetTemplate explicit implementation
		IfcPropertySetTemplateTypeEnum? IIfcPropertySetTemplate.TemplateType { get { return @TemplateType; } }	
		IfcIdentifier? IIfcPropertySetTemplate.ApplicableEntity { get { return @ApplicableEntity; } }	
		IEnumerable<IIfcPropertyTemplate> IIfcPropertySetTemplate.HasPropertyTemplates { get { return @HasPropertyTemplates; } }	
		 
		IEnumerable<IIfcRelDefinesByTemplate> IIfcPropertySetTemplate.Defines {  get { return @Defines; } }
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcPropertySetTemplate(IModel model) : base(model) 		{ 
			Model = model; 
			_hasPropertyTemplates = new ItemSet<IfcPropertyTemplate>( this, 0,  7);
		}

		#region Explicit attribute fields
		private IfcPropertySetTemplateTypeEnum? _templateType;
		private IfcIdentifier? _applicableEntity;
		private ItemSet<IfcPropertyTemplate> _hasPropertyTemplates;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(5, EntityAttributeState.Optional, EntityAttributeType.Enum, EntityAttributeType.None, -1, -1, 7)]
		public IfcPropertySetTemplateTypeEnum? @TemplateType 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _templateType;
				((IPersistEntity)this).Activate(false);
				return _templateType;
			} 
			set
			{
				SetValue( v =>  _templateType = v, _templateType, value,  "TemplateType", 5);
			} 
		}	
		[EntityAttribute(6, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 8)]
		public IfcIdentifier? @ApplicableEntity 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _applicableEntity;
				((IPersistEntity)this).Activate(false);
				return _applicableEntity;
			} 
			set
			{
				SetValue( v =>  _applicableEntity = v, _applicableEntity, value,  "ApplicableEntity", 6);
			} 
		}	
		[IndexedProperty]
		[EntityAttribute(7, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 1, -1, 9)]
		public ItemSet<IfcPropertyTemplate> @HasPropertyTemplates 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _hasPropertyTemplates;
				((IPersistEntity)this).Activate(false);
				return _hasPropertyTemplates;
			} 
		}	
		#endregion



		#region Inverse attributes
		[InverseProperty("RelatingTemplate")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 10)]
		public IEnumerable<IfcRelDefinesByTemplate> @Defines 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelDefinesByTemplate>(e => Equals(e.RelatingTemplate), "RelatingTemplate", this);
			} 
		}
		#endregion


		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
				case 1: 
				case 2: 
				case 3: 
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 4: 
                    _templateType = (IfcPropertySetTemplateTypeEnum) System.Enum.Parse(typeof (IfcPropertySetTemplateTypeEnum), value.EnumVal, true);
					return;
				case 5: 
					_applicableEntity = value.StringVal;
					return;
				case 6: 
					_hasPropertyTemplates.InternalAdd((IfcPropertyTemplate)value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcPropertySetTemplate other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcPropertySetTemplate
            var root = (@IfcPropertySetTemplate)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcPropertySetTemplate left, @IfcPropertySetTemplate right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcPropertySetTemplate left, @IfcPropertySetTemplate right)
        {
            return !(left == right);
        }

        #endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}