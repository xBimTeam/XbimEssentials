// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.StructuralLoadResource;
using Xbim.Ifc4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.StructuralAnalysisDomain;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcStructuralActivity
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcStructuralActivity : IIfcProduct
	{
		IIfcStructuralLoad @AppliedLoad { get; }
		IfcGlobalOrLocalEnum @GlobalOrLocal { get; }
		IEnumerable<IIfcRelConnectsStructuralActivity> @AssignedToStructuralItem {  get; }
	
	}
}

namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	[ExpressType("IfcStructuralActivity", 41)]
	// ReSharper disable once PartialTypeWithSinglePart
	public abstract partial class @IfcStructuralActivity : IfcProduct, IIfcStructuralActivity, IEquatable<@IfcStructuralActivity>
	{
		#region IIfcStructuralActivity explicit implementation
		IIfcStructuralLoad IIfcStructuralActivity.AppliedLoad { get { return @AppliedLoad; } }	
		IfcGlobalOrLocalEnum IIfcStructuralActivity.GlobalOrLocal { get { return @GlobalOrLocal; } }	
		 
		IEnumerable<IIfcRelConnectsStructuralActivity> IIfcStructuralActivity.AssignedToStructuralItem {  get { return @AssignedToStructuralItem; } }
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcStructuralActivity(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcStructuralLoad _appliedLoad;
		private IfcGlobalOrLocalEnum _globalOrLocal;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(8, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 20)]
		public IfcStructuralLoad @AppliedLoad 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _appliedLoad;
				((IPersistEntity)this).Activate(false);
				return _appliedLoad;
			} 
			set
			{
				SetValue( v =>  _appliedLoad = v, _appliedLoad, value,  "AppliedLoad", 8);
			} 
		}	
		[EntityAttribute(9, EntityAttributeState.Mandatory, EntityAttributeType.Enum, EntityAttributeType.None, -1, -1, 21)]
		public IfcGlobalOrLocalEnum @GlobalOrLocal 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _globalOrLocal;
				((IPersistEntity)this).Activate(false);
				return _globalOrLocal;
			} 
			set
			{
				SetValue( v =>  _globalOrLocal = v, _globalOrLocal, value,  "GlobalOrLocal", 9);
			} 
		}	
		#endregion



		#region Inverse attributes
		[InverseProperty("RelatedStructuralActivity")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, 1, 22)]
		public IEnumerable<IfcRelConnectsStructuralActivity> @AssignedToStructuralItem 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelConnectsStructuralActivity>(e => Equals(e.RelatedStructuralActivity), "RelatedStructuralActivity", this);
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
				case 4: 
				case 5: 
				case 6: 
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 7: 
					_appliedLoad = (IfcStructuralLoad)(value.EntityVal);
					return;
				case 8: 
                    _globalOrLocal = (IfcGlobalOrLocalEnum) System.Enum.Parse(typeof (IfcGlobalOrLocalEnum), value.EnumVal, true);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcStructuralActivity other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcStructuralActivity
            var root = (@IfcStructuralActivity)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcStructuralActivity left, @IfcStructuralActivity right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcStructuralActivity left, @IfcStructuralActivity right)
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