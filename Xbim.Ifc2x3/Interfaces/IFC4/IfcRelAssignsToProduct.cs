// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Interfaces;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.Kernel
{
	public partial class @IfcRelAssignsToProduct : IIfcRelAssignsToProduct
	{

		private  IIfcProductSelect _relatingProduct4;

		IIfcProductSelect IIfcRelAssignsToProduct.RelatingProduct 
		{ 
			get
			{
				return  _relatingProduct4 ?? RelatingProduct;
			} 
			set
			{
				if (value == null)
				{
					RelatingProduct = null;
					if (_relatingProduct4 != null)
						SetValue(v => _relatingProduct4 = v, _relatingProduct4, null, "RelatingProduct", byte.MaxValue);
					return;
				}
				
				var val = value as IfcProduct;
				if (val != null)
				{
					RelatingProduct = val;
					if (_relatingProduct4 != null)
						SetValue(v => _relatingProduct4 = v, _relatingProduct4, null, "RelatingProduct", byte.MaxValue);
					return;
				} 

				if(RelatingProduct != null)
					RelatingProduct = null;
				SetValue(v => _relatingProduct4 = v, _relatingProduct4, value, "RelatingProduct", byte.MaxValue);
				
			}
		}
	//## Custom code
	//##
	}
}