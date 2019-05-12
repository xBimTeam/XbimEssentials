using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Common;
using Xbim.Common.Collections;
using Xbim.Common.Metadata;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Tessellator
{
    //set of classes to implement interfaces that convert an IIfcPolygonalFaceSet into  and Ifc set of IIfcFaces, so that we can use a  standard meshing algorithm
    public class XbimPolygonalFaceSet : IList<IIfcFace>
    {
        IIfcPolygonalFaceSet _faceSet;
        public XbimPolygonalFaceSet(IIfcPolygonalFaceSet faceSet)
        {
            _faceSet = faceSet;
        }

        public IIfcFace this[int index] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public int Count => _faceSet.Faces.Count;

        public bool IsReadOnly => true;

        public void Add(IIfcFace item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(IIfcFace item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(IIfcFace[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<IIfcFace> GetEnumerator()
        {
            return _faceSet.Faces.Select(f => new XbimPolygonalFace(f, _faceSet.Coordinates, _faceSet.PnIndex)).GetEnumerator();
        }

        public int IndexOf(IIfcFace item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, IIfcFace item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(IIfcFace item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _faceSet.Faces.Select(f => new XbimPolygonalFace(f, _faceSet.Coordinates, _faceSet.PnIndex)).GetEnumerator();
        }
    }


    /// <summary>
    /// Wrapper to support IIfcFace interface for an IIfcPolygonalFaceSet
    /// </summary>
    public class XbimPolygonalFace : IIfcFace
    {

        private IIfcIndexedPolygonalFace polygonalFace;
        private IIfcCartesianPointList3D coordinates;
        private IItemSet<IfcPositiveInteger> pnIndex;



        public XbimPolygonalFace(IIfcIndexedPolygonalFace polygonalFace, IIfcCartesianPointList3D coordinates, IItemSet<IfcPositiveInteger> pnIndex)
        {
            this.polygonalFace = polygonalFace;
            this.coordinates = coordinates;
            this.pnIndex = pnIndex;
        }

        public IItemSet<IIfcFaceBound> Bounds
        {
            get
            {
                return new XbimFaceBoundSet(polygonalFace, coordinates, pnIndex);
            }
        }

        public IEnumerable<IIfcTextureMap> HasTextureMaps => throw new NotImplementedException();

        public IEnumerable<IIfcPresentationLayerAssignment> LayerAssignment => polygonalFace.LayerAssignment;

        public IEnumerable<IIfcStyledItem> StyledByItem => polygonalFace.StyledByItem;

        public int EntityLabel => polygonalFace.EntityLabel;

        public IModel Model => polygonalFace.Model;

        public bool Activated => polygonalFace.Activated;

        public ExpressType ExpressType => polygonalFace.ExpressType;

        public IModel ModelOf => polygonalFace.Model;

        public void Parse(int propIndex, IPropertyValue value, int[] nested)
        {
            polygonalFace.Parse(propIndex, value, nested);
        }
    }

    public class XbimFaceBoundSet : IItemSet<IIfcFaceBound>
    {
        XbimPolygonalFaceBound[] faceBounds;
        // IIfcIndexedPolygonalFace indexFace;
        public XbimFaceBoundSet(IIfcIndexedPolygonalFace face, IIfcCartesianPointList3D coordinates, IItemSet<IfcPositiveInteger> pnIndex)
        {
            // indexFace = face;
            if (face is IIfcIndexedPolygonalFaceWithVoids)
            {
                var faceWithVoids = (IIfcIndexedPolygonalFaceWithVoids)face;
                faceBounds = new XbimPolygonalFaceBound[faceWithVoids.InnerCoordIndices.Count + 1];
                faceBounds[0] = new XbimPolygonalFaceBound(faceWithVoids.CoordIndex, coordinates, pnIndex);
                for (int i = 0; i < faceWithVoids.InnerCoordIndices.Count; i++)
                {
                    faceBounds[i + 1] = new XbimPolygonalFaceBound(faceWithVoids.InnerCoordIndices[i], coordinates, pnIndex);
                }
            }
            else //there will just be one
            {
                faceBounds = new[] { new XbimPolygonalFaceBound(face.CoordIndex, coordinates, pnIndex) };
            }
        }
        public IIfcFaceBound this[int index] { get => faceBounds[index]; set => throw new NotSupportedException(); }

        public int Count => faceBounds.Length;

        public bool IsReadOnly => true;

        public IPersistEntity OwningEntity => throw new NotImplementedException();
#pragma warning disable 67
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67
        public void Add(IIfcFaceBound item)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<IIfcFaceBound> values)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IIfcFaceBound item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IIfcFaceBound[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IIfcFaceBound FirstOrDefault(Func<IIfcFaceBound, bool> predicate)
        {
            return faceBounds[0];
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate) where TF : IIfcFaceBound
        {
            throw new NotImplementedException();
        }

        public IIfcFaceBound GetAt(int index)
        {
            return faceBounds[index];
        }

        public IEnumerator<IIfcFaceBound> GetEnumerator()
        {
            return faceBounds.Select(f => f).GetEnumerator();
        }

        public int IndexOf(IIfcFaceBound item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IIfcFaceBound item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IIfcFaceBound item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW : IIfcFaceBound
        {
            return faceBounds.Cast<TW>().Where(f => predicate(f));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return faceBounds.GetEnumerator();
        }
    }
    public class XbimPolygonalFaceBound : IIfcFaceBound
    {
        private IItemSet<IfcPositiveInteger> coordIndex;
        private IIfcCartesianPointList3D coordinates;
        private IItemSet<IfcPositiveInteger> pnIndex;

        public XbimPolygonalFaceBound(IItemSet<IfcPositiveInteger> coordIndex, IIfcCartesianPointList3D coordinates, IItemSet<IfcPositiveInteger> pnIndex)
        {
            this.coordIndex = coordIndex;
            this.coordinates = coordinates;
            this.pnIndex = pnIndex;
        }

        public IIfcLoop Bound
        {
            get => new XbimPolyLoop(coordIndex, coordinates, pnIndex);
            set => throw new NotSupportedException();
        }
        public IfcBoolean Orientation
        {
            get => true;
            set => throw new NotSupportedException();
        }

        public IEnumerable<IIfcPresentationLayerAssignment> LayerAssignment => throw new NotSupportedException();

        public IEnumerable<IIfcStyledItem> StyledByItem => throw new NotSupportedException();

        public int EntityLabel => throw new NotSupportedException();

        public IModel Model => throw new NotSupportedException();

        public bool Activated => true;

        public ExpressType ExpressType => throw new NotSupportedException();

        public IModel ModelOf => throw new NotSupportedException();

        public void Parse(int propIndex, IPropertyValue value, int[] nested)
        {
            throw new NotSupportedException();
        }
    }

    public class XbimPolyLoop : IIfcPolyLoop
    {
        private IItemSet<IfcPositiveInteger> coordIndex;
        private IIfcCartesianPointList3D coordinates;
        private IItemSet<IfcPositiveInteger> pnIndex;

        public XbimPolyLoop(IItemSet<IfcPositiveInteger> coordIndex, IIfcCartesianPointList3D coordinates, IItemSet<IfcPositiveInteger> pnIndex)
        {
            this.coordIndex = coordIndex;
            this.coordinates = coordinates;
            this.pnIndex = pnIndex;
        }

        public IEnumerable<IIfcPresentationLayerAssignment> LayerAssignment => throw new NotSupportedException();

        public IEnumerable<IIfcStyledItem> StyledByItem => throw new NotSupportedException();

        public int EntityLabel => throw new NotSupportedException();

        public IModel Model => throw new NotSupportedException();

        public bool Activated => true;

        public ExpressType ExpressType => throw new NotSupportedException();

        public IModel ModelOf => throw new NotSupportedException();

        public IItemSet<IIfcCartesianPoint> Polygon => new XbimCartesianPoint3dList(coordIndex, coordinates, pnIndex);

        public void Parse(int propIndex, IPropertyValue value, int[] nested)
        {
            throw new NotSupportedException();
        }
    }

    public class XbimCartesianPoint3dList : IItemSet<IIfcCartesianPoint>
    {
        List<IIfcCartesianPoint> points;
        public XbimCartesianPoint3dList(IItemSet<IfcPositiveInteger> coordIndex, IIfcCartesianPointList3D coordinates, IItemSet<IfcPositiveInteger> pnIndex)
        {
            points = new List<IIfcCartesianPoint>(coordIndex.Count);
            if (pnIndex != null && pnIndex.Count > 0) //we do a lookup
            {
                foreach (int idx in coordIndex)
                {
                    var pnt = coordinates.CoordList[(int)(pnIndex[idx-1]-1)];
                    points.Add(new XbimCartesianPoint3D(pnt[0], pnt[1], pnt[2]));
                }
            }
            else
            {
                foreach (int idx in coordIndex)
                {
                    var pnt = coordinates.CoordList[(int)(idx-1)];
                    points.Add(new XbimCartesianPoint3D(pnt[0], pnt[1], pnt[2]));
                }
            }

        }
        public IIfcCartesianPoint this[int index] { get => points[index]; set => throw new NotSupportedException(); }

        public int Count => points.Count;

        public bool IsReadOnly => true;

        public IPersistEntity OwningEntity => throw new NotImplementedException();
#pragma warning disable 67
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67
        public void Add(IIfcCartesianPoint item)
        {
            throw new NotSupportedException();
        }

        public void AddRange(IEnumerable<IIfcCartesianPoint> values)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(IIfcCartesianPoint item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(IIfcCartesianPoint[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IIfcCartesianPoint FirstOrDefault(Func<IIfcCartesianPoint, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate) where TF : IIfcCartesianPoint
        {
            throw new NotImplementedException();
        }

        public IIfcCartesianPoint GetAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IIfcCartesianPoint> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        public int IndexOf(IIfcCartesianPoint item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IIfcCartesianPoint item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(IIfcCartesianPoint item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW : IIfcCartesianPoint
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return points.GetEnumerator();
        }
    }

    public struct XbimCartesianPoint3D : IIfcCartesianPoint
    {

        public XbimCartesianPoint3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public IItemSet<IfcLengthMeasure> Coordinates => throw new NotSupportedException();

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public IEnumerable<IIfcPresentationLayerAssignment> LayerAssignment => throw new NotSupportedException();

        public IEnumerable<IIfcStyledItem> StyledByItem => throw new NotSupportedException();

        public IfcDimensionCount Dim => 3;

        public int EntityLabel => throw new NotSupportedException();

        public IModel Model => throw new NotSupportedException();

        public bool Activated => true;

        public ExpressType ExpressType => throw new NotSupportedException();

        public IModel ModelOf => throw new NotSupportedException();

        public void Parse(int propIndex, IPropertyValue value, int[] nested)
        {
            throw new NotSupportedException();
        }
    }

}
