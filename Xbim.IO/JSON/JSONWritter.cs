using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Xbim.IO.JSON
{
    public class JSONWritter: XmlWriter
    {
        private readonly TextWriter _writer;
        private int _depth;
        private readonly Stack<int> _count = new Stack<int>();
        private string _indentation = "";
        private bool _afterLastValue;
        private int _lastCount;
        public int Depth
        {
            get { return _depth; }
            set
            {
                if (value > _depth)
                {
                    _count.Push(0);
                    _afterLastValue = false;
                }
                if (value < _depth)
                {
                    _lastCount = _count.Pop();
                    _afterLastValue = true;
                }

                _depth = value;
                if (Settings == null) return;
                if (!Settings.Indent) return;

                //reset indentation string
                _indentation = "";
                for (int i = 0; i < _depth; i++)
                {
                    _indentation += Settings.IndentChars;
                }
            }
        }

        private int Count
        {
            get { return _count.Count > 0 ? _count.Peek() : 0; }
        }

        public JSONWritter(TextWriter writer, XmlWriterSettings settings = null)
        {
            _writer = writer;
            _settings = settings ?? new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                CloseOutput = true
            };
            _writeState = WriteState.Start;
        }

        public override XmlWriterSettings Settings
        {
            get { return _settings; }
        }

        private void Indent()
        {
            if (!_afterLastValue || _lastCount == 0)
                _writer.Write(Count > 0 ? "," : "{");
            _afterLastValue = false;

            NewLine();
            _writer.Write(_indentation);
        }

        private bool _anyContent;

        private void Write(string data)
        {
            if(_writeState == WriteState.Closed)
                throw new Exception("Document closed already");

            _writer.Write(data);
            _anyContent = true;
        }

        private void NewLine()
        {
            if (_writeState == WriteState.Closed)
                throw new Exception("Document closed already");

            if (Settings == null) return;
            _writer.Write(Settings.NewLineChars);
            
            if(_count.Count > 0)
                _count.Push(_count.Pop() + 1);
        }

        private WriteState _writeState;
        private readonly XmlWriterSettings _settings;

        public override void WriteStartDocument()
        {
            //Write("{");
            Depth++;
            _anyContent = false;
        }

        public override void WriteStartDocument(bool standalone)
        {
            WriteStartDocument();
        }

        public override void WriteEndDocument()
        {
            
            Depth--;
            if (Depth != 0)
                throw new FormatException("Document is not well formatted");
            Indent();
            Write("}");
            _writeState = WriteState.Closed;
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
        }


        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            _writeState = WriteState.Element;
            Indent();
            Write(string.Format("\"{0}\":", localName));
            Depth++;
            _anyContent = false;
        }

        public override void WriteEndElement()
        {
            if(_writeState != WriteState.Element)
                throw new FormatException("Can't close element when not inside.");

            Depth--;

            if (!_anyContent)
            {
                Write("null");
            }

            if (_lastCount != 0)
            {
                Indent();
                Write("}");
            }
        }

        public override void WriteFullEndElement()
        {
            WriteEndElement();
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            _writeState = WriteState.Attribute;
            Indent();
            Write(string.Format("\"{0}\":", localName));
            _anyContent = true;
        }

        public override void WriteEndAttribute()
        {
            if(_writeState != WriteState.Attribute)
                throw new FormatException("Can't end attribute when not inside");

            if (!_anyContent)
                Write("null");

            _writeState = WriteState.Element;
        }

        public override void WriteCData(string text)
        {
            
        }

        public override void WriteComment(string text)
        {
            Indent();
            Write("/*" + text + "*/");
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
        }

        public override void WriteEntityRef(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteCharEntity(char ch)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteWhitespace(string ws)
        {
            _writer.Write(ws);
            _anyContent = true;
        }

        public override void WriteString(string text)
        {
            _writer.Write("\"{0}\"", text);
            _anyContent = true;
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            var data = new char[count];
            Array.Copy(buffer, index, data, 0, count);
            var str = new string(data);
            WriteString(str);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            var data = new char[count];
            Array.Copy(buffer, index, data, 0, count);
            var str = new string(data);
            _writer.Write(str);
            _anyContent = true;
        }

        public override void WriteRaw(string data)
        {
            _writer.Write(data);
            _anyContent = true;
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            var str = Convert.ToBase64String(buffer, index, count, Base64FormattingOptions.None);
            WriteString(str);
        }

        public override void Close()
        {
            if (Settings == null)
            {
                _writer.Close();
                return;
            }

            if (Settings.CloseOutput)
                _writer.Close();
        }

        public override void Flush()
        {
            _writer.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return null;
        }

        public override WriteState WriteState
        {
            get { return _writeState; }
        }

        public override void WriteValue(bool value)
        {
            _writer.Write(value ? "true" : "false");
            _anyContent = true;
        }

        public override void WriteValue(double value)
        {
            _writer.Write(value.ToString("G"));
            _anyContent = true;
        }

        public override void WriteValue(float value)
        {
            _writer.Write(value.ToString("G"));
            _anyContent = true;
        }

        public override void WriteValue(int value)
        {
            _writer.Write(value);
            _anyContent = true;
        }

        public override void WriteValue(long value)
        {
            _writer.Write(value);
            _anyContent = true;
        }
    }
}
