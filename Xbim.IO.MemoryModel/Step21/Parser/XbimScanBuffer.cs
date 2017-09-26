using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IO.Parser
{
    public class XbimScanBuffer : ScanBuff
    {
        private Stream _stream;
        private byte[] _buffer = new byte[0x8000];
        private byte[] _prevBuffer = new byte[0x8000];
        private int _bufferOffset;
        private int _bufferPosition;
        private int _prevBufferPosition;
        private Encoding _encoding = Encoding.ASCII;
        private const byte hash = (byte)'#';
        private const byte zero = (byte)'0';
        private const byte markOffset = 5;

        public XbimScanBuffer(Stream stream)
        {
            _stream = stream;
        }

        public override int Pos
        {
            get => _bufferOffset + _bufferPosition;
            set => _bufferPosition = value - _bufferOffset;
        }

        public override string GetString(int begin, int limit)
        {
            var data = GetSubArray(begin, limit);
            var result = _encoding.GetString(data);
            return result;
        }

        public int GetLabel(int begin, int limit)
        {
            var data = GetSubArray(begin, limit);
            var label = 0;
            var order = 0;
            // iterate from the end, skip thi first character '#'
            for (int i = data.Length - 1; i > 0; i--)
            {
                var component = data[i] - zero;
                label += component * _magnitudes[order++];
            }
            return label;
        }

        private byte[] GetSubArray(int begin, int limit)
        {
            var start = begin - _bufferOffset;
            var length = limit - begin;
            var result = new byte[length];

            if (start > 0)
            {
                Array.ConstrainedCopy(_buffer, start, result, 0, length);
                return result;
            }

            var prevStart = _prevBufferPosition + start;
            var prevLength = Math.Abs(start);
            Array.ConstrainedCopy(_prevBuffer, prevStart, result, 0, prevLength);

            length = length + start;
            start = 0;
            Array.ConstrainedCopy(_buffer, start, result, prevLength, length);

            return result;

        }

        private int[] _magnitudes = new int[]
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };

        public override int Read()
        {
            //if position was set to the point before current position in the stream, read the data from buffer again
            if (Pos < _stream.Position)
            {
                if (_bufferPosition > 0)
                {
                    return _buffer[_bufferPosition++];
                }
                else
                {
                    var index = _prevBufferPosition + _bufferPosition;
                    _bufferPosition++;
                    return _prevBuffer[index];

                }
            }

            var value = _stream.ReadByte();

            // overflowing buffer capacity, need to extend buffer
            if (_bufferPosition == _buffer.Length)
            {
                // double the size
                Array.Resize(ref _buffer, _buffer.Length * 2);
            }

            // check for EOF
            if (value > -1)
            {
                _buffer[_bufferPosition++] = (byte)value;
            }

            return value;
        }

        public override void Mark()
        {
            if (_bufferPosition < markOffset)
                return;

            // swap buffers
            var temp = _prevBuffer;
            _prevBuffer = _buffer;
            _buffer = temp;


            _bufferOffset += _bufferPosition - markOffset;
            _prevBufferPosition = _bufferPosition - markOffset;
            _bufferPosition = markOffset;

            //copy mark offset data to avoid data reading using both previous and current buffer most of the time
            for (int i = 0; i < markOffset; i++)
                _buffer[i] = _prevBuffer[_prevBufferPosition + i];
        }
    }
}
