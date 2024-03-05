using System;
using System.IO;

namespace Xbim.Common.Model
{
    // Based on https://stackoverflow.com/a/28036366
    /// <summary>
    /// A Stream implementation that wraps an underlying non-seekable stream to provide a limited 'back buffer'
    /// which enables the content to be seekable within the buffer window.
    /// </summary>
    /// <remarks>Useful to be able to read a header of a file when an input stream is not seekable - e.g. a NetworkStream</remarks>
    public class ReadSeekableStream : Stream
    {
        private long _underlyingPosition;
        private readonly byte[] _seekBackBuffer;
        private int _seekBackBufferCount;
        private int _seekBackBufferIndex;
        private readonly Stream _underlyingStream;
        

        /// <summary>
        /// Constructs a new <see cref="ReadSeekableStream"/> with the provided inner stream and buffer size
        /// </summary>
        /// <param name="underlyingStream"></param>
        /// <param name="seekBackBufferSize"></param>
        /// <exception cref="Exception"></exception>
        public ReadSeekableStream(Stream underlyingStream, int seekBackBufferSize)
        {
            if (!underlyingStream.CanRead)
                throw new Exception($"Provided stream {underlyingStream} is not readable");

            if (underlyingStream.CanSeek)
                throw new Exception($"Provided stream {underlyingStream} is already Seekable, and will not benefit from this wrapper");
            if (underlyingStream is null)
            {
                throw new ArgumentNullException(nameof(underlyingStream));
            }

            if (seekBackBufferSize <= 0)
            {
                throw new Exception("Buffer size must be a positive");
            }

            _underlyingStream = underlyingStream;
            _seekBackBuffer = new byte[seekBackBufferSize];
            BufferEnabled = true;
        }

        /// <summary>
        /// Disables any further back buffering. Once disabled Seeking is no longer possible.
        /// </summary>
        /// <remarks>This allows you to eliminate the overhead of the buffering once you know you do not need to Seek any further.</remarks>
        public void DisableBuffering()
        {
            BufferEnabled = false;
        }

        /// <summary>
        /// Indicates whether the buffer is active
        /// </summary>
        public bool BufferEnabled { get; private set; }

        /// <inheritDoc/>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// Indicates whether the stream supports Seeking. Note: this can change if Buffering is disabled.
        /// </summary>
        public override bool CanSeek { get { return BufferEnabled; } }

        /// <inheritDoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int copiedFromBackBufferCount = 0;
            if (_seekBackBufferIndex < _seekBackBufferCount)
            {
                // Read from buffer
                copiedFromBackBufferCount = Math.Min(count, _seekBackBufferCount - _seekBackBufferIndex);
                Buffer.BlockCopy(_seekBackBuffer, _seekBackBufferIndex, buffer, offset, copiedFromBackBufferCount);
                offset += copiedFromBackBufferCount;
                count -= copiedFromBackBufferCount;
                _seekBackBufferIndex += copiedFromBackBufferCount;
            }
            int bytesReadFromUnderlying = 0;
            if (count > 0)
            {
                // Read from underlying and fill buffer
                bytesReadFromUnderlying = _underlyingStream.Read(buffer, offset, count);
                if (BufferEnabled && bytesReadFromUnderlying > 0)
                {
                    _underlyingPosition += bytesReadFromUnderlying;

                    var copyToBufferCount = Math.Min(bytesReadFromUnderlying, _seekBackBuffer.Length);
                    var copyToBufferOffset = Math.Min(_seekBackBufferCount, _seekBackBuffer.Length - copyToBufferCount);
                    var bufferBytesToMove = Math.Min(_seekBackBufferCount - 1, copyToBufferOffset);

                    if (bufferBytesToMove > 0)
                        Buffer.BlockCopy(_seekBackBuffer, _seekBackBufferCount - bufferBytesToMove, _seekBackBuffer, 0, bufferBytesToMove);
                    Buffer.BlockCopy(buffer, offset, _seekBackBuffer, copyToBufferOffset, copyToBufferCount);
                    _seekBackBufferCount = Math.Min(_seekBackBuffer.Length, _seekBackBufferCount + copyToBufferCount);
                    _seekBackBufferIndex = _seekBackBufferCount;
                }
            }
            return copiedFromBackBufferCount + bytesReadFromUnderlying;
        }

        /// <summary>
        /// Sets the position within the current stream. Note: seeking backward is only possible within the range of the buffer window
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when seeking beyond the beginning of the buffer or past the end of the stream</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if(!BufferEnabled)
            {
                throw new NotSupportedException("Cannot seek when back buffer is disabled");
            }
            if (origin == SeekOrigin.End)
                return SeekFromEnd((int)Math.Max(0, -offset));

            var relativeOffset = origin == SeekOrigin.Current
                ? offset
                : offset - Position;

