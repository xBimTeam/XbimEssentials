using System.Collections.ObjectModel;

namespace Xbim.Tessellator
{
    class XbimContourVertexCollection : KeyedCollection<Vec3, ContourVertex>
    {
     
        public XbimContourVertexCollection(float precision)
            : base(new Vec3EqualityComparer(precision))
        {
          
        }

        protected override Vec3 GetKeyForItem(ContourVertex item)
        {
            return item.Position;
        }

        public void Add(Vec3 v, ref ContourVertex contourVertex)
        {   
            contourVertex.Position = v;
            contourVertex.Data = Count;
            base.Add(contourVertex);
        }
        public new void Add(ContourVertex cv)
        {
            cv.Data = Count;
            base.Add(cv);
        }
    }
}
