// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.PresentationOrganizationResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.MeasureResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Xbim.Common.Metadata;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ExternalReferenceResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcExternalReference
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcExternalReference : IPersistEntity, IfcLightDistributionDataSourceSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect
	{
		IfcURIReference? @Location { get; }
		IfcIdentifier? @Identification { get; }
		IfcLabel? @Name { get; }
		IEnumerable<IIfcExternalReferenceRelationship> @ExternalReferenceForResources {  get; }
	
	}
}

namespace Xbim.Ifc4.ExternalReferenceResource
{
	[ExpressType("IfcExternalReference", 133)]
	// ReSharper disable once PartialTypeWithSinglePart
	public abstract partial class @IfcExternalReference : IPersistEntity, INotifyPropertyChanged, IIfcExternalReference, IEquatable<@IfcExternalReference>
	{
		#region IIfcExternalReference explicit implementation
		IfcURIReference? IIfcExternalReference.Location { get { return @Location; } }	
		IfcIdentifier? IIfcExternalReference.Identification { get { return @Identification; } }	
		IfcLabel? IIfcExternalReference.Name { get { return @Name; } }	
		 
		IEnumerable<IIfcExternalReferenceRelationship> IIfcExternalReference.ExternalReferenceForResources {  get { return @ExternalReferenceForResources; } }
		#endregion

		#region Implementation of IPersistEntity

		public int EntityLabel {get; internal set;}
		
		public IModel Model { get; internal set; }

		/// <summary>
        /// This property is deprecated and likely to be removed. Use just 'Model' instead.
        /// </summary>
		[Obsolete("This property is deprecated and likely to be removed. Use just 'Model' instead.")]
        public IModel ModelOf { get { return Model; } }
		
	    internal ActivationStatus ActivationStatus = ActivationStatus.NotActivated;

	    ActivationStatus IPersistEntity.ActivationStatus { get { return ActivationStatus; } }
		
		void IPersistEntity.Activate(bool write)
		{
			switch (ActivationStatus)
		    {
		        case ActivationStatus.ActivatedReadWrite:
		            return;
		        case ActivationStatus.NotActivated:
		            lock (this)
		            {
                        //check again in the lock
		                if (ActivationStatus == ActivationStatus.NotActivated)
		                {
		                    if (Model.Activate(this, write))
		                    {
		                        ActivationStatus = write
		                            ? ActivationStatus.ActivatedReadWrite
		                            : ActivationStatus.ActivatedRead;
		                    }
		                }
		            }
		            break;
		        case ActivationStatus.ActivatedRead:
		            if (!write) return;
		            if (Model.Activate(this, true))
                        ActivationStatus = ActivationStatus.ActivatedReadWrite;
		            break;
		        default:
		            throw new ArgumentOutOfRangeException();
		    }
		}

		void IPersistEntity.Activate (Action activation)
		{
			if (ActivationStatus != ActivationStatus.NotActivated) return; //activation can only happen once in a lifetime of the object
			
			activation();
			ActivationStatus = ActivationStatus.ActivatedRead;
		}

		ExpressType IPersistEntity.ExpressType { get { return Model.Metadata.ExpressType(this);  } }
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcExternalReference(IModel model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcURIReference? _location;
		private IfcIdentifier? _identification;
		private IfcLabel? _name;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(1, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 1)]
		public IfcURIReference? @Location 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _location;
				((IPersistEntity)this).Activate(false);
				return _location;
			} 
			set
			{
				SetValue( v =>  _location = v, _location, value,  "Location");
			} 
		}	
		[EntityAttribute(2, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 2)]
		public IfcIdentifier? @Identification 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _identification;
				((IPersistEntity)this).Activate(false);
				return _identification;
			} 
			set
			{
				SetValue( v =>  _identification = v, _identification, value,  "Identification");
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 3)]
		public IfcLabel? @Name 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _name;
				((IPersistEntity)this).Activate(false);
				return _name;
			} 
			set
			{
				SetValue( v =>  _name = v, _name, value,  "Name");
			} 
		}	
		#endregion



		#region Inverse attributes
		[InverseProperty("RelatingReference")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 4)]
		public IEnumerable<IfcExternalReferenceRelationship> @ExternalReferenceForResources 
		{ 
			get 
			{
				return Model.Instances.Where<IfcExternalReferenceRelationship>(e => Equals(e.RelatingReference), "RelatingReference", this);
			} 
		}
		#endregion

		#region INotifyPropertyChanged implementation
		 
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged( string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

		#endregion

		#region Transactional property setting

		protected void SetValue<TProperty>(Action<TProperty> setter, TProperty oldValue, TProperty newValue, string notifyPropertyName)
		{
			//activate for write if it is not activated yet
			if (ActivationStatus != ActivationStatus.ActivatedReadWrite)
				((IPersistEntity)this).Activate(true);

			//just set the value if the model is marked as non-transactional
			if (!Model.IsTransactional)
			{
				setter(newValue);
				NotifyPropertyChanged(notifyPropertyName);
				return;
			}

			//check there is a transaction
			var txn = Model.CurrentTransaction;
			if (txn == null) throw new Exception("Operation out of transaction.");

			Action doAction = () => {
				setter(newValue);
				NotifyPropertyChanged(notifyPropertyName);
			};
			Action undoAction = () => {
				setter(oldValue);
				NotifyPropertyChanged(notifyPropertyName);
			};
			doAction();

			//do action and THAN add to transaction so that it gets the object in new state
			txn.AddReversibleAction(doAction, undoAction, this, ChangeType.Modified);
		}

		#endregion

		#region IPersist implementation
		public virtual void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
					_location = value.StringVal;
					return;
				case 1: 
					_identification = value.StringVal;
					return;
				case 2: 
					_name = value.StringVal;
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcExternalReference other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcExternalReference
            var root = (@IfcExternalReference)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcExternalReference left, @IfcExternalReference right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcExternalReference left, @IfcExternalReference right)
        {
            return !(left == right);
        }

        public static bool operator ==(@IfcExternalReference left, IfcLightDistributionDataSourceSelect right)
		{
			return left == right as @IfcExternalReference;
		}

		public static bool operator !=(@IfcExternalReference left, IfcLightDistributionDataSourceSelect right)
		{
			return !(left == right);
		}

        public static bool operator ==(@IfcExternalReference left, IfcObjectReferenceSelect right)
		{
			return left == right as @IfcExternalReference;
		}

		public static bool operator !=(@IfcExternalReference left, IfcObjectReferenceSelect right)
		{
			return !(left == right);
		}

        public static bool operator ==(@IfcExternalReference left, IfcResourceObjectSelect right)
		{
			return left == right as @IfcExternalReference;
		}

		public static bool operator !=(@IfcExternalReference left, IfcResourceObjectSelect right)
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