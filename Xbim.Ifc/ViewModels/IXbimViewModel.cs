using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Common;

//using Xbim.IO;

namespace Xbim.Ifc.ViewModels
{
    // todo: bonghi: need to add Parent property to ensure efficiency in node finding for trees.
    public interface IXbimViewModel : INotifyPropertyChanged
    {
        IEnumerable<IXbimViewModel> Children { get; }
        string Name {get;}
        int EntityLabel { get; }
        IPersistEntity Entity { get; }
        IModel Model { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        IXbimViewModel CreatingParent { get; set; } 
    }
}
