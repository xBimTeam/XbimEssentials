// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc2x3.PresentationResource;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.PresentationAppearanceResource;
//## Custom using statements
//##

namespace Xbim.Ifc2x3.Interfaces
{
	/// <summary>
    /// Readonly interface for IfcTextStyle
    /// </summary>
	// ReSharper disable once PartialTypeWithSinglePart
	public partial interface @IIfcTextStyle : IIfcPresentationStyle, IfcPresentationStyleSelect
	{
		IIfcCharacterStyleSelect @TextCharacterAppearance { get; }
		IIfcTextStyleSelect @TextStyle { get; }
		IIfcTextFontSelect @TextFontStyle { get; }
	
	}
}

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	[ExpressType("IfcTextStyle", 427)]
	// ReSharper disable once PartialTypeWithSinglePart
	public  partial class @IfcTextStyle : IfcPresentationStyle, IInstantiableEntity, IIfcTextStyle, IEquatable<@IfcTextStyle>
	{
		#region IIfcTextStyle explicit implementation
		IIfcCharacterStyleSelect IIfcTextStyle.TextCharacterAppearance { get { return @TextCharacterAppearance; } }	
		IIfcTextStyleSelect IIfcTextStyle.TextStyle { get { return @TextStyle; } }	
		IIfcTextFontSelect IIfcTextStyle.TextFontStyle { get { return @TextFontStyle; } }	
		 
		#endregion

		//internal constructor makes sure that objects are not created outside of the model/ assembly controlled area
		internal IfcTextStyle(IModel model) : base(model) 		{ 
			Model = model; 
		}

		#region Explicit attribute fields
		private IfcCharacterStyleSelect _textCharacterAppearance;
		private IfcTextStyleSelect _textStyle;
		private IfcTextFontSelect _textFontStyle;
		#endregion
	
		#region Explicit attribute properties
		[EntityAttribute(2, EntityAttributeState.Optional, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 2)]
		public IfcCharacterStyleSelect @TextCharacterAppearance 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _textCharacterAppearance;
				((IPersistEntity)this).Activate(false);
				return _textCharacterAppearance;
			} 
			set
			{
				SetValue( v =>  _textCharacterAppearance = v, _textCharacterAppearance, value,  "TextCharacterAppearance", 2);
			} 
		}	
		[EntityAttribute(3, EntityAttributeState.Optional, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 3)]
		public IfcTextStyleSelect @TextStyle 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _textStyle;
				((IPersistEntity)this).Activate(false);
				return _textStyle;
			} 
			set
			{
				SetValue( v =>  _textStyle = v, _textStyle, value,  "TextStyle", 3);
			} 
		}	
		[EntityAttribute(4, EntityAttributeState.Mandatory, EntityAttributeType.Class, EntityAttributeType.None, -1, -1, 4)]
		public IfcTextFontSelect @TextFontStyle 
		{ 
			get 
			{
				if(ActivationStatus != ActivationStatus.NotActivated) return _textFontStyle;
				((IPersistEntity)this).Activate(false);
				return _textFontStyle;
			} 
			set
			{
				SetValue( v =>  _textFontStyle = v, _textFontStyle, value,  "TextFontStyle", 4);
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
					_textCharacterAppearance = (IfcCharacterStyleSelect)(value.EntityVal);
					return;
				case 2: 
					_textStyle = (IfcTextStyleSelect)(value.EntityVal);
					return;
				case 3: 
					_textFontStyle = (IfcTextFontSelect)(value.EntityVal);
					return;
				default:
					throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1, GetType().Name.ToUpper()));
			}
		}
		#endregion

		#region Equality comparers and operators
        public bool Equals(@IfcTextStyle other)
	    {
	        return this == other;
	    }

	    public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (GetType() != obj.GetType()) return false;

            // Cast as @IfcTextStyle
            var root = (@IfcTextStyle)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            //good enough as most entities will be in collections of  only one model, equals distinguishes for model
            return EntityLabel.GetHashCode(); 
        }

        public static bool operator ==(@IfcTextStyle left, @IfcTextStyle right)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.Model == right.Model);

        }

        public static bool operator !=(@IfcTextStyle left, @IfcTextStyle right)
        {
            return !(left == right);
        }

        public static bool operator ==(@IfcTextStyle left, IfcPresentationStyleSelect right)
		{
			return left == right as @IfcTextStyle;
		}

		public static bool operator !=(@IfcTextStyle left, IfcPresentationStyleSelect right)
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