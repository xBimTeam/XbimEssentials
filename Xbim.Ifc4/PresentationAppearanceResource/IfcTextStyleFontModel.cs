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
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.PresentationAppearanceResource;
//## Custom using statements
//##

namespace Xbim.Ifc4.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcTextStyleFontModel
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcTextStyleFontModel : IIfcPreDefinedTextFont
	{
		IEnumerable<IfcTextFontName> @FontFamily { get; }
		IfcFontStyle? @FontStyle { get; }
		IfcFontVariant? @FontVariant { get; }
		IfcFontWeight? @FontWeight { get; }
		IIfcSizeSelect @FontSize { get; }
	
	}
}

namespace Xbim.Ifc4.PresentationAppearanceResource
{
	[ExpressType("IfcTextStyleFontModel", 503)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcTextStyleFontModel : IfcPreDefinedTextFont, IInstantiableEntity, IIfcTextStyleFontModel, IEquatable<@IfcTextStyleFontModel>
	{
		#region IIfcTextStyleFontModel explicit implementation
		IEnumerable<IfcTextFontName> IIfcTextStyleFontModel.FontFamily { get { return @FontFamily; } }	
		IfcFontStyle? IIfcTextStyleFontModel.FontStyle { get { return @FontStyle; } }	
		IfcFontVariant? IIfcTextStyleFontModel.FontVariant { get { return @FontVariant; } }	
		IfcFontWeight? IIfcTextStyleFontModel.FontWeight { get { return @FontWeight; } }	
		IIfcSizeSelect IIfcTextStyleFontModel.FontSize { get { return @FontSize; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcTextStyleFontModel(IModel model) : base(model) 		{ 
			Model = model; 
			_fontFamily = new ItemSet<IfcTextFontName>( this, 0,  2);
		}

		#region Explicit attribute fields
		private ItemSet<IfcTextFontName> _fontFamily;
		private IfcFontStyle? _fontStyle;
		private IfcFontVariant? _fontVariant;
		private IfcFontWeight? _fontWeight;
		private IfcSizeSelect _fontSize;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(2, EntityAttributeState.Mandatory, EntityAttributeType.List, EntityAttributeType.None, 1, -1, 2)]
		public ItemSet<IfcTextFontName> @FontFamily 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _fontFamily;
				((IPersistEntity)this).Activate(false);
				return _fontFamily;
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 3)]
		public IfcFontStyle? @FontStyle 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _fontStyle;
				((IPersistEntity)this).Activate(false);
				return _fontStyle;
			} 
			set
			{
				SetValue( v =>  _fontStyle = v, _fontStyle, value,  "FontStyle", 3);
			} 
		}	
		[EntityAttribute(4, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 4)]
		public IfcFontVariant? @FontVariant 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _fontVariant;
				((IPersistEntity)this).Activate(false);
				return _fontVariant;
			} 
			set
			{
				SetValue( v =>  _fontVariant = v, _fontVariant, value,  "FontVariant", 4);
			} 
		}	
		[EntityAttribute(5, EntityAttributeState.Optional, EntityAttributeType.None, EntityAttributeType.None, -1, -1, 5)]
		public IfcFontWeight? @FontWeight 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _fontWeight;
				((IPersistEntity)this).Activate(false);
				return _fontWeight;
			} 
			set
			{
				SetValue( v =>  _fontWeight = v, _fontWeight, value,  "FontWeight", 5);
			} 
		}	
		[EntityAttribute(6, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 6)]
		public IfcSizeSelect @FontSize 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _fontSize;
				((IPersistEntity)this).Activate(false);
				return _fontSize;
			} 
			set
			{
				SetValue( v =>  _fontSize = v, _fontSize, value,  "FontSize", 6);
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
					_fontFamily.InternalAdd(value.StringVal);
					return;
				case 2: 
					_fontStyle = value.StringVal;
					return;
				case 3: 
					_fontVariant = value.StringVal;
					return;
				case 4: 
					_fontWeight = value.StringVal;
					return;
				case 5: 
					_fontSize = (IfcSizeSelect)(value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcTextStyleFontModel other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcTextStyleFontModel
            var root = (@IfcTextStyleFontModel)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcTextStyleFontModel left, @IfcTextStyleFontModel right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcTextStyleFontModel left, @IfcTextStyleFontModel right)
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