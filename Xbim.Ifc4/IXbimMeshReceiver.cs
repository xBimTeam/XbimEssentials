namespace Xbim.Ifc4
{
    public interface IXbimMeshReceiver
    {
        //mesh operation has commenced
        void BeginUpdate();
        //mesh operation has ended
        void EndUpdate();
        //begin operation on a new face
        int AddFace();
        //add a node to the specified face, with normal and texture coord
        int AddNode(int face, double px, double py, double pz, double nx, double ny, double nz, double u, double v);
        //add a node to the specified face with normal only
        int AddNode(int face, double px, double py, double pz, double nx, double ny, double nz);
        //add a node to the specified face, the normal is not defined and must be calculated
        int AddNode(int face, double px, double py, double pz);
        //add a triangle to the face, a, b and c are previously added nodes to the face
        void AddTriangle(int face, int a, int b, int c);
        //add a quadrilateral to the face, a, b, c and d c are previously added nodes to the face
        void AddQuad(int face, int a, int b, int c, int d);
        /// <summary>
        /// The front, back or both material
        /// </summary>
        SurfaceStyling SurfaceStyling { get; set; }
        
    }
}
