using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Model;
using Xbim.Ifc2x3;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ReadSeekableStreamTests
    {
        [TestMethod]
        public void CanOpenFromNonSeekableStream()
        {
            // Tests our NonSeekable fake stream
            const string ifcPath = @"TestFiles/SmallModelIfc2x3.ifc";
            using var fileStream = File.Open(ifcPath, FileMode.Open);
            using var stream = new NonSeekableStream(fileStream);
            Assert.IsFalse(stream.CanSeek);

            using var model = MemoryModel.OpenReadStep21(stream);

            Assert.AreNotEqual(0, model.Instances.Count);
            

        }

        [TestMethod]
        public void MemoryModelThrowsWhenNonSeekableStreamUsed()
        {
            // Simulate the user reading the header from the stream, before passing to the parser - without our buffered Stream

            const string ifcPath = @"TestFiles/SmallModelIfc2x3.ifc";
            using var fileStream = File.Open(ifcPath, FileMode.Open);
            using var stream = new NonSeekableStream(fileStream);
            Assert.IsFalse(stream.CanSeek);

            var schema = ModelHelper.GetStepFileSchemaVersion(stream);

            stream.Seek(0, SeekOrigin.Begin);   // Will be ignored.

            var ex = Assert.ThrowsException<XbimParserException>(() =>  MemoryModel.OpenReadStep21(stream));

            ex.Message.Should().StartWith("IFC Schema could not be read from Header");

        }

        [TestMethod]
        public void CanReseekAndOpenFromNonSeekableStream()
        {

            const string ifcPath = @"TestFiles/SmallModelIfc2x3.ifc";

            using var fileStream = File.Open(ifcPath, FileMode.Open);
            using var nonseekableStream = new NonSeekableStream(fileStream);
            Assert.IsFalse(nonseekableStream.CanSeek);
            using var stream = new ReadSeekableStream(nonseekableStream, 4096);    // 4KB Buffer
            Assert.IsTrue(stream.CanSeek);
            var schema = ModelHelper.GetStepFileSchemaVersion(stream);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(0, stream.Position);

            var mm = new MemoryModel(new EntityFactoryIfc2x3());

            using var model = MemoryModel.OpenReadStep21(stream);
            Assert.AreNotEqual(0, model.Instances.Count);
        }

        [TestMethod]
        public void CanDisableBuffer()
        {

            const string ifcPath = @"TestFiles/SmallModelIfc2x3.ifc";

            using var fileStream = File.Open(ifcPath, FileMode.Open);
            using var nonseekableStream = new NonSeekableStream(fileStream);
            using var stream = new ReadSeekableStream(nonseekableStream, 4096);
            var schema = ModelHelper.GetStepFileSchemaVersion(stream);
            stream.Seek(0, SeekOrigin.Begin);

            stream.DisableBuffering();  // No future Reads are cached

            var mm = new MemoryModel(new EntityFactoryIfc2x3());

            using var model = MemoryModel.OpenReadStep21(stream);
            Assert.AreNotEqual(0, model.Instances.Count);
        }


        [TestMethod]
        public void DisablingBufferPreventsSeek()
        {

            const string ifcPath = @"TestFiles/SmallModelIfc2x3.ifc";

            using var fileStream = File.Open(ifcPath, FileMode.Open);
            using var nonseekableStream = new NonSeekableStream(fileStream);
            using var stream = new ReadSeekableStream(nonseekableStream, 4096);    // 4KB Buffer
            
            // Makes the Stream useless
            stream.DisableBuffering();
            var schema = ModelHelper.GetStepFileSchemaVersion(stream);
            var ex = Assert.ThrowsException<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));

            ex.Message.Should().Be("Cannot seek when back buffer is disabled");
        }

    }

    /// <summary>
    /// Simulates a non-seekable stream such as a NetworkStream 
    /// </summary>
    public class NonSeekableStream : Stream
    {
        private readonly Stream innerStream;

        public NonSeekableStream(Stream innerStream)
        {
            this.innerStream = innerStream;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => 0;

        public override long Position { get => innerStream.Position; set => throw new System.NotImplementedException(); }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Position;    // i.e. silently ignore Seeking
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}
