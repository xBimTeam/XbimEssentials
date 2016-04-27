// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ActorResource;
using Xbim.Ifc4.DateTimeResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc4.ExternalReferenceResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcDocumentInformation
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcDocumentInformation : IIfcExternalInformation, IfcDocumentSelect
	{
		IfcIdentifier @Identification { get; }
		IfcLabel @Name { get; }
		IfcText? @Description { get; }
		IfcURIReference? @Location { get; }
		IfcText? @Purpose { get; }
		IfcText? @IntendedUse { get; }
		IfcText? @Scope { get; }
		IfcLabel? @Revision { get; }
		IIfcActorSelect @DocumentOwner { get; }
		IEnumerable<IIfcActorSelect> @Editors { get; }
		IfcDateTime? @CreationTime { get; }
		IfcDateTime? @LastRevisionTime { get; }
		IfcIdentifier? @ElectronicFormat { get; }
		IfcDate? @ValidFrom { get; }
		IfcDate? @ValidUntil { get; }
		IfcDocumentConfidentialityEnum? @Confidentiality { get; }
		IfcDocumentStatusEnum? @Status { get; }
		IEnumerable<IIfcRelAssociatesDocument> @DocumentInfoForObjects {  get; }
		IEnumerable<IIfcDocumentReference> @HasDocumentReferences {  get; }
		IEnumerable<IIfcDocumentInformationRelationship> @IsPointedTo {  get; }
		IEnumerable<IIfcDocumentInformationRelationship> @IsPointer {  get; }
	
	}
}

