#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IIfcPersistExtensions.cs
// (See accompanying copyright.rtf)

#endregion

#region Directives
using System;
using Xbim.Common.Exceptions;

using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.Transactions;
using System.Runtime.CompilerServices;
#endregion

[assembly: InternalsVisibleTo("Xbim.IO"), InternalsVisibleTo("Xbim.Ifc2x3")]

namespace Xbim.XbimExtensions
{
    /// <summary>
    /// Extension methods for the <see cref="IPersistIfc"/> interface.
    /// </summary>
    public static class IIfcPersistExtensions
    {

        /// <summary>
        /// Handles the case where a property was not expected for this entity.
        /// </summary>
        /// <param name="persistIfc">The item being parsed.</param>
        /// <param name="propIndex">Index of the property.</param>
        /// <param name="value">The value of the property.</param>
    
        internal static void HandleUnexpectedAttribute(this IPersistIfc persistIfc, int propIndex, IPropertyValue value)
        {
            // TODO: Review this workaround for older IFC files with extraneous properties
            if (value.Type == IfcParserType.Enum && String.Compare(value.EnumVal, "NOTDEFINED") == 0)
                return;

            throw new XbimParserException(string.Format("Attribute index {0} is out of range for {1}", propIndex + 1,
                                                      persistIfc.GetType().Name.ToUpper()));
        }

        /// <summary>
        ///   Set a property /field value, if a transaction is active it is transacted and undoable, if the owner supports INotifyPropertyChanged, the required events will be raised
        /// </summary>
        /// <typeparam name = "TProperty"></typeparam>
        /// The property type to be set
        /// <param name = "field"></param>
        /// The field to be set
        /// <param name = "newValue"></param>
        /// The value to set the field to
        /// <param name = "setter"></param>
        /// The function to set and unset the field
        /// <param name = "notifyPropertyName"></param>
        /// A list of property names of the owner to raise notification on
        internal static void SetModelValue<TProperty>(this IPersistIfc entity, IPersistIfcEntity target, ref TProperty field, TProperty newValue,
                                                      ReversibleInstancePropertySetter<TProperty> setter,
                                                      string notifyPropertyName)
        {
            //The object must support Property Change Notification so notify
            ISupportChangeNotification iPropChanged = target as ISupportChangeNotification;

            if (iPropChanged != null)
            {
                target.Activate(true);
               // Transaction.AddPropertyChange(setter, field, newValue);
                
                iPropChanged.NotifyPropertyChanging(notifyPropertyName);
                field = newValue;
                //target.ModelOf.Update(target);
                iPropChanged.NotifyPropertyChanged(notifyPropertyName);
                
            }
            else
                throw new Exception(
                    string.Format(
                        "Request to Notify Property Changes on type {0} that does not support ISupportChangeNotification",
                        target.GetType().Name));
        }

    }
}
