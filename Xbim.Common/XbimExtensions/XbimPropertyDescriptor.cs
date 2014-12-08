#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    XbimPropertyDescriptor.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;

#endregion

namespace Xbim.XbimExtensions
{
    public delegate P GetValueCallback<T, P>(T instance);

    public delegate void SetValueCallback<T, P>(T instance, P value);

    public class XbimPropertyDescriptor<T, P> : PropertyDescriptor
    {
        private readonly GetValueCallback<T, P> _getValueCallback;
        private readonly SetValueCallback<T, P> _setValueCallback;

        public XbimPropertyDescriptor(string propertyName, GetValueCallback<T, P> getValueCallback,
                                      SetValueCallback<T, P> setValueCallback)
            : base(propertyName, null)
        {
            _getValueCallback = getValueCallback;
            _setValueCallback = setValueCallback;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof (T); }
        }

        public override object GetValue(object component)
        {
            return _getValueCallback((T) component);
        }

        public override bool IsReadOnly
        {
            get { return _setValueCallback == null; }
        }

        public override Type PropertyType
        {
            get { return typeof (P); }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            if (_setValueCallback != null)
            {
                _setValueCallback((T) component, (P) value);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}