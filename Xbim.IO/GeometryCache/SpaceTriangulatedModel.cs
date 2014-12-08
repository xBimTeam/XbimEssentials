#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.IO
// Filename:    SpaceTriangulatedModel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.IO;
using System.Windows.Media.Media3D;
using System.Xml;

#endregion

namespace Xbim.IO.GeometryCache
{
    public class SpaceTriangulatedModel
    {
        public long EntityLabel = 0;
        public Matrix3D Matrix;
        public MeshGeometry Mesh;

        public void DumpXML(string FileName)
        {
            Stream WriteStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            DumpXML(WriteStream);
            WriteStream.Close();
        }

        public void DumpXML(Stream WriteStream)
        {
            XmlTextWriter XWriter = new XmlTextWriter(WriteStream, null);
            XMLWriterInit(XWriter);
            DumpInXMLWriter(XWriter);
            XMLWriterFinalise(XWriter);
            XWriter.Close();
        }

        public static void XMLWriterInit(XmlTextWriter XWriter)
        {
            XWriter.Formatting = Formatting.Indented;
            XWriter.WriteStartDocument();
            XWriter.WriteComment("XMLSerialised XBIM geometry");
            XWriter.WriteStartElement("XBIMGeometry");
        }

        public static void XMLWriterFinalise(XmlTextWriter XWriter)
        {
            XWriter.WriteEndElement();
            XWriter.WriteEndDocument();
            XWriter.Close();
        }

        public void DumpInXMLWriter(XmlWriter XWriter)
        {
            XWriter.WriteStartElement("Mesh");
            if (EntityLabel != 0)
            {
                XWriter.WriteAttributeString("EntityLabel", EntityLabel.ToString());
            }
            WriteTransformMatrix(Matrix, XWriter);
            BoundingBox bb = WriteGeometry(Mesh, XWriter);
            if (!Matrix.IsIdentity && bb.IsValid)
            {
                BoundingBox Wbb = bb.TransformBy(Matrix);
                XWriter.WriteStartElement("WBB");
                XWriter.WriteAttributeString("MnX", Wbb.PointMin.X.ToString());
                XWriter.WriteAttributeString("MnY", Wbb.PointMin.Y.ToString());
                XWriter.WriteAttributeString("MnZ", Wbb.PointMin.Z.ToString());

                XWriter.WriteAttributeString("MxX", Wbb.PointMax.X.ToString());
                XWriter.WriteAttributeString("MxY", Wbb.PointMax.Y.ToString());
                XWriter.WriteAttributeString("MxZ", Wbb.PointMax.Z.ToString());
                XWriter.WriteEndElement();
            }

            XWriter.WriteEndElement();
        }

        private static void WriteTransformMatrix(Matrix3D m, XmlWriter bw)
        {
            bw.WriteStartElement("T");

            if (m.IsIdentity)
                bw.WriteAttributeString("value", "Identity");
            else
            {
                bw.WriteAttributeString("M11", m.M11.ToString());
                bw.WriteAttributeString("M12", m.M12.ToString());
                bw.WriteAttributeString("M13", m.M13.ToString());
                bw.WriteAttributeString("M14", m.M14.ToString());

                bw.WriteAttributeString("M21", m.M21.ToString());
                bw.WriteAttributeString("M22", m.M22.ToString());
                bw.WriteAttributeString("M23", m.M23.ToString());
                bw.WriteAttributeString("M24", m.M24.ToString());

                bw.WriteAttributeString("M31", m.M31.ToString());
                bw.WriteAttributeString("M32", m.M32.ToString());
                bw.WriteAttributeString("M33", m.M33.ToString());
                bw.WriteAttributeString("M34", m.M34.ToString());

                bw.WriteAttributeString("M41", m.OffsetX.ToString());
                bw.WriteAttributeString("M42", m.OffsetY.ToString());
                bw.WriteAttributeString("M43", m.OffsetZ.ToString());
                bw.WriteAttributeString("M44", m.M44.ToString());
            }
            bw.WriteEndElement();
        }

        public class BoundingBox
        {
            public bool IsValid = false;
            public Point3D PointMin = new Point3D();
            public Point3D PointMax = new Point3D();

