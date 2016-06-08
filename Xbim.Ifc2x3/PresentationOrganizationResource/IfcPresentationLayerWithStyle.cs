// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc2x3.PresentationAppearanceResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.PresentationOrganizationResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcPresentationLayerWithStyle
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcPresentationLayerWithStyle : IIfcPresentationLayerAssignment
	{
		bool? @LayerOn { get;  set; }
		bool? @LayerFrozen { get;  set; }
		bool? @LayerBlocked { get;  set; }
		IEnumerable<IIfcPresentationStyleSelect> @LayerStyles { get; }
	
	}
}

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
	[ExpressType("IfcPresentationLayerWithStyle", 259)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcPresentationLayerWithStyle : IfcPresentationLayerAssignment, IInstantiableEntity, IIfcPresentationLayerWithStyle, IEquatable<@IfcPresentationLayerWithStyle>
	{
		#region IIfcPresentationLayerWithStyle explicit implementation
		bool? IIfcPresentationLayerWithStyle.LayerOn { 
			get { return @LayerOn; } 
 
			set { LayerOn = value;}
		}	
		bool? IIfcPresentationLayerWithStyle.LayerFrozen { 
			get { return @LayerFrozen; } 
 
			set { LayerFrozen = value;}
		}	
		bool? IIfcPresentationLayerWithStyle.LayerBlocked { 
			get { return @LayerBlocked; } 
 
			set { LayerBlocked = value;}
		}	
		IEnumerable<IIfcPresentationStyleSelect> IIfcPresentationLayerWithStyle.LayerStyles { 
			get { return @LayerStyles; } 
		}	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcPresentationLayerWithStyle(IModel model) : base(model) 		{ 
			Model = model; 
			_layerStyles = new ItemSet<IfcPresentationStyleSelect>( this, 0,  8);
		}

		#region Explicit attribute fields
		private bool? _layerOn;
		private bool? _layerFrozen;
		private bool? _layerBlocked;
		private ItemSet<IfcPresentationStyleSelect> _layerStyles;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(5, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public bool? @LayerOn 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _layerOn;
				((IPersistEntity)this).Activate(false);
				return _layerOn;
			} 
			set
			{
				SetValue( v =>  _layerOn = v, _layerOn, value,  "LayerOn", 5);
			} 
		}	
		[EntityAttribute(6, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 6)]
		public bool? @LayerFrozen 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _layerFrozen;
				((IPersistEntity)this).Activate(false);
				return _layerFrozen;
			} 
			set
			{
				SetValue( v =>  _layerFrozen = v, _layerFrozen, value,  "LayerFrozen", 6);
			} 
		}	
		[EntityAttribute(7, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 7)]
		public bool? @LayerBlocked 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _layerBlocked;
				((IPersistEntity)this).Activate(false);
				return _layerBlocked;
			} 
			set
			{
				SetValue( v =>  _layerBlocked = v, _layerBlocked, value,  "LayerBlocked", 7);
			} 
		}	
		[EntityAttribute(8, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 8)]
		public ItemSet<IfcPresentationStyleSelect> @LayerStyles 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _layerStyles;
				((IPersistEntity)this).Activate(false);
				return _layerStyles;
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
					_layerOn = value.BooleanVal;
					return;
				case 5: 
					_layerFrozen = value.BooleanVal;
					return;
				case 6: 
					_layerBlocked = value.BooleanVal;
					return;
				case 7: 
					_layerStyles.InternalAdd((IfcPresentationStyleSelect)value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcPresentationLayerWithStyle other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcPresentationLayerWithStyle
            var root = (@IfcPresentationLayerWithStyle)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcPresentationLayerWithStyle left, @IfcPresentationLayerWithStyle right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcPresentationLayerWithStyle left, @IfcPresentationLayerWithStyle right)
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