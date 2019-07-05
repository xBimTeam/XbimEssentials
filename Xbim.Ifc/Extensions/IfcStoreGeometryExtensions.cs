using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IfcStoreGeometryExtensions
    {
        public const int WexBimId = 94132117;       // Magic

        /// <summary>
        /// This function is used to generate the .wexbim model files.
        /// </summary>
        /// <param name="model">The model to export</param>
        /// <param name="binaryStream">An open writable streamer.</param>
        /// <param name="products">Optional products to be written to the wexBIM file. If null, all products from the model will be saved</param>
        /// <param name="translation">Optional 3D vector to apply</param>
        public static void SaveAsWexBim(this IModel model, BinaryWriter binaryStream, IEnumerable<IIfcProduct> products = null, 
            IVector3D translation = null)
        {
            products = products ?? model.Instances.OfType<IIfcProduct>();
            // ReSharper disable RedundantCast
            if (model.GeometryStore == null) throw new XbimException("Geometry store has not been initialised");
            // ReSharper disable once CollectionNeverUpdated.Local
            var colourMap = new XbimColourMap();
            using (var geomRead = model.GeometryStore.BeginRead())
            {

                var lookup = geomRead.ShapeGeometries;
                var styles = geomRead.StyleIds;
                var regions = geomRead.ContextRegions.SelectMany(r => r).ToList();
                //we need to get all the default styles for various products
                var defaultStyles = geomRead.ShapeInstances.Select(i => -(int)i.IfcTypeId).Distinct();
                var allStyles = defaultStyles.Concat(styles).ToList();
                int numberOfGeometries = 0;
                int numberOfVertices = 0;
                int numberOfTriangles = 0;
                int numberOfMatrices = 0;
                int numberOfProducts = 0;
                int numberOfStyles = allStyles.Count;
                //start writing out

                binaryStream.Write((Int32)WexBimId); //magic number

                binaryStream.Write((byte)2); //version of stream, arrays now packed as doubles
                var start = (int)binaryStream.Seek(0, SeekOrigin.Current);
                binaryStream.Write((Int32)0); //number of shapes
                binaryStream.Write((Int32)0); //number of vertices
                binaryStream.Write((Int32)0); //number of triangles
                binaryStream.Write((Int32)0); //number of matrices
                binaryStream.Write((Int32)0); //number of products
                binaryStream.Write((Int32)numberOfStyles); //number of styles
                binaryStream.Write(Convert.ToSingle(model.ModelFactors.OneMetre));
                //write out conversion to meter factor

                binaryStream.Write(Convert.ToInt16(regions.Count)); //write out the population data
                var t = XbimMatrix3D.Identity;
                t = Translate(t, translation);
                foreach (var r in regions)
                {
                    binaryStream.Write((Int32)(r.Population));
                    var bounds = r.ToXbimRect3D();
                    var centre = t.Transform(r.Centre);
                    //write out the centre of the region
                    binaryStream.Write((Single)centre.X);
                    binaryStream.Write((Single)centre.Y);
                    binaryStream.Write((Single)centre.Z);
                    //bounding box of largest region
                    binaryStream.Write(bounds.ToFloatArray());
                }
                //textures

                foreach (var styleId in allStyles)
                {
                    XbimColour colour;
                    if (styleId > 0)
                    {
                        var ss = (IIfcSurfaceStyle)model.Instances[styleId];
                        var texture = XbimTexture.Create(ss);
                        colour = texture.ColourMap.FirstOrDefault();
                    }
                    else //use the default in the colour map for the enetity type
                    {
                        var theType = model.Metadata.GetType((short)Math.Abs(styleId));
                        colour = colourMap[theType.Name];
                    }
                    if (colour == null) colour = XbimColour.DefaultColour;
                    binaryStream.Write((Int32)styleId); //style ID                       
                    binaryStream.Write((Single)colour.Red);
                    binaryStream.Write((Single)colour.Green);
                    binaryStream.Write((Single)colour.Blue);
                    binaryStream.Write((Single)colour.Alpha);

                }

                //write out all the product bounding boxes
                var prodIds = new HashSet<int>();
                foreach (var product in products)
                {
                    if (product is IIfcFeatureElement) continue;
                    prodIds.Add(product.EntityLabel);

                    var bb = XbimRect3D.Empty;
                    foreach (var si in geomRead.ShapeInstancesOfEntity(product))
                    {
                        var transformation = Translate(si.Transformation, translation);
                        var bbPart = XbimRect3D.TransformBy(si.BoundingBox, transformation);
                        //make sure we put the box in the right place and then convert to axis aligned
                        if (bb.IsEmpty) bb = bbPart;
                        else
                            bb.Union(bbPart);
                    }
                    //do not write out anything with no geometry
                    if (bb.IsEmpty) continue;

                    binaryStream.Write((Int32)product.EntityLabel);
                    binaryStream.Write((UInt16)model.Metadata.ExpressTypeId(product));
                    binaryStream.Write(bb.ToFloatArray());
                    numberOfProducts++;
                }

                //projections and openings have already been applied, 

                var toIgnore = new short[4];
                toIgnore[0] = model.Metadata.ExpressTypeId("IFCOPENINGELEMENT");
                toIgnore[1] = model.Metadata.ExpressTypeId("IFCPROJECTIONELEMENT");
                if (model.SchemaVersion == XbimSchemaVersion.Ifc4 || model.SchemaVersion == XbimSchemaVersion.Ifc4x1)
                {
                    toIgnore[2] = model.Metadata.ExpressTypeId("IFCVOIDINGFEATURE");
                    toIgnore[3] = model.Metadata.ExpressTypeId("IFCSURFACEFEATURE");
                }

                foreach (var geometry in lookup)
                {
                    if (geometry.ShapeData.Length <= 0) //no geometry to display so don't write out any products for it
                        continue;
                    var instances = geomRead.ShapeInstancesOfGeometry(geometry.ShapeLabel);



                    var xbimShapeInstances = instances.Where(si => !toIgnore.Contains(si.IfcTypeId) &&
                                                                 si.RepresentationType ==
                                                                 XbimGeometryRepresentationType
                                                                     .OpeningsAndAdditionsIncluded && prodIds.Contains(si.IfcProductLabel)).ToList();
                    if (!xbimShapeInstances.Any()) continue;
                    numberOfGeometries++;
                    binaryStream.Write(xbimShapeInstances.Count); //the number of repetitions of the geometry
                    if (xbimShapeInstances.Count > 1)
                    {
                        foreach (IXbimShapeInstanceData xbimShapeInstance in xbimShapeInstances)
                        //write out each of the ids style and transforms
                        {
                            binaryStream.Write(xbimShapeInstance.IfcProductLabel);
                            binaryStream.Write((UInt16)xbimShapeInstance.IfcTypeId);
                            binaryStream.Write((UInt32)xbimShapeInstance.InstanceLabel);
                            binaryStream.Write((Int32)xbimShapeInstance.StyleLabel > 0
                                ? xbimShapeInstance.StyleLabel
                                : xbimShapeInstance.IfcTypeId * -1);

                            var transformation = Translate(XbimMatrix3D.FromArray(xbimShapeInstance.Transformation), translation);
                            binaryStream.Write(transformation.ToArray());
                            numberOfTriangles +=
                                XbimShapeTriangulation.TriangleCount(((IXbimShapeGeometryData)geometry).ShapeData);
                            numberOfMatrices++;
                        }
                        numberOfVertices +=
                            XbimShapeTriangulation.VerticesCount(((IXbimShapeGeometryData)geometry).ShapeData);
                        // binaryStream.Write(geometry.ShapeData);
                        var ms = new MemoryStream(((IXbimShapeGeometryData)geometry).ShapeData);
                        var br = new BinaryReader(ms);
                        var tr = br.ReadShapeTriangulation();

                        tr.Write(binaryStream);
                    }
                    else //now do the single instances
                    {
                        var xbimShapeInstance = xbimShapeInstances[0];

                        // IXbimShapeGeometryData geometry = ShapeGeometry(kv.Key);
                        binaryStream.Write((Int32)xbimShapeInstance.IfcProductLabel);
                        binaryStream.Write((UInt16)xbimShapeInstance.IfcTypeId);
                        binaryStream.Write((Int32)xbimShapeInstance.InstanceLabel);
                        binaryStream.Write((Int32)xbimShapeInstance.StyleLabel > 0
                            ? xbimShapeInstance.StyleLabel
                            : xbimShapeInstance.IfcTypeId * -1);

                        //Read all vertices and normals in the geometry stream and transform

                        var ms = new MemoryStream(((IXbimShapeGeometryData)geometry).ShapeData);
                        var br = new BinaryReader(ms);
                        var tr = br.ReadShapeTriangulation();
                        var transformation = Translate(xbimShapeInstance.Transformation, translation);
                        var trTransformed = tr.Transform(transformation);
                        trTransformed.Write(binaryStream);
                        numberOfTriangles += XbimShapeTriangulation.TriangleCount(((IXbimShapeGeometryData)geometry).ShapeData);
                        numberOfVertices += XbimShapeTriangulation.VerticesCount(((IXbimShapeGeometryData)geometry).ShapeData);
                    }
                }


                binaryStream.Seek(start, SeekOrigin.Begin);
                binaryStream.Write((Int32)numberOfGeometries);
                binaryStream.Write((Int32)numberOfVertices);
                binaryStream.Write((Int32)numberOfTriangles);
                binaryStream.Write((Int32)numberOfMatrices);
                binaryStream.Write((Int32)numberOfProducts);
                binaryStream.Seek(0, SeekOrigin.End); //go back to end
                // ReSharper restore RedundantCast
            }
        }

        /// <summary>
        /// If translation is defined, returns matrix translated by the vector
        /// </summary>
        /// <param name="matrix">Input matrix</param>
        /// <param name="translation">Translation</param>
        /// <returns>Translated matrix</returns>
        private static XbimMatrix3D Translate(XbimMatrix3D matrix, IVector3D translation)
        {
            if (translation == null) return matrix;
            var translationMatrix = XbimMatrix3D.CreateTranslation(translation.X, translation.Y, translation.Z);
            return XbimMatrix3D.Multiply(matrix, translationMatrix);
        }

    }
}
