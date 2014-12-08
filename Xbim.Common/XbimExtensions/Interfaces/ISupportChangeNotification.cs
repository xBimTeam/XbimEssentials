#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ISupportChangeNotification.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

using System.ComponentModel;
namespace Xbim.XbimExtensions.Interfaces
{
    public interface ISupportChangeNotification : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        ///   Raises the NotifyPropertyChanged event on this object
        /// </summary>
        /// <param name = "propertyName"></param>
        void NotifyPropertyChanged(string propertyName);

        void NotifyPropertyChanging(string propertyName);
    }
}