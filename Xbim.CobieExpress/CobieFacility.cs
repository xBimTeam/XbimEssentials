// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.CobieExpress.Interfaces;
using Xbim.CobieExpress;
//## Custom using statements
//##

namespace Xbim.CobieExpress.Interfaces
{
	/// <summary>
    /// Readonly interface for CobieFacility
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @ICobieFacility : ICobieAsset, SpatialDivision
	{
		ICobieLinearUnit @LinearUnits { get; }
		ICobieAreaUnit @AreaUnits { get; }
		ICobieVolumeUnit @VolumeUnits { get; }
		ICobieCurrencyUnit @CurrencyUnit { get; }
		string @AreaMeasurement { get; }
		ICobieProject @Project { get; }
		ICobieSite @Site { get; }
		ICobiePhase @Phase { get; }
		IEnumerable<ICobieFloor> @Floors {  get; }
		IEnumerable<ICobieSystem> @Systems {  get; }
	
	}
}

namespace Xbim.CobieExpress
{
	[ExpressType("Facility", 14)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @CobieFacility : CobieAsset, IInstantiableEntity, ICobieFacility, IContainsEntityReferences, IContainsIndexedReferences, IEquatable<@CobieFacility>
	{
		#region ICobieFacility explicit implementation
		ICobieLinearUnit ICobieFacility.LinearUnits { get { return @LinearUnits; } }	
		ICobieAreaUnit ICobieFacility.AreaUnits { get { return @AreaUnits; } }	
		ICobieVolumeUnit ICobieFacility.VolumeUnits { get { return @VolumeUnits; } }	
		ICobieCurrencyUnit ICobieFacility.CurrencyUnit { get { return @CurrencyUnit; } }	
		string ICobieFacility.AreaMeasurement { get { return @AreaMeasurement; } }	
		ICobieProject ICobieFacility.Project { get { return @Project; } }	
		ICobieSite ICobieFacility.Site { get { return @Site; } }	
		ICobiePhase ICobieFacility.Phase { get { return @Phase; } }	
		 
		IEnumerable<ICobieFloor> ICobieFacility.Floors {  get { return @Floors; } }
		IEnumerable<ICobieSystem> ICobieFacility.Systems {  get { return @Systems; } }
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal CobieFacility(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private CobieLinearUnit _linearUnits;
		private CobieAreaUnit _areaUnits;
		private CobieVolumeUnit _volumeUnits;
		private CobieCurrencyUnit _currencyUnit;
		private string _areaMeasurement;
		private CobieProject _project;
		private CobieSite _site;
		private CobiePhase _phase;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(13, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 15)]
		public CobieLinearUnit @LinearUnits 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _linearUnits;
				((IPersistEntity)this).Activate(false);
				return _linearUnits;
			} 
			set
			{
				SetValue( v =>  _linearUnits = v, _linearUnits, value,  "LinearUnits", 13);
			} 
		}	
		[EntityAttribute(14, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 16)]
		public CobieAreaUnit @AreaUnits 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _areaUnits;
				((IPersistEntity)this).Activate(false);
				return _areaUnits;
			} 
			set
			{
				SetValue( v =>  _areaUnits = v, _areaUnits, value,  "AreaUnits", 14);
			} 
		}	
		[EntityAttribute(15, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 17)]
		public CobieVolumeUnit @VolumeUnits 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _volumeUnits;
				((IPersistEntity)this).Activate(false);
				return _volumeUnits;
			} 
			set
			{
				SetValue( v =>  _volumeUnits = v, _volumeUnits, value,  "VolumeUnits", 15);
			} 
		}	
		[EntityAttribute(16, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 18)]
		public CobieCurrencyUnit @CurrencyUnit 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _currencyUnit;
				((IPersistEntity)this).Activate(false);
				return _currencyUnit;
			} 
			set
			{
				SetValue( v =>  _currencyUnit = v, _currencyUnit, value,  "CurrencyUnit", 16);
			} 
		}	
		[EntityAttribute(17, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 19)]
		public string @AreaMeasurement 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _areaMeasurement;
				((IPersistEntity)this).Activate(false);
				return _areaMeasurement;
			} 
			set
			{
				SetValue( v =>  _areaMeasurement = v, _areaMeasurement, value,  "AreaMeasurement", 17);
			} 
		}	
		[IndexedProperty]
		[EntityAttribute(18, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 20)]
		public CobieProject @Project 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _project;
				((IPersistEntity)this).Activate(false);
				return _project;
			} 
			set
			{
				SetValue( v =>  _project = v, _project, value,  "Project", 18);
			} 
		}	
		[IndexedProperty]
		[EntityAttribute(19, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 21)]
		public CobieSite @Site 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _site;
				((IPersistEntity)this).Activate(false);
				return _site;
			} 
			set
			{
				SetValue( v =>  _site = v, _site, value,  "Site", 19);
			} 
		}	
		[EntityAttribute(20, EntityAttributeState.Optional, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 22)]
		public CobiePhase @Phase 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _phase;
				((IPersistEntity)this).Activate(false);
				return _phase;
			} 
			set
			{
				SetValue( v =>  _phase = v, _phase, value,  "Phase", 20);
			} 
		}	
		#endregion



		#region Inverse attributes
		[InverseProperty("Facility")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 1, -1, 23)]
		public IEnumerable<CobieFloor> @Floors 
		{ 
			get 
			{
				return Model.Instances.Where<CobieFloor>(e => Equals(e.Facility), "Facility", this);
			} 
		}
		[InverseProperty("Facility")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, -1, -1, 24)]
		public IEnumerable<CobieSystem> @Systems 
		{ 
			get 
			{
				return Model.Instances.Where<CobieSystem>(e => Equals(e.Facility), "Facility", this);
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
				case 7: 
				case 8: 
				case 9: 
				case 10: 
				case 11: 
					base.Parse(propIndex, value, nestedIndex); 
					return;
				case 12: 
					_linearUnits = (CobieLinearUnit)(value.EntityVal);
					return;
				case 13: 
					_areaUnits = (CobieAreaUnit)(value.EntityVal);
					return;
				case 14: 
					_volumeUnits = (CobieVolumeUnit)(value.EntityVal);
					return;
				case 15: 
					_currencyUnit = (CobieCurrencyUnit)(value.EntityVal);
					return;
				case 16: 
					_areaMeasurement = value.StringVal;
					return;
				case 17: 
					_project = (CobieProject)(value.EntityVal);
					return;
				case 18: 
					_site = (CobieSite)(value.EntityVal);
					return;
				case 19: 
					_phase = (CobiePhase)(value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@CobieFacility other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @CobieFacility
            var root = (@CobieFacility)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@CobieFacility left, @CobieFacility right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@CobieFacility left, @CobieFacility right)
        {
            return !(left == right);
        }

        public static bool operator ==(@CobieFacility left, SpatialDivision right)
		{
			return left == right as @CobieFacility;
		}

		public static bool operator !=(@CobieFacility left, SpatialDivision right)
		{
			return !(left == right);
		}

        #endregion

		#region IContainsEntityReferences
		IEnumerable<IPersistEntity> IContainsEntityReferences.References 
		{
			get 
			{
				if (@Created != null)
					yield return @Created;
				if (@ExternalSystem != null)
					yield return @ExternalSystem;
				if (@ExternalObject != null)
					yield return @ExternalObject;
				foreach(var entity in @Categories)
					yield return entity;
				foreach(var entity in @Impacts)
					yield return entity;
				foreach(var entity in @Documents)
					yield return entity;
				foreach(var entity in @Attributes)
					yield return entity;
				foreach(var entity in @Representations)
					yield return entity;
				if (@LinearUnits != null)
					yield return @LinearUnits;
				if (@AreaUnits != null)
					yield return @AreaUnits;
				if (@VolumeUnits != null)
					yield return @VolumeUnits;
				if (@CurrencyUnit != null)
					yield return @CurrencyUnit;
				if (@Project != null)
					yield return @Project;
				if (@Site != null)
					yield return @Site;
				if (@Phase != null)
					yield return @Phase;
			}
		}
		#endregion


		#region IContainsIndexedReferences
        IEnumerable<IPersistEntity> IContainsIndexedReferences.IndexedReferences 
		{ 
			get
			{
				if (@Project != null)
					yield return @Project;
				if (@Site != null)
					yield return @Site;
				
			} 
		}
		#endregion

		#region Custom code (will survive code regeneration)
		//## Custom code
		//##
		#endregion
	}
}