namespace Xbim.Ifc4.ExternalReferenceResource
{
	[ExpressType("IfcDocumentInformation", 208)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcDocumentInformation : IfcExternalInformation, IInstantiableEntity, IIfcDocumentInformation, IEquatable<@IfcDocumentInformation>
	{
		#region IIfcDocumentInformation explicit implementation
		IfcIdentifier IIfcDocumentInformation.Identification { get { return @Identification; } }	
		IfcLabel IIfcDocumentInformation.Name { get { return @Name; } }	
		IfcText? IIfcDocumentInformation.Description { get { return @Description; } }	
		IfcURIReference? IIfcDocumentInformation.Location { get { return @Location; } }	
		IfcText? IIfcDocumentInformation.Purpose { get { return @Purpose; } }	
		IfcText? IIfcDocumentInformation.IntendedUse { get { return @IntendedUse; } }	
		IfcText? IIfcDocumentInformation.Scope { get { return @Scope; } }	
		IfcLabel? IIfcDocumentInformation.Revision { get { return @Revision; } }	
		IIfcActorSelect IIfcDocumentInformation.DocumentOwner { get { return @DocumentOwner; } }	
		IEnumerable<IIfcActorSelect> IIfcDocumentInformation.Editors { get { return @Editors; } }	
		IfcDateTime? IIfcDocumentInformation.CreationTime { get { return @CreationTime; } }	
		IfcDateTime? IIfcDocumentInformation.LastRevisionTime { get { return @LastRevisionTime; } }	
		IfcIdentifier? IIfcDocumentInformation.ElectronicFormat { get { return @ElectronicFormat; } }	
		IfcDate? IIfcDocumentInformation.ValidFrom { get { return @ValidFrom; } }	
		IfcDate? IIfcDocumentInformation.ValidUntil { get { return @ValidUntil; } }	
		IfcDocumentConfidentialityEnum? IIfcDocumentInformation.Confidentiality { get { return @Confidentiality; } }	
		IfcDocumentStatusEnum? IIfcDocumentInformation.Status { get { return @Status; } }	
		 
		IEnumerable<IIfcRelAssociatesDocument> IIfcDocumentInformation.DocumentInfoForObjects {  get { return @DocumentInfoForObjects; } }
		IEnumerable<IIfcDocumentReference> IIfcDocumentInformation.HasDocumentReferences {  get { return @HasDocumentReferences; } }
		IEnumerable<IIfcDocumentInformationRelationship> IIfcDocumentInformation.IsPointedTo {  get { return @IsPointedTo; } }
		IEnumerable<IIfcDocumentInformationRelationship> IIfcDocumentInformation.IsPointer {  get { return @IsPointer; } }
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcDocumentInformation(IModel model) : base(model) 		{ 
			Model = model; 
			_editors = new OptionalItemSet<IfcActorSelect>( this, 0 );
		}

		#region Explicit attribute fields
		private IfcIdentifier _identification;
		private IfcLabel _name;
		private IfcText? _description;
		private IfcURIReference? _location;
		private IfcText? _purpose;
		private IfcText? _intendedUse;
		private IfcText? _scope;
		private IfcLabel? _revision;
		private IfcActorSelect _documentOwner;
		private OptionalItemSet<IfcActorSelect> _editors;
		private IfcDateTime? _creationTime;
		private IfcDateTime? _lastRevisionTime;
		private IfcIdentifier? _electronicFormat;
		private IfcDate? _validFrom;
		private IfcDate? _validUntil;
		private IfcDocumentConfidentialityEnum? _confidentiality;
		private IfcDocumentStatusEnum? _status;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(1, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 1)]
		public IfcIdentifier @Identification 
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
		[EntityAttribute(2, EntityAttributeState.Mandatory, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 2)]
		public IfcLabel @Name 
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
		[EntityAttribute(3, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 3)]
		public IfcText? @Description 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _description;
				((IPersistEntity)this).Activate(false);
				return _description;
			} 
			set
			{
				SetValue( v =>  _description = v, _description, value,  "Description");
			} 
		}	
		[EntityAttribute(4, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 4)]
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
		[EntityAttribute(5, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public IfcText? @Purpose 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _purpose;
				((IPersistEntity)this).Activate(false);
				return _purpose;
			} 
			set
			{
				SetValue( v =>  _purpose = v, _purpose, value,  "Purpose");
			} 
		}	
		[EntityAttribute(6, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 6)]
		public IfcText? @IntendedUse 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _intendedUse;
				((IPersistEntity)this).Activate(false);
				return _intendedUse;
			} 
			set
			{
				SetValue( v =>  _intendedUse = v, _intendedUse, value,  "IntendedUse");
			} 
		}	
		[EntityAttribute(7, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 7)]
		public IfcText? @Scope 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _scope;
				((IPersistEntity)this).Activate(false);
				return _scope;
			} 
			set
			{
				SetValue( v =>  _scope = v, _scope, value,  "Scope");
			} 
		}	
		[EntityAttribute(8, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 8)]
		public IfcLabel? @Revision 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _revision;
				((IPersistEntity)this).Activate(false);
				return _revision;
			} 
			set
			{
				SetValue( v =>  _revision = v, _revision, value,  "Revision");
			} 
		}	
		[EntityAttribute(9, EntityAttributeState.Optional, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 9)]
		public IfcActorSelect @DocumentOwner 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _documentOwner;
				((IPersistEntity)this).Activate(false);
				return _documentOwner;
			} 
			set
			{
				SetValue( v =>  _documentOwner = v, _documentOwner, value,  "DocumentOwner");
			} 
		}	
		[EntityAttribute(10, EntityAttributeState.Optional, EntityAttributeType.Set, EntityAttributeType.Class, 1, -1, 10)]
		public OptionalItemSet<IfcActorSelect> @Editors 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _editors;
				((IPersistEntity)this).Activate(false);
				return _editors;
			} 
		}	
		[EntityAttribute(11, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 11)]
		public IfcDateTime? @CreationTime 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _creationTime;
				((IPersistEntity)this).Activate(false);
				return _creationTime;
			} 
			set
			{
				SetValue( v =>  _creationTime = v, _creationTime, value,  "CreationTime");
			} 
		}	
		[EntityAttribute(12, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 12)]
		public IfcDateTime? @LastRevisionTime 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _lastRevisionTime;
				((IPersistEntity)this).Activate(false);
				return _lastRevisionTime;
			} 
			set
			{
				SetValue( v =>  _lastRevisionTime = v, _lastRevisionTime, value,  "LastRevisionTime");
			} 
		}	
		[EntityAttribute(13, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 13)]
		public IfcIdentifier? @ElectronicFormat 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _electronicFormat;
				((IPersistEntity)this).Activate(false);
				return _electronicFormat;
			} 
			set
			{
				SetValue( v =>  _electronicFormat = v, _electronicFormat, value,  "ElectronicFormat");
			} 
		}	
		[EntityAttribute(14, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 14)]
		public IfcDate? @ValidFrom 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _validFrom;
				((IPersistEntity)this).Activate(false);
				return _validFrom;
			} 
			set
			{
				SetValue( v =>  _validFrom = v, _validFrom, value,  "ValidFrom");
			} 
		}	
		[EntityAttribute(15, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 15)]
		public IfcDate? @ValidUntil 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _validUntil;
				((IPersistEntity)this).Activate(false);
				return _validUntil;
			} 
			set
			{
				SetValue( v =>  _validUntil = v, _validUntil, value,  "ValidUntil");
			} 
		}	
		[EntityAttribute(16, EntityAttributeState.Optional, EntityAttributeType.Enum, EntityAttributeType.None, -1, -1, 16)]
		public IfcDocumentConfidentialityEnum? @Confidentiality 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _confidentiality;
				((IPersistEntity)this).Activate(false);
				return _confidentiality;
			} 
			set
			{
				SetValue( v =>  _confidentiality = v, _confidentiality, value,  "Confidentiality");
			} 
		}	
		[EntityAttribute(17, EntityAttributeState.Optional, EntityAttributeType.Enum, EntityAttributeType.None, -1, -1, 17)]
		public IfcDocumentStatusEnum? @Status 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _status;
				((IPersistEntity)this).Activate(false);
				return _status;
			} 
			set
			{
				SetValue( v =>  _status = v, _status, value,  "Status");
			} 
		}	
		#endregion



		#region Inverse attributes
		[InverseProperty("RelatingDocument")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 18)]
		public IEnumerable<IfcRelAssociatesDocument> @DocumentInfoForObjects 
		{ 
			get 
			{
				return Model.Instances.Where<IfcRelAssociatesDocument>(e => Equals(e.RelatingDocument), "RelatingDocument", this);
			} 
		}
		[InverseProperty("ReferencedDocument")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 19)]
		public IEnumerable<IfcDocumentReference> @HasDocumentReferences 
		{ 
			get 
			{
				return Model.Instances.Where<IfcDocumentReference>(e => Equals(e.ReferencedDocument), "ReferencedDocument", this);
			} 
		}
		[InverseProperty("RelatedDocuments")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, -1, 20)]
		public IEnumerable<IfcDocumentInformationRelationship> @IsPointedTo 
		{ 
			get 
			{
				return Model.Instances.Where<IfcDocumentInformationRelationship>(e => e.RelatedDocuments != null &&  e.RelatedDocuments.Contains(this), "RelatedDocuments", this);
			} 
		}
		[InverseProperty("RelatingDocument")]
		[EntityAttribute(-1, EntityAttributeState.Mandatory, EntityAttributeType.Set, EntityAttributeType.Class, 0, 1, 21)]
		public IEnumerable<IfcDocumentInformationRelationship> @IsPointer 
		{ 
			get 
			{
				return Model.Instances.Where<IfcDocumentInformationRelationship>(e => Equals(e.RelatingDocument), "RelatingDocument", this);
			} 
		}
		#endregion


		#region IPersist implementation
		public  override void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
		{
			switch (propIndex)
			{
				case 0: 
					_identification = value.StringVal;
					return;
				case 1: 
					_name = value.StringVal;
					return;
				case 2: 
					_description = value.StringVal;
					return;
				case 3: 
					_location = value.StringVal;
					return;
				case 4: 
					_purpose = value.StringVal;
					return;
				case 5: 
					_intendedUse = value.StringVal;
					return;
				case 6: 
					_scope = value.StringVal;
					return;
				case 7: 
					_revision = value.StringVal;
					return;
				case 8: 
					_documentOwner = (IfcActorSelect)(value.EntityVal);
					return;
				case 9: 
					if (_editors == null) _editors = new OptionalItemSet<IfcActorSelect>( this );
					_editors.InternalAdd((IfcActorSelect)value.EntityVal);
					return;
				case 10: 
					_creationTime = value.StringVal;
					return;
				case 11: 
					_lastRevisionTime = value.StringVal;
					return;
				case 12: 
					_electronicFormat = value.StringVal;
					return;
				case 13: 
					_validFrom = value.StringVal;
					return;
				case 14: 
					_validUntil = value.StringVal;
					return;
				case 15: 
                    _confidentiality = (IfcDocumentConfidentialityEnum) System.Enum.Parse(typeof (IfcDocumentConfidentialityEnum), value.EnumVal, true);
					return;
				case 16: 
                    _status = (IfcDocumentStatusEnum) System.Enum.Parse(typeof (IfcDocumentStatusEnum), value.EnumVal, true);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcDocumentInformation other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcDocumentInformation
            var root = (@IfcDocumentInformation)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcDocumentInformation left, @IfcDocumentInformation right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcDocumentInformation left, @IfcDocumentInformation right)
        {
            return !(left == right);
        }

        public static bool operator ==(@IfcDocumentInformation left, IfcDocumentSelect right)
		{
			return left == right as @IfcDocumentInformation;
		}

		public static bool operator !=(@IfcDocumentInformation left, IfcDocumentSelect right)
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