#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ReversibleProperty.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    /// <summary>
    ///   Represents a static method that sets a given property of a given instance to a given value.
    /// </summary>
    /// <typeparam name = "TOwner">Type that owns the property to set.</typeparam>
    /// <typeparam name = "TProperty">Type of property to set</typeparam>
    /// <param name = "instance">Instance to set property for.</param>
    /// <param name = "newValue">Value to set property to</param>
    public delegate void ReversiblePropertySetter<TOwner, TProperty>(TOwner instance, TProperty newValue)
        where TOwner : class;

    /// <summary>
    ///   Represents a instance method that sets a given instance property to a given value.
    /// </summary>
    /// <typeparam name = "TProperty">Type of property to set</typeparam>
    /// <param name = "newValue">Value to set property to.</param>
    public delegate void ReversibleInstancePropertySetter<TProperty>(TProperty newValue);

    public delegate void ReversibleOperation<TClass>();


    /*
    public class ReversibleProperty<TOwner,TProperty>  
    {
        readonly string propName;
        readonly ReversiblePropertySetter<TOwner, TProperty> setter;

        public ReversibleProperty(string propertyName, ReversiblePropertySetter<TOwner,TProperty> setter)
        {
            this.propName = propertyName;
            this.setter = setter;
        }

        public void SetValue(TOwner instance, TProperty newValue)
        {
            setter(instance, newValue);
        }
    }
    */

    public class ReversiblePropertyEdit<TOwner, TProperty> : Edit where TOwner : class
    {
        private readonly ReversiblePropertySetter<TOwner, TProperty> setter;
        private readonly TOwner instance;
        private TProperty oldValue;
        private TProperty newValue;

        public ReversiblePropertyEdit(ReversiblePropertySetter<TOwner, TProperty> setter, TOwner instance,
                                      TProperty oldValue, TProperty newValue)
        {
            this.setter = setter;
            this.instance = instance;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public override Edit Reverse()
        {
            setter(instance, oldValue);
            Switch(ref oldValue, ref newValue);
            return this;
        }
    }

    public class ReversibleInstancePropertyEdit<TProperty> : Edit
    {
        private readonly ReversibleInstancePropertySetter<TProperty> setter;
        private TProperty oldValue;
        private TProperty newValue;

        public ReversibleInstancePropertyEdit(ReversibleInstancePropertySetter<TProperty> setter, TProperty oldValue,
                                              TProperty newValue)
        {
            this.setter = setter;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public override Edit Reverse()
        {
            setter(oldValue);
            Switch(ref oldValue, ref newValue);
            return this;
        }
    }

    public class ReversibleOperationEdit<TClass> : Edit
    {
        private ReversibleOperation<TClass> operation1;
        private ReversibleOperation<TClass> operation2;


        public ReversibleOperationEdit(ReversibleOperation<TClass> op1, ReversibleOperation<TClass> op2)
        {
            this.operation1 = op1;
            this.operation2 = op2;
        }

        public override Edit Reverse()
        {
            operation1();
            Switch(ref operation1, ref operation2);
            return this;
        }
    }
}