namespace Xbim.IO.Optimized
{
    public sealed partial class Scanner
    {
        private XbimScanBuffer _buffer;

        public void SetSource(XbimScanBuffer source)
        {
            _buffer = source;
            buffer = source;
            lNum = 0;
            code = '\n'; // to initialize yyline, yycol and lineStart
            GetCode();
        }

        public int yylabel
        {
            get
            {
                return _buffer.GetLabel(tokPos, tokEPos);
            }
        }
    }
}
