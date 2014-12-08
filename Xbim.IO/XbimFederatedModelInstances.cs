using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;
using System.Collections;

namespace Xbim.IO
{

    internal class XbimFederatedInstancesEntityEnumerator : IEnumerator<IPersistIfcEntity>, IEnumerator
    {
        private List<XbimModel> models;
        int currentModelIndex = 0;
        private IfcPersistedInstanceCache cache;
        private XbimEntityCursor cursor;
        private int currentEntityLabel;

        public XbimFederatedInstancesEntityEnumerator(IEnumerable<XbimModel> models)
        {
            this.models = models.ToList();
            Reset();
        }
        public IPersistIfcEntity Current
        {
            get { return cache.GetInstance(currentEntityLabel); }
        }


        public void Reset()
        {
            currentEntityLabel = 0;
            currentModelIndex = 0;
            XbimModel first = models.FirstOrDefault();
            if (first != null)
            {
                cache = first.Cache;
                this.cursor = cache.GetEntityTable(); 
                cursor.MoveBeforeFirst();
            }
            
        }

        object IEnumerator.Current
        {
            get { return cache.GetInstance(currentEntityLabel); }
        }

        bool IEnumerator.MoveNext()
        {
            int label;
            if (cursor.TryMoveNextLabel(out label))
            {
                currentEntityLabel = label;
                return true;
            }
            else if (currentModelIndex < models.Count-1) //we have more models to process
            {
                currentModelIndex++; //go to next model
                cache.FreeTable(cursor);
                cache = models[currentModelIndex].Cache;
                this.cursor = cache.GetEntityTable();
                cursor.MoveBeforeFirst();
                if (cursor.TryMoveNextLabel(out label))
                {
                    currentEntityLabel = label;
                    return true;
                }
            }
            return false;
        }


        public void Dispose()
        {
            cache.FreeTable(cursor);
        }
    }
    public class XbimFederatedModelInstances : IXbimInstanceCollection
    {
        XbimModel _model;

        public IEnumerable<IPersistIfcEntity> OfType(string StringType, bool activate)
        {
            
            foreach (var instance in _model.InstancesLocal.OfType(StringType, activate))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType(StringType, activate))
                    yield return instance;
            
            //long[] l = new long[] { -1, 2 };
            //foreach (var item in l)
            //{
            //    yield return item;    
            //}
        }

        public XbimFederatedModelInstances(XbimModel model)
        {
            _model = model;
        }
        public IEnumerable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expr) where T : IPersistIfcEntity
        {
            foreach (var instance in _model.InstancesLocal.Where<T>(expr))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.Where<T>(expr))
                    yield return instance;
        }

        public IEnumerable<T> OfType<T>() where T : IPersistIfcEntity
        {
            foreach (var instance in _model.InstancesLocal.OfType<T>())
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType<T>())
                    yield return instance;
        }

        public IEnumerable<T> OfType<T>(bool activate) where T : IPersistIfcEntity
        {
            foreach (var instance in _model.InstancesLocal.OfType<T>(activate))
                yield return instance;
            foreach (var refModel in _model.ReferencedModels)
                foreach (var instance in refModel.Model.Instances.OfType<T>(activate))
                    yield return instance;
        }

        public IPersistIfcEntity New(Type t)
        {
            return _model.InstancesLocal.New(t);
        }

        public T New<T>(InitProperties<T> initPropertiesFunc) where T : IPersistIfcEntity, new()
        {
            return _model.InstancesLocal.New<T>(initPropertiesFunc);
        }

        public T New<T>() where T : IPersistIfcEntity, new()
        {
            return _model.InstancesLocal.New<T>();
        }


        /// <summary>
        /// returns the local instance with the given label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public IPersistIfcEntity this[int label]
        {
            get { return _model.InstancesLocal[label]; }
        }
        /// <summary>
        /// Returns the instance that corresponds to this handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public IPersistIfcEntity this[XbimInstanceHandle handle]
        {
            get { return handle.GetEntity(); }
        }

        public long Count
        {
            get
            {
                long total = _model.InstancesLocal.Count;
                foreach (var refModel in _model.ReferencedModels)
                    total += refModel.Model.Instances.Count;
                return total;
            }
        }

        public long CountOf<T>() where T : IPersistIfcEntity
        {
            long total = _model.InstancesLocal.CountOf<T>();
            foreach (var refModel in _model.ReferencedModels)
                total += refModel.Model.Instances.CountOf<T>();
            return total;
        }

        /// <summary>
        /// returns the geometry from the local instances
        /// Does not access federated model geometry
        /// </summary>
        /// <param name="geometryLabel"></param>
        /// <returns></returns>
        public IPersistIfcEntity GetFromGeometryLabel(int geometryLabel)
        {
            XbimGeometryHandle filledGeomData = _model.Cache.GetGeometryHandle(geometryLabel);
            return _model.Cache.GetInstance(filledGeomData.ProductLabel, true, true);
        }

       

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new XbimFederatedInstancesEntityEnumerator(_model.AllModels);
        }

        IEnumerator<IPersistIfcEntity> IEnumerable<IPersistIfcEntity>.GetEnumerator()
        {
            return new XbimFederatedInstancesEntityEnumerator(_model.AllModels);
        }
    }
}
