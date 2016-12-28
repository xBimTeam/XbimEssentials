namespace Xbim.Ifc4
{
    internal interface IVectorOrDirection
    {
        int Dim { get; set; }
        double[] DirectionRatios { get; }
    }
}