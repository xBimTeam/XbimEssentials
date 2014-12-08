#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ModelManager.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.XbimExtensions.Transactions;
using Xbim.XbimExtensions.Transactions.Extensions;

#endregion

namespace Xbim.XbimExtensions
{
    public enum ModelValidationMode
    {
        ValidationOn,
        ValidationOff
    }

    public class ValidationErrorCollection : List<string>
    {
    }

    public delegate void XbimValidationEventHandler(string errorMsg);
        
    public static class ModelManager
    {
        private static readonly HashSet<IModel> _models;

        /// <summary>
        ///   System wide variable to determine if validation is on or off, default = on
        ///   For improved performance trun to off for release builds
        /// </summary>
        public static ModelValidationMode Validation = ModelValidationMode.ValidationOff;

        private static ValidationErrorCollection _modelValidationErrors = new ValidationErrorCollection();
        private static HashSet<object> _validated = new HashSet<object>();


        [ThreadStatic] internal static IModel _lastAccessedModel;
        [ThreadStatic] internal static IPersistIfc _lastAccessedInstance;

        [ThreadStatic] internal static IModel _transactingModel;


        static ModelManager()
        {
            _models = new HashSet<IModel>();
#if DEBUG
            Validation = ModelValidationMode.ValidationOn;
#endif
        }

        public static IModel LastAccessedModel
        {
            get { return _lastAccessedModel; }
        }

        public static IModel TransactingModel
        {
            get { return _transactingModel; }
            set { _transactingModel = value; }
        }


        //public static XbimMemoryModel CreateModelTransient()
        //{
        //    XbimMemoryModel model = new XbimMemoryModel();
        //    if (_lastAccessedModel == null) _lastAccessedModel = model;
        //    _models.Add_Reversible(model);
        //    return model;
        //}

        public static void AddModel(IModel model)
        {
            if (_lastAccessedModel == null) _lastAccessedModel = model;
            _models.Add_Reversible(model);
        }

        ///// <summary>
        /////   Releases the Model and nulls all values, model is no longer useable when this is called
        ///// </summary>
        ///// <param name = "model"></param>
        //public static void ReleaseModel(IModel model)
        //{
        //    _models.Remove_Reversible(model);
        //    if (_lastAccessedModel == model)
        //    {
        //        _lastAccessedModel = null;
        //        _lastAccessedInstance = null;
        //    }
        //}

        public static IEnumerable<IModel> Models
        {
            get { return _models; }
        }

        public static IModel ModelOf(IPersistIfcEntity instance)
        {
#if SupportActivation
            if (instance.ModelOf != null) return instance.ModelOf;
            else
#endif
                if (instance == _lastAccessedInstance) return _lastAccessedModel;

            if (_lastAccessedModel != null && _lastAccessedModel.ContainsInstance(instance))
                //try this first to improve performance
            {
                _lastAccessedInstance = instance;
                return _lastAccessedModel;
            }
            else
            {
                foreach (IModel model in _models.Where(m => m != _lastAccessedModel))
                {
                    if (model.ContainsInstance(instance))
                    {
                        _lastAccessedModel = model;
                        _lastAccessedInstance = instance;
                        return model;
                    }
                }
            }
            throw new ArgumentException(
                "Failure in ModelManager.ModelOf, the instance is in none of the current models", "instance");
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
        internal static void SetModelValue<TProperty>(IPersistIfcEntity target, ref TProperty field, TProperty newValue,
                                                      ReversibleInstancePropertySetter<TProperty> setter,
                                                      string notifyPropertyName)
        {
            //The object must support Property Change Notification so notify
            ISupportChangeNotification iPropChanged = target as ISupportChangeNotification;

            if (iPropChanged != null)
            {
                Transaction.AddPropertyChange(setter, field, newValue);
#if SupportActivation
                
                target.Activate(true);
#endif
                iPropChanged.NotifyPropertyChanging(notifyPropertyName);
                field = newValue;
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