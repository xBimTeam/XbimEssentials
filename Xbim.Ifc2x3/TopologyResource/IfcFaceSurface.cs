// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometryResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.TopologyResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcFaceSurface
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcFaceSurface : IIfcFace, IfcSurfaceOrFaceSurface
	{
		IIfcSurface @FaceSurface { get;  set; }
		bool @SameSense { get;  set; }
	
	}
}

namespace Xbim.Ifc2x3.TopologyResource
{
	[ExpressType("IfcFaceSurface", 85)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcFaceSurface : IfcFace, IInstantiableEntity, IIfcFaceSurface, IContainsEntityReferences, IEquatable<@IfcFaceSurface>
	{
		#region IIfcFaceSurface explicit implementation
		IIfcSurface IIfcFaceSurface.FaceSurface { 
			get { return @FaceSurface; } 
 
 
			set { FaceSurface = value as IfcSurface;}
		}	
		bool IIfcFaceSurface.SameSense { 
			get { return @SameSense; } 
 
			set { SameSense = value;}
		}	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcFaceSurface(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcSurface _faceSurface;
		private bool _sameSense;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(2, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 4)]
		public IfcSurface @FaceSurface 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _faceSurface;
				((IPersistEntity)this).Activate(false);
				return _faceSurface;
			} 
			set
			{
				SetValue( v =>  _faceSurface = v, _faceSurface, value,  "FaceSurface", 2);
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public bool @SameSense 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _sameSense;
				((IPersistEntity)this).Activate(false);
				return _sameSense;
			} 
			set
			{
				SetValue( v =>  _sameSense = v, _sameSense, value,  "SameSense", 3);
			} 
		}	
		#endregion





		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 1: 
					_faceSurface = (IfcSurface)(value.EntityVal);
					return;
				case 2: 
					_sameSense = value.BooleanVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcFaceSurface other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcFaceSurface
            var root = (@IfcFaceSurface)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcFaceSurface left, @IfcFaceSurface right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcFaceSurface left, @IfcFaceSurface right)
        {
            return !(left == right);
        }

        public static bool operator ==(@IfcFaceSurface left, IfcSurfaceOrFaceSurface right)
		{
			return left == right as @IfcFaceSurface;
		}

		public static bool operator !=(@IfcFaceSurface left, IfcSurfaceOrFaceSurface right)
		{
			return !(left == right);
		}

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				foreach(var entity in @Bounds)
					yield return entity;
				if (@FaceSurface != null)
					yield return @FaceSurface;
			}
		}
		#endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}