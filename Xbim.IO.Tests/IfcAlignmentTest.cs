using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class IfcAlignmentTest
    {
        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcAlignmentSampleTest()
        {
            using (var store = IfcStore.Open("alignment.ifc"))
            {
                var lbl187 = store.Instances[187] as IIfcAlignment2DVerSegLine;
                {
                    Assert.IsNotNull(lbl187);
                    Assert.IsNull(lbl187.TangentialContinuity);
                    Assert.IsNull(lbl187.StartTag);
                    Assert.IsNull(lbl187.EndTag);
                    Assert.AreEqual(114.31055000000000632, (double)lbl187.StartDistAlong.Value, 0.0000000001);
                    Assert.AreEqual(0.46144999999999924967, (double)lbl187.HorizontalLength.Value, 0.0000000001);
                    Assert.AreEqual(120.92796229608872238, (double)lbl187.StartHeight.Value, 0.0000000001);
                    Assert.AreEqual(-0.017254948724066940940, (double)lbl187.StartGradient.Value, 0.0000000001);
                }

                var lbl176 = store.Instances[176] as IIfcCartesianPoint;
                {
                    Assert.IsNotNull(lbl176);
                    Assert.AreEqual(72.4044, lbl176.X);
                    Assert.AreEqual(-48.4366, lbl176.Y);
                    Assert.AreEqual(double.NaN, lbl176.Z);
                }

                var lbl175 = store.Instances[175] as IIfcCircularArcSegment2D;
                {
                    Assert.IsNotNull(lbl175);
                    Assert.AreEqual(6.1580586433410644531, lbl175.StartDirection, 0.0000000001);
                    Assert.AreEqual(14.428274131065547792, lbl175.SegmentLength, 0.0000000001);
                    Assert.AreEqual(23.499925494517583502, lbl175.Radius, 0.0000000001);
                    Assert.AreEqual(false, lbl175.IsCCW.Value);
                    Assert.AreSame(lbl176, lbl175.StartPoint);
                }

                var lbl174 = store.Instances[174] as IIfcAlignment2DHorizontalSegment;
                {
                    Assert.IsNotNull(lbl174);
                    Assert.IsNull(lbl174.TangentialContinuity);
                    Assert.IsNull(lbl174.StartTag);
                    Assert.IsNull(lbl174.EndTag);
                    Assert.AreSame(lbl174.CurveGeometry, lbl175);
                }

                var lbl46 = store.Instances[46] as IIfcCartesianPoint;
                {
                    Assert.IsNotNull(lbl46);
                    Assert.AreEqual(25.4971, lbl46.X);
                    Assert.AreEqual(-0.5315, lbl46.Y);
                    Assert.AreEqual(double.NaN, lbl46.Z);
                }

                var lbl45 = store.Instances[45] as IIfcCircularArcSegment2D;
                {
                    Assert.IsNotNull(lbl45);
                    Assert.AreEqual(0.99530634085600766525, lbl45.StartDirection, 0.0000000001);
                    Assert.AreEqual(31.415982774591739712, lbl45.SegmentLength, 0.0000000001);
                    Assert.AreEqual(15.000017896838162557, lbl45.Radius, 0.0000000001);
                    Assert.AreEqual(true, lbl45.IsCCW.Value);
                    Assert.AreSame(lbl45.StartPoint, lbl46);
                }

                var lbl44 = store.Instances[44] as IIfcAlignment2DHorizontalSegment;
                {
                    Assert.IsNotNull(lbl44);
                    Assert.IsNull(lbl44.TangentialContinuity);
                    Assert.IsNull(lbl44.StartTag);
                    Assert.IsNull(lbl44.EndTag);
                    Assert.AreSame(lbl44.CurveGeometry, lbl45);
                }

                var lbl43 = store.Instances[43] as IIfcCartesianPoint;
                {
                    Assert.IsNotNull(lbl43);
                    Assert.AreEqual(0, lbl43.X);
                    Assert.AreEqual(0, lbl43.Y);
                    Assert.AreEqual(double.NaN, lbl43.Z);
                }

                var lbl42 = store.Instances[42] as IIfcCircularArcSegment2D;
                {
                    Assert.IsNotNull(lbl42);
                    Assert.AreEqual(5.2461930831247052254, lbl42.StartDirection, 0.0000000001);
                    Assert.AreEqual(30.484556939386848740, lbl42.SegmentLength, 0.0000000001);
                    Assert.AreEqual(15.000040369074330471, lbl42.Radius, 0.0000000001);
                    Assert.AreSame(lbl43, lbl42.StartPoint);
                }

                var lbl41 = store.Instances[41] as IIfcAlignment2DHorizontalSegment;
                {
                    Assert.IsNotNull(lbl41);
                    Assert.IsNull(lbl41.TangentialContinuity);
                    Assert.IsNull(lbl41.StartTag);
                    Assert.IsNull(lbl41.EndTag);
                    Assert.AreSame(lbl42, lbl41.CurveGeometry);
                }

                var lbl40 = store.Instances[40] as IIfcAlignment2DHorizontal;
                {
                    Assert.IsNotNull(lbl40);
                    Assert.AreEqual(0, lbl40.StartDistAlong);

                    Assert.AreSame(lbl41, lbl40.Segments[0]);
                    Assert.AreSame(lbl44, lbl40.Segments[1]);
                }

                var lbl39 = store.Instances[39] as IIfcAlignment;
                {
                    Assert.IsNotNull(lbl39);
                    Assert.AreEqual("2awhANoPj79wbTFb_nA1Iy", lbl39.GlobalId.Value);
                    Assert.IsNull(lbl39.OwnerHistory);
                    Assert.IsNull(lbl39.Name);
                    Assert.AreEqual("KREIS1", lbl39.Description?.Value);
                }
            }
        }
    }
}