            /// <summary>
            ///   Extends to include the specified point; returns true if boundaries were changed.
            /// </summary>
            /// <param name = "Point"></param>
            /// <returns></returns>
            internal bool IncludePoint(Point3D Point)
            {
                if (!IsValid)
                {
                    PointMin = Point;
                    PointMax = Point;
                    IsValid = true;
                    return true;
                }
                bool ret = false;

                if (PointMin.X > Point.X)
                {
                    PointMin.X = Point.X;
                    ret = true;
                }
                if (PointMin.Y > Point.Y)
                {
                    PointMin.Y = Point.Y;
                    ret = true;
                }
                if (PointMin.Z > Point.Z)
                {
                    PointMin.Z = Point.Z;
                    ret = true;
                }

                if (PointMax.X < Point.X)
                {
                    PointMax.X = Point.X;
                    ret = true;
                }
                if (PointMax.Y < Point.Y)
                {
                    PointMax.Y = Point.Y;
                    ret = true;
                }
                if (PointMax.Z < Point.Z)
                {
                    PointMax.Z = Point.Z;
                    ret = true;
                }

                IsValid = true;

                return ret;
            }

            internal void IncludeBoundingBox(BoundingBox childBB)
            {
                if (!childBB.IsValid)
                    return;
                this.IncludePoint(childBB.PointMin);
                this.IncludePoint(childBB.PointMax);
            }

            internal BoundingBox TransformBy(Matrix3D Matrix)
            {
                BoundingBox NewBB = new BoundingBox();
                // include all 8 vertices of the box.
                NewBB.IncludePoint(new Point3D(PointMin.X, PointMin.Y, PointMin.Z), Matrix);
                NewBB.IncludePoint(new Point3D(PointMin.X, PointMin.Y, PointMax.Z), Matrix);
                NewBB.IncludePoint(new Point3D(PointMin.X, PointMax.Y, PointMin.Z), Matrix);
                NewBB.IncludePoint(new Point3D(PointMin.X, PointMax.Y, PointMax.Z), Matrix);

                NewBB.IncludePoint(new Point3D(PointMax.X, PointMin.Y, PointMin.Z), Matrix);
                NewBB.IncludePoint(new Point3D(PointMax.X, PointMin.Y, PointMax.Z), Matrix);
                NewBB.IncludePoint(new Point3D(PointMax.X, PointMax.Y, PointMin.Z), Matrix);
                NewBB.IncludePoint(new Point3D(PointMax.X, PointMax.Y, PointMax.Z), Matrix);
                return NewBB;
            }

            private bool IncludePoint(Point3D Point, Matrix3D Matrix)
            {
                Point3D t = Point3D.Multiply(Point, Matrix);
                return IncludePoint(t);
            }
        }

        private static BoundingBox WriteGeometry(MeshGeometry Mesh, XmlWriter Writer)
        {
            BoundingBox bb = new BoundingBox();
            foreach (var item in Mesh.Children)
            {
                MeshGeometry m = item as MeshGeometry;
                if (m != null)
                {
                    Writer.WriteStartElement("Mesh");
                    BoundingBox childBB = WriteGeometry(m, Writer);
                    bb.IncludeBoundingBox(childBB);
                    Writer.WriteEndElement();
                }
            }
            if (Mesh.Positions.Count == Mesh.Normals.Count && Mesh.Positions.Count > 0)
            {
                for (int i = 0; i < Mesh.Normals.Count; i++)
                {
                    bb.IncludePoint(Mesh.Positions[i]);

                    Writer.WriteStartElement("PN");
                    Writer.WriteAttributeString("PX", Mesh.Positions[i].X.ToString());
                    Writer.WriteAttributeString("PY", Mesh.Positions[i].Y.ToString());
                    Writer.WriteAttributeString("PZ", Mesh.Positions[i].Z.ToString());

                    Writer.WriteAttributeString("NX", Mesh.Normals[i].X.ToString());
                    Writer.WriteAttributeString("NY", Mesh.Normals[i].Y.ToString());
                    Writer.WriteAttributeString("NZ", Mesh.Normals[i].Z.ToString());
                    Writer.WriteEndElement();
                }
                for (int i = 0; i < Mesh.TriangleIndices.Count;)
                {
                    Writer.WriteStartElement("F");
                    Writer.WriteAttributeString("I1", Mesh.TriangleIndices[i++].ToString());
                    Writer.WriteAttributeString("I2", Mesh.TriangleIndices[i++].ToString());
                    Writer.WriteAttributeString("I3", Mesh.TriangleIndices[i++].ToString());
                    Writer.WriteEndElement();
                }
            }
            if (bb.IsValid)
            {
                Writer.WriteStartElement("BB");
                Writer.WriteAttributeString("MnX", bb.PointMin.X.ToString());
                Writer.WriteAttributeString("MnY", bb.PointMin.Y.ToString());
                Writer.WriteAttributeString("MnZ", bb.PointMin.Z.ToString());

                Writer.WriteAttributeString("MxX", bb.PointMax.X.ToString());
                Writer.WriteAttributeString("MxY", bb.PointMax.Y.ToString());
                Writer.WriteAttributeString("MxZ", bb.PointMax.Z.ToString());
                Writer.WriteEndElement();
            }

            return bb;
        }
    }
}