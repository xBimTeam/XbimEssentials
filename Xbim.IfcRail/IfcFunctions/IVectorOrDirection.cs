namespace Xbim.IfcRail
{
    internal interface IVectorOrDirection
    {
        int Dim { get; set; }
        double[] DirectionRatios { get; }
    }
}