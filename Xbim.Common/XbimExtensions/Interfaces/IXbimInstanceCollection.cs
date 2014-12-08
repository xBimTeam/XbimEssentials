using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IXbimInstanceCollection : IEnumerable<IPersistIfcEntity>
    {
        IEnumerable<T> Where<T>(Expression<Func<T, bool>> expr) where T : IPersistIfcEntity;
        IEnumerable<T> OfType<T>() where T : IPersistIfcEntity;
        IEnumerable<T> OfType<T>(bool activate) where T : IPersistIfcEntity;
        IEnumerable<IPersistIfcEntity> OfType(string StringType, bool activate);
        IPersistIfcEntity New(Type t);
        T New<T>(InitProperties<T> initPropertiesFunc) where T : IPersistIfcEntity, new();
        T New<T>() where T : IPersistIfcEntity, new();
        IPersistIfcEntity this[int label] { get; }
        long Count { get; }
        long CountOf<T>() where T : IPersistIfcEntity;
        IPersistIfcEntity GetFromGeometryLabel(int geometryLabel);
    }
}
