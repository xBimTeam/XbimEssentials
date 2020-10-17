using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common.Geometry;
using Xunit;

namespace Xbim.Essentials.Tests
{
    public class XbimMultiValueDictionaryTests
    {
        [Fact]
        public void Can_Sort()
        {
            var dict = new XbimMultiValueDictionary<int, XbimRect3D>
            {
                { 1, new XbimRect3D(0, 0, 0, 1, 1, 1) },
                { 2, new XbimRect3D(0, 0, 0, 3, 3, 3) },
                { 2, new XbimRect3D(0, 0, 0, 1, 1, 1) },
                { 3, new XbimRect3D(0, 0, 0, 2, 2, 2) },
                { 3, new XbimRect3D(0, 0, 0, 4, 4, 4) },
            };

            var sorted = dict.OrderByDescending(k => k.Value.First().Volume).ToList();

            sorted.Should().BeInDescendingOrder(new FirstVolumeComparer()  );
            sorted.First().Key.Should().Be(2);
            sorted.Last().Key.Should().Be(1);
        }

        private class FirstVolumeComparer : IComparer<KeyValuePair<int, ICollection<XbimRect3D>>>
        {
            public int Compare(KeyValuePair<int, ICollection<XbimRect3D>> x, KeyValuePair<int, ICollection<XbimRect3D>> y)
            {
                return (int)(x.Value.FirstOrDefault().Volume - y.Value.FirstOrDefault().Volume);
            }
        }
    }
}
