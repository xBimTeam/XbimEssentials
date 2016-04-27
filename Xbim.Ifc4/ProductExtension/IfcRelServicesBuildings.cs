// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProductExtension;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcRelServicesBuildings
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcRelServicesBuildings : IIfcRelConnects
	{
		IIfcSystem @RelatingSystem { get; }
		IEnumerable<IIfcSpatialElement> @RelatedBuildings { get; }
	
	}
}

namespace Xbim.Ifc4.ProductExtension
{
	[ExpressType("IfcRelServicesBuildings", 600)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcRelServicesBuildings : IfcRelConnects, IInstantiableEntity, IIfcRelServicesBuildings, IEquatable<@IfcRelServicesBuildings>
	{
		#region IIfcRelServicesBuildings explicit implementation
		IIfcSystem IIfcRelServicesBuildings.RelatingSystem { get { return @RelatingSystem; } }	
		IEnumerable<IIfcSpatialElement> IIfcRelServicesBuildings.RelatedBuildings { get { return @RelatedBuildings; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcRelServicesBuildings(IModel model) : base(model) 		{ 
			Model = model; 
			_relatedBuildings = new ItemSet<IfcSpatialElement>( this, 0,  6);
		}

		#region Explicit attribute fields
		private IfcSystem _relatingSystem;
		private ItemSet<IfcSpatialElement> _relatedBuildings;
		#endregion
	
		#region Explicit attribute properties
		[IndexedProperty]
		[EntityAttribute(5, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 5)]
		public IfcSystem @RelatingSystem 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _relatingSystem;
				((IPersistEntity)this).Activate(false);
				return _relatingSystem;
			} 
			set
			{
				SetValue( v =>  _relatingSystem = v, _relatingSystem, value,  "RelatingSystem", 5);
			} 
		}	
		[IndexedProperty]
		[EntityAttribute(6, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 1, -1, 6)]
		public ItemSet<IfcSpatialElement> @RelatedBuildings 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _relatedBuildings;
				((IPersistEntity)this).Activate(false);
				return _relatedBuildings;
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
					_relatingSystem = (IfcSystem)(value.EntityVal);
					return;
				case 5: 
					_relatedBuildings.InternalAdd((IfcSpatialElement)value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcRelServicesBuildings other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcRelServicesBuildings
            var root = (@IfcRelServicesBuildings)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcRelServicesBuildings left, @IfcRelServicesBuildings right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcRelServicesBuildings left, @IfcRelServicesBuildings right)
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