            if (relativeOffset == 0)
                return Position;
            else if (relativeOffset > 0)
                return SeekForward(relativeOffset);
            else
                return SeekBackwards(-relativeOffset);
        }

        private long SeekForward(long origOffset)
        {
            long offset = origOffset;
            var seekBackBufferLength = _seekBackBuffer.Length;

            int backwardSoughtBytes = _seekBackBufferCount - _seekBackBufferIndex;
            int seekForwardInBackBuffer = (int)Math.Min(offset, backwardSoughtBytes);
            offset -= seekForwardInBackBuffer;
            _seekBackBufferIndex += seekForwardInBackBuffer;

            if (offset > 0)
            {
                // first completely fill seekBackBuffer to remove special cases from while loop below
                if (_seekBackBufferCount < seekBackBufferLength)
                {
                    var maxRead = seekBackBufferLength - _seekBackBufferCount;
                    if (offset < maxRead)
                        maxRead = (int)offset;
                    var bytesRead = _underlyingStream.Read(_seekBackBuffer, _seekBackBufferCount, maxRead);
                    _underlyingPosition += bytesRead;
                    _seekBackBufferCount += bytesRead;
                    _seekBackBufferIndex = _seekBackBufferCount;
                    if (bytesRead < maxRead)
                    {
                        if (_seekBackBufferCount < offset)
                            throw new NotSupportedException("Reached end of stream seeking forward " + origOffset + " bytes");
                        return Position;
                    }
                    offset -= bytesRead;
                }

                // now alternate between filling tempBuffer and seekBackBuffer
                bool fillTempBuffer = true;
                var tempBuffer = new byte[seekBackBufferLength];
                while (offset > 0)
                {
                    var maxRead = offset < seekBackBufferLength ? (int)offset : seekBackBufferLength;
                    var bytesRead = _underlyingStream.Read(fillTempBuffer ? tempBuffer : _seekBackBuffer, 0, maxRead);
                    _underlyingPosition += bytesRead;
                    var bytesReadDiff = maxRead - bytesRead;
                    offset -= bytesRead;
                    if (bytesReadDiff > 0 /* reached end-of-stream */ || offset == 0)
                    {
                        if (fillTempBuffer)
                        {
                            if (bytesRead > 0)
                            {
                                Buffer.BlockCopy(_seekBackBuffer, bytesRead, _seekBackBuffer, 0, bytesReadDiff);
                                Buffer.BlockCopy(tempBuffer, 0, _seekBackBuffer, bytesReadDiff, bytesRead);
                            }
                        }
                        else
                        {
                            if (bytesRead > 0)
                                Buffer.BlockCopy(_seekBackBuffer, 0, _seekBackBuffer, bytesReadDiff, bytesRead);
                            Buffer.BlockCopy(tempBuffer, bytesRead, _seekBackBuffer, 0, bytesReadDiff);
                        }
                        if (offset > 0)
                            throw new NotSupportedException("Reached end of stream seeking forward " + origOffset + " bytes");
                    }
                    fillTempBuffer = !fillTempBuffer;
                }
            }
            return Position;
        }

        private long SeekBackwards(long offset)
        {
            var intOffset = (int)offset;
            if (offset > int.MaxValue || intOffset > _seekBackBufferIndex)
                throw new NotSupportedException("Cannot currently seek backwards more than " + _seekBackBufferIndex + " bytes");
            _seekBackBufferIndex -= intOffset;
            return Position;
        }

        private long SeekFromEnd(long offset)
        {
            var intOffset = (int)offset;
            var seekBackBufferLength = _seekBackBuffer.Length;
            if (offset > int.MaxValue || intOffset > seekBackBufferLength)
                throw new NotSupportedException("Cannot seek backwards from end more than " + seekBackBufferLength + " bytes");

            // first completely fill seekBackBuffer to remove special cases from while loop below
            if (_seekBackBufferCount < seekBackBufferLength)
            {
                var maxRead = seekBackBufferLength - _seekBackBufferCount;
                var bytesRead = _underlyingStream.Read(_seekBackBuffer, _seekBackBufferCount, maxRead);
                _underlyingPosition += bytesRead;
                _seekBackBufferCount += bytesRead;
                _seekBackBufferIndex = Math.Max(0, _seekBackBufferCount - intOffset);
                if (bytesRead < maxRead)
                {
                    if (_seekBackBufferCount < intOffset)
                        throw new NotSupportedException("Could not seek backwards from end " + intOffset + " bytes");
                    return Position;
                }
            }
            else
            {
                _seekBackBufferIndex = _seekBackBufferCount;
            }

            // now alternate between filling tempBuffer and seekBackBuffer
            bool fillTempBuffer = true;
            var tempBuffer = new byte[seekBackBufferLength];
            while (true)
            {
                var bytesRead = _underlyingStream.Read(fillTempBuffer ? tempBuffer : _seekBackBuffer, 0, seekBackBufferLength);
                _underlyingPosition += bytesRead;
                var bytesReadDiff = seekBackBufferLength - bytesRead;
                if (bytesReadDiff > 0) // reached end-of-stream
                {
                    if (fillTempBuffer)
                    {
                        if (bytesRead > 0)
                        {
                            Buffer.BlockCopy(_seekBackBuffer, bytesRead, _seekBackBuffer, 0, bytesReadDiff);
                            Buffer.BlockCopy(tempBuffer, 0, _seekBackBuffer, bytesReadDiff, bytesRead);
                        }
                    }
                    else
                    {
                        if (bytesRead > 0)
                            Buffer.BlockCopy(_seekBackBuffer, 0, _seekBackBuffer, bytesReadDiff, bytesRead);
                        Buffer.BlockCopy(tempBuffer, bytesRead, _seekBackBuffer, 0, bytesReadDiff);
                    }
                    _seekBackBufferIndex -= intOffset;
                    return Position;
                }
                fillTempBuffer = !fillTempBuffer;
            }
        }

        public override long Position
        {
            get { return _underlyingPosition - (_seekBackBufferCount - _seekBackBufferIndex); }
            set { Seek(value, SeekOrigin.Begin); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _underlyingStream.Close();
            base.Dispose(disposing);
        }

        public override bool CanTimeout { get { return _underlyingStream.CanTimeout; } }
        public override bool CanWrite { get { return _underlyingStream.CanWrite; } }
        public override long Length { get { return _underlyingStream.Length; } }
        public override void SetLength(long value) { _underlyingStream.SetLength(value); }
        public override void Write(byte[] buffer, int offset, int count) { _underlyingStream.Write(buffer, offset, count); }
        public override void Flush() { _underlyingStream.Flush(); }
    }
}
