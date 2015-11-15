namespace Xbim.Common.Geometry
{
    public interface IXbimShapeInstanceData
    {
        /// <summary>
        /// The unique label of this shape instance
        /// </summary>
        int InstanceLabel { get; set; }
        /// <summary>
        /// The IFC type of the product this instance represents
        /// </summary>
        short IfcTypeId { get; set; }
        /// <summary>
        /// The label of the IFC Product object that  this instance fully or partly defines
        /// </summary>
        int IfcProductLabel { get; set; }
        /// <summary>
        /// The style that this shape is presented in when it overrides the shape style
        /// </summary>
        int StyleLabel { get; set; }
        /// <summary>
        /// The id of the shape geometry  that this is an instance of
        /// </summary>
        int ShapeGeometryLabel { get; set; }
        /// <summary>
        /// The label of the IFC representation context of this instance
        /// </summary>
        int RepresentationContext { get; set; }
        /// <summary>
        /// What type of representation, typically this is how the shape has been generated, i.e. openings have been applied or not applied
        /// </summary>
        byte RepresentationType { get; set; }
        /// <summary>
        /// The transformation to be applied to shape to place it in the world coordinates
        /// </summary>
        byte[] Transformation { get; set; }
        /// <summary>
        /// The bounding box of this instance, does not require tranformation to place in world coordinates
        /// </summary>
        byte[] BoundingBox { get; set; }
       
    }
}
