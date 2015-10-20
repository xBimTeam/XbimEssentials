#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    StepP21Lex.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#define BACKUP
#define BYTEMODE

#region Directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace Xbim.IO.Parser
{
#if STANDALONE
    //
    // These are the dummy declarations for stand-alone GPLEX applications
    // normally these declarations would come from the parser.
    // If you declare /noparser, or %option noparser then you get this.
    //

     public enum Tokens
    { 
      EOF = 0, maxParseToken = int.MaxValue 
      // must have at least these two, values are almost arbitrary
    }

     public abstract class ScanBase
    {
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylex")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylex")]
        public abstract int yylex();

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yywrap")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yywrap")]
        protected virtual bool yywrap() { return true; }

#if BABEL
        protected abstract int CurrentSc { get; set; }
        // EolState is the 32-bit of state data persisted at 
        // the end of each line for Visual Studio colorization.  
        // The default is to return CurrentSc.  You must override
        // this if you want more complicated behavior.
        public virtual int EolState { 
            get { return CurrentSc; }
            set { CurrentSc = value; } 
        }
    }
    
     public interface IColorScan
    {
        void SetSource(string source, int offset);
        int GetNext(ref int state, out int start, out int end);
#endif // BABEL
    }

#endif
    // STANDALONE

    // If the compiler can't find the scanner base class maybe you
    // need to run GPPG with the /gplex option, or GPLEX with /noparser
#if BABEL
     public sealed partial class Scanner : ScanBase, IColorScan
    {
        private ScanBuff buffer;
        int currentScOrd;  // start condition ordinal
        
        protected override int CurrentSc 
        {
             // The current start state is a property
             // to try to avoid the user error of setting
             // scState but forgetting to update the FSA
             // start state "currentStart"
             //
             get { return currentScOrd; }  // i.e. return YY_START;
             set { currentScOrd = value;   // i.e. BEGIN(value);
                   currentStart = startState[value]; }
        }
#else
    // BABEL
    public sealed partial class Scanner : ScanBase
    {
        private ScanBuff buffer;
        private int currentScOrd; // start condition ordinal
#endif
        // BABEL

        /// <summary>
        ///   The input buffer for this scanner.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScanBuff Buffer
        {
            get { return buffer; }
        }

        private static int GetMaxParseToken()
        {
            var f = typeof (Tokens).GetField("maxParseToken");
            return (f == null ? int.MaxValue : (int) f.GetValue(null));
        }

        private static readonly int parserMax = GetMaxParseToken();

        private enum Result
        {
            accept,
            noMatch,
            contextFound
        };

        private const int maxAccept = 81;
        private const int initial = 82;
        private const int eofNum = 0;
        private const int goStart = -1;
        private const int INITIAL = 0;

        #region user code

/*
   A simple example of using GPLEX to implement the unix "strings" 
   functionality.  Reads a (possibly binary) file, finding sequences
   of alphabetic ASCII characters.
 */
        public static int Pass = 1;
        public static bool emitPass = true;
        public static bool comment;

        public void SetValue()
        {
            if (!comment)
            {
                yylval.strVal = yytext;
            }
        }

        #endregion user code

        private int state;
        private int currentStart = startState[0];
        private int code; // last code read
        private int cCol; // column number of code
        private int lNum; // current line number
        //
        // The following instance variables are used, among other
        // things, for constructing the yylloc location objects.
        //
        private int tokPos; // buffer position at start of token
        private int tokCol; // zero-based column number at start of token
        private int tokLin; // line number at start of token
        private int tokEPos; // buffer position at end of token
        private int tokECol; // column number at end of token
        private int tokELin; // line number at end of token
        private string tokTxt; // lazily constructed text of token
#if STACK          
        private Stack<int> scStack = new Stack<int>();
#endif
        // STACK

        #region ScannerTables

        private struct Table
        {
            public readonly int min;
            public readonly int rng;
            public readonly int dflt;
            public readonly sbyte[] nxt;

            public Table(int m, int x, int d, sbyte[] n)
            {
                min = m;
                rng = x;
                dflt = d;
                nxt = n;
            }
        };

        private static readonly int[] startState = new[] {82, 0};

        private static readonly Table[] NxS = new Table[100]
                                                  {
/* NxS[   0] */ new Table(0, 0, 0, null), // Shortest string ""
/* NxS[   1] */ // Shortest string ""
                new Table(0, 1, -1, new sbyte[] {1}),
/* NxS[   2] */ new Table(0, 0, -1, null), // Shortest string "\x01"
/* NxS[   3] */ new Table(0, 0, -1, null), // Shortest string "\t"
/* NxS[   4] */ new Table(0, 0, -1, null), // Shortest string "\n"
/* NxS[   5] */ new Table(0, 0, -1, null), // Shortest string "\r"
/* NxS[   6] */ new Table(0, 0, -1, null), // Shortest string "\x20"
/* NxS[   7] */ // Shortest string "!"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              81, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, -1, -1, -1, -1, -1, -1, -1, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81, -1, -1, -1, -1, 81, -1, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81
                                          }),
/* NxS[   8] */ // Shortest string "\""
                new Table(48, 23, -1, new sbyte[]
                                          {
                                              99, 99, 99, 99, 99, 99,
                                              99, 99, 99, 99, -1, -1, -1, -1, -1, -1, -1, 99, 99, 99, 99, 99,
                                              99
                                          }),
/* NxS[   9] */ // Shortest string "#"
                new Table(48, 10, -1, new sbyte[]
                                          {
                                              77, 77, 77, 77, 77, 77,
                                              77, 77, 77, 77
                                          }),
/* NxS[  10] */ new Table(0, 0, -1, null), // Shortest string "$"
/* NxS[  11] */ // Shortest string "&"
                new Table(83, 1, -1, new sbyte[] {94}),
/* NxS[  12] */ // Shortest string "'"
                new Table(1, 128, 93, new sbyte[]
                                          {
                                              -1, -1, -1, -1, -1, -1,
                                              -1, -1, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              75, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, -1, -1
                                          }),
/* NxS[  13] */ new Table(0, 0, -1, null), // Shortest string "("
/* NxS[  14] */ new Table(0, 0, -1, null), // Shortest string ")"
/* NxS[  15] */ // Shortest string "*"
                new Table(47, 1, -1, new sbyte[] {74}),
/* NxS[  16] */ // Shortest string "+"
                new Table(46, 12, -1, new sbyte[]
                                          {
                                              62, -1, 73, 73, 73, 73,
                                              73, 73, 73, 73, 73, 73
                                          }),
/* NxS[  17] */ new Table(0, 0, -1, null), // Shortest string ","
/* NxS[  18] */ // Shortest string "."
                new Table(46, 50, -1, new sbyte[]
                                          {
                                              62, -1, 68, 68, 68, 68,
                                              68, 68, 68, 68, 68, 68, -1, -1, -1, -1, -1, -1, -1, 89, 89, 89,
                                              89, 89, 90, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
                                              90, 91, 89, 89, 89, 89, 89, -1, -1, -1, -1, 89
                                          }),
/* NxS[  19] */ // Shortest string "/"
                new Table(42, 1, -1, new sbyte[] {67}),
/* NxS[  20] */ // Shortest string "0"
                new Table(46, 77, -1, new sbyte[]
                                          {
                                              62, -1, 63, 63, 63, 63,
                                              63, 63, 63, 63, 63, 63, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23
                                          }),
/* NxS[  21] */ new Table(0, 0, -1, null), // Shortest string ";"
/* NxS[  22] */ new Table(0, 0, -1, null), // Shortest string "="
/* NxS[  23] */ // Shortest string "A"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  24] */ // Shortest string "D"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 58, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  25] */ // Shortest string "E"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 42, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  26] */ // Shortest string "H"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 36,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  27] */ // Shortest string "I"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 33, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  28] */ // Shortest string "S"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 29, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  29] */ // Shortest string "ST"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 30,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  30] */ // Shortest string "STE"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 31, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  31] */ // Shortest string "STEP"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, 32, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  32] */ new Table(0, 0, -1, null), // Shortest string "STEP;"
/* NxS[  33] */ // Shortest string "IS"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 34, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  34] */ // Shortest string "ISO"
                new Table(45, 78, -1, new sbyte[]
                                          {
                                              83, -1, -1, 34, 34, 34,
                                              34, 34, 34, 34, 34, 34, 34, -1, 35, -1, -1, -1, -1, -1, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23
                                          }),
/* NxS[  35] */ new Table(0, 0, -1, null), // Shortest string "ISO;"
/* NxS[  36] */ // Shortest string "HE"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 37, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  37] */ // Shortest string "HEA"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 38, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  38] */ // Shortest string "HEAD"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 39,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  39] */ // Shortest string "HEADE"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 40, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  40] */ // Shortest string "HEADER"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, 41, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  41] */ new Table(0, 0, -1, null), // Shortest string "HEADER;"
/* NxS[  42] */ // Shortest string "EN"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 43, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  43] */ // Shortest string "END"
                new Table(45, 78, -1, new sbyte[]
                                          {
                                              84, -1, -1, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              44, 23, 23, 23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23
                                          }),
/* NxS[  44] */ // Shortest string "ENDS"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 45, 23, 46,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 47, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  45] */ // Shortest string "ENDSC"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 54, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  46] */ // Shortest string "ENDSE"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 52, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  47] */ // Shortest string "ENDST"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 48,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  48] */ // Shortest string "ENDSTE"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 49, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  49] */ // Shortest string "ENDSTEP"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, 50, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  50] */ // Shortest string "ENDSTEP;"
                new Table(10, 1, 51, new sbyte[] {-1}),
/* NxS[  51] */ // Shortest string "ENDSTEP;\x01"
                new Table(10, 1, 51, new sbyte[] {-1}),
/* NxS[  52] */ // Shortest string "ENDSEC"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, 53, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  53] */ new Table(0, 0, -1, null), // Shortest string "ENDSEC;"
/* NxS[  54] */ // Shortest string "ENDSCO"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 55, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  55] */ // Shortest string "ENDSCOP"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 56,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  56] */ // Shortest string "ENDSCOPE"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  57] */ new Table(0, 0, -1, null), // Shortest string "END-ISO;"
/* NxS[  58] */ // Shortest string "DA"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 59, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  59] */ // Shortest string "DAT"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, -1, -1, -1, -1, -1, -1, 60, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  60] */ // Shortest string "DATA"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, -1, 61, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  61] */ new Table(0, 0, -1, null), // Shortest string "DATA;"
/* NxS[  62] */ // Shortest string "+."
                new Table(46, 24, -1, new sbyte[]
                                          {
                                              62, -1, 62, 62, 62, 62,
                                              62, 62, 62, 62, 62, 62, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                              -1, 88
                                          }),
/* NxS[  63] */ // Shortest string "00"
                new Table(46, 77, -1, new sbyte[]
                                          {
                                              62, -1, 63, 63, 63, 63,
                                              63, 63, 63, 63, 63, 63, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23,
                                              23, 64, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23
                                          }),
/* NxS[  64] */ // Shortest string "00E"
                new Table(43, 80, -1, new sbyte[]
                                          {
                                              65, -1, 65, -1, -1, 66,
                                              66, 66, 66, 66, 66, 66, 66, 66, 66, -1, -1, -1, -1, -1, -1, -1,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23
                                          }),
/* NxS[  65] */ // Shortest string "+.E+"
                new Table(48, 10, -1, new sbyte[]
                                          {
                                              65, 65, 65, 65, 65, 65,
                                              65, 65, 65, 65
                                          }),
/* NxS[  66] */ // Shortest string "00E0"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              66, 66, 66, 66, 66, 66,
                                              66, 66, 66, 66, -1, -1, -1, -1, -1, -1, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, -1, -1, -1, -1, 23, -1, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                              23, 23, 23, 23, 23
                                          }),
/* NxS[  67] */ new Table(0, 0, -1, null), // Shortest string "/*"
/* NxS[  68] */ // Shortest string ".0"
                new Table(46, 50, -1, new sbyte[]
                                          {
                                              62, -1, 68, 68, 68, 68,
                                              68, 68, 68, 68, 68, 68, -1, -1, -1, -1, -1, -1, -1, 89, 89, 89,
                                              89, 92, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, -1, -1, -1, -1, 89
                                          }),
/* NxS[  69] */ new Table(0, 0, -1, null), // Shortest string ".U."
/* NxS[  70] */ new Table(0, 0, -1, null), // Shortest string ".A."
/* NxS[  71] */ new Table(0, 0, -1, null), // Shortest string ".F."
/* NxS[  72] */ // Shortest string ".0E0"
                new Table(46, 50, -1, new sbyte[]
                                          {
                                              70, -1, 72, 72, 72, 72,
                                              72, 72, 72, 72, 72, 72, -1, -1, -1, -1, -1, -1, -1, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, -1, -1, -1, -1, 89
                                          }),
/* NxS[  73] */ // Shortest string "+0"
                new Table(46, 24, -1, new sbyte[]
                                          {
                                              62, -1, 73, 73, 73, 73,
                                              73, 73, 73, 73, 73, 73, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                              -1, 88
                                          }),
/* NxS[  74] */ new Table(0, 0, -1, null), // Shortest string "*/"
/* NxS[  75] */ // Shortest string "''"
                new Table(39, 1, -1, new sbyte[] {93}),
/* NxS[  76] */ new Table(0, 0, -1, null), // Shortest string "&SCOPE"
/* NxS[  77] */ // Shortest string "#0"
                new Table(9, 53, -1, new sbyte[]
                                         {
                                             98, -1, -1, -1, -1, -1,
                                             -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                             -1, 98, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                             -1, 77, 77, 77, 77, 77, 77, 77, 77, 77, 77, -1, -1, -1, 78
                                         }),
/* NxS[  78] */ new Table(0, 0, -1, null), // Shortest string "#0="
/* NxS[  79] */ new Table(0, 0, -1, null), // Shortest string "#0\t="
/* NxS[  80] */ new Table(0, 0, -1, null), // Shortest string "\"0\""
/* NxS[  81] */ // Shortest string "!0"
                new Table(48, 75, -1, new sbyte[]
                                          {
                                              81, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, -1, -1, -1, -1, -1, -1, -1, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81, -1, -1, -1, -1, 81, -1, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81, 81,
                                              81, 81, 81, 81, 81
                                          }),
/* NxS[  82] */ // Shortest string ""
                new Table(0, 123, 2, new sbyte[]
                                         {
                                             1, 2, 2, 2, 2, 2,
                                             2, 2, 2, 3, 4, 2, 2, 5, 2, 2, 2, 2, 2, 2, 2, 2,
                                             2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6, 7, 8, 9, 10, 2,
                                             11, 12, 13, 14, 15, 16, 17, 16, 18, 19, 20, 20, 20, 20, 20, 20,
                                             20, 20, 20, 20, 2, 21, 2, 22, 2, 2, 2, 23, 23, 23, 24, 25,
                                             23, 23, 26, 27, 23, 23, 23, 23, 23, 23, 23, 23, 23, 28, 23, 23,
                                             23, 23, 23, 23, 23, 2, 2, 2, 2, 23, 2, 23, 23, 23, 23, 23,
                                             23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
                                             23, 23, 23, 23, 23
                                         }),
/* NxS[  83] */ // Shortest string "ISO-"
                new Table(45, 15, -1, new sbyte[]
                                          {
                                              83, -1, -1, 83, 83, 83,
                                              83, 83, 83, 83, 83, 83, 83, -1, 35
                                          }),
/* NxS[  84] */ // Shortest string "END-"
                new Table(73, 1, -1, new sbyte[] {85}),
/* NxS[  85] */ // Shortest string "END-I"
                new Table(83, 1, -1, new sbyte[] {86}),
/* NxS[  86] */ // Shortest string "END-IS"
                new Table(79, 1, -1, new sbyte[] {87}),
/* NxS[  87] */ // Shortest string "END-ISO"
                new Table(45, 15, -1, new sbyte[]
                                          {
                                              87, -1, -1, 87, 87, 87,
                                              87, 87, 87, 87, 87, 87, 87, -1, 57
                                          }),
/* NxS[  88] */ // Shortest string "+.E"
                new Table(43, 15, -1, new sbyte[]
                                          {
                                              65, -1, 65, -1, -1, 65,
                                              65, 65, 65, 65, 65, 65, 65, 65, 65
                                          }),
/* NxS[  89] */ // Shortest string ".A"
                new Table(46, 50, -1, new sbyte[]
                                          {
                                              70, -1, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, -1, -1, -1, -1, -1, -1, -1, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, -1, -1, -1, -1, 89
                                          }),
/* NxS[  90] */ // Shortest string ".F"
                new Table(46, 50, -1, new sbyte[]
                                          {
                                              71, -1, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, -1, -1, -1, -1, -1, -1, -1, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, -1, -1, -1, -1, 89
                                          }),
/* NxS[  91] */ // Shortest string ".U"
                new Table(46, 50, -1, new sbyte[]
                                          {
                                              69, -1, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, -1, -1, -1, -1, -1, -1, -1, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, -1, -1, -1, -1, 89
                                          }),
/* NxS[  92] */ // Shortest string ".0E"
                new Table(43, 53, -1, new sbyte[]
                                          {
                                              65, -1, 65, 70, -1, 72,
                                              72, 72, 72, 72, 72, 72, 72, 72, 72, -1, -1, -1, -1, -1, -1, -1,
                                              89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
                                              89, 89, 89, 89, 89, 89, 89, 89, 89, 89, -1, -1, -1, -1, 89
                                          }),
/* NxS[  93] */ // Shortest string "'\t"
                new Table(1, 128, 93, new sbyte[]
                                          {
                                              -1, -1, -1, -1, -1, -1,
                                              -1, -1, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              75, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93, 93,
                                              93, 93, 93, 93, 93, 93, 93, 93, -1, -1
                                          }),
/* NxS[  94] */ // Shortest string "&S"
                new Table(67, 1, -1, new sbyte[] {95}),
/* NxS[  95] */ // Shortest string "&SC"
                new Table(79, 1, -1, new sbyte[] {96}),
/* NxS[  96] */ // Shortest string "&SCO"
                new Table(80, 1, -1, new sbyte[] {97}),
/* NxS[  97] */ // Shortest string "&SCOP"
                new Table(69, 1, -1, new sbyte[] {76}),
/* NxS[  98] */ // Shortest string "#0\t"
                new Table(9, 53, -1, new sbyte[]
                                         {
                                             98, -1, -1, -1, -1, -1,
                                             -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                             -1, 98, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                             -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 79
                                         }),
/* NxS[  99] */ // Shortest string "\"0"
                new Table(34, 37, -1, new sbyte[]
                                          {
                                              80, -1, -1, -1, -1, -1,
                                              -1, -1, -1, -1, -1, -1, -1, -1, 99, 99, 99, 99, 99, 99, 99, 99,
                                              99, 99, -1, -1, -1, -1, -1, -1, -1, 99, 99, 99, 99, 99, 99
                                          }),
                                                  };

        private int NextState()
        {
            if (code == ScanBuff.EndOfFile)
                return eofNum;
            else
                unchecked
                {
                    int rslt;
                    int idx = (byte) (code - NxS[state].min);
                    if ((uint) idx >= (uint) NxS[state].rng) rslt = NxS[state].dflt;
                    else rslt = NxS[state].nxt[idx];
                    return rslt;
                }
        }

        #endregion

#if BACKUP
        // ==============================================================
        // == Nested struct used for backup in automata that do backup ==
        // ==============================================================

        private struct Context // class used for automaton backup.
        {
            public int bPos;
            public int rPos; // scanner.readPos saved value
            public int cCol;
            public int lNum; // Need this in case of backup over EOL.
            public int state;
            public int cChr;
        }

        private Context ctx;
#endif
        // BACKUP

        // ==============================================================
        // ==== Nested struct to support input switching in scanners ====
        // ==============================================================

        private struct BufferContext
        {
            internal ScanBuff buffSv;
            internal int chrSv;
            internal int cColSv;
            internal int lNumSv;
        }

        // ==============================================================
        // ===== Private methods to save and restore buffer contexts ====
        // ==============================================================

        /// <summary>
        ///   This method creates a buffer context record from
        ///   the current buffer object, together with some
        ///   scanner state values.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private BufferContext MkBuffCtx()
        {
            BufferContext rslt;
            rslt.buffSv = this.buffer;
            rslt.chrSv = this.code;
            rslt.cColSv = this.cCol;
            rslt.lNumSv = this.lNum;
            return rslt;
        }

        /// <summary>
        ///   This method restores the buffer value and allied
        ///   scanner state from the given context record value.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void RestoreBuffCtx(BufferContext value)
        {
            this.buffer = value.buffSv;
            this.code = value.chrSv;
            this.cCol = value.cColSv;
            this.lNum = value.lNumSv;
        }

        // =================== End Nested classes =======================

#if !NOFILES
        public Scanner(Stream file)
        {
            SetSource(file); // no unicode option
        }

#endif
        // !NOFILES

        public Scanner()
        {
        }

        private int readPos;

        private void GetCode()
        {
            if (code == '\n') // This needs to be fixed for other conventions
                // i.e. [\r\n\205\u2028\u2029]
            {
                cCol = -1;
                lNum++;
            }
            readPos = buffer.Pos;

            // Now read new codepoint.
            code = buffer.Read();
            if (code > ScanBuff.EndOfFile)
            {
#if (!BYTEMODE)
                if (code >= 0xD800 && code <= 0xDBFF)
                {
                    int next = buffer.Read();
                    if (next < 0xDC00 || next > 0xDFFF)
                        code = ScanBuff.UnicodeReplacementChar;
                    else
                        code = (0x10000 + (code & 0x3FF << 10) + (next & 0x3FF));
                }
#endif
                cCol++;
            }
        }

        private void MarkToken()
        {
#if (!PERSIST)
            buffer.Mark();
#endif
            tokPos = readPos;
            tokLin = lNum;
            tokCol = cCol;
        }

        private void MarkEnd()
        {
            tokTxt = null;
            tokEPos = readPos;
            tokELin = lNum;
            tokECol = cCol;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private int Peek()
        {
            int rslt, codeSv = code, cColSv = cCol, lNumSv = lNum, bPosSv = buffer.Pos;
            GetCode();
            rslt = code;
            lNum = lNumSv;
            cCol = cColSv;
            code = codeSv;
            buffer.Pos = bPosSv;
            return rslt;
        }

        // ==============================================================
        // =====    Initialization of string-based input buffers     ====
        // ==============================================================

        /// <summary>
        ///   Create and initialize a StringBuff buffer object for this scanner
        /// </summary>
        /// <param name = "source">the input string</param>
        /// <param name = "offset">starting offset in the string</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(string source, int offset)
        {
            this.buffer = ScanBuff.GetBuffer(source);
            this.buffer.Pos = offset;
            this.lNum = 0;
            this.code = '\n'; // to initialize yyline, yycol and lineStart
            GetCode();
        }

#if !NOFILES
        // ================ LineBuffer Initialization ===================

        /// <summary>
        ///   Create and initialize a LineBuff buffer object for this scanner
        /// </summary>
        /// <param name = "source">the list of input strings</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(IList<string> source)
        {
            this.buffer = ScanBuff.GetBuffer(source);
            this.code = '\n'; // to initialize yyline, yycol and lineStart
            this.lNum = 0;
            GetCode();
        }

        // =============== StreamBuffer Initialization ==================

        /// <summary>
        ///   Create and initialize a StreamBuff buffer object for this scanner.
        ///   StreamBuff is buffer for 8-bit byte files.
        /// </summary>
        /// <param name = "source">the input byte stream</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(Stream source)
        {
            this.buffer = ScanBuff.GetBuffer(source);
            this.lNum = 0;
            this.code = '\n'; // to initialize yyline, yycol and lineStart
            GetCode();
        }

#if !BYTEMODE
    // ================ TextBuffer Initialization ===================

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetSource(Stream source, int fallbackCodePage)
        {
            this.buffer = ScanBuff.GetBuffer(source, fallbackCodePage);
            this.lNum = 0;
            this.code = '\n'; // to initialize yyline, yycol and lineStart
            GetCode();
        }
#endif
        // !BYTEMODE
#endif
        // !NOFILES

        // ==============================================================

#if BABEL
    //
    //  Get the next token for Visual Studio
    //
    //  "state" is the inout mode variable that maintains scanner
    //  state between calls, using the EolState property. In principle,
    //  if the calls of EolState are costly set could be called once
    //  only per line, at the start; and get called only at the end
    //  of the line. This needs more infrastructure ...
    //
        public int GetNext(ref int state, out int start, out int end)
        {
                Tokens next;
            int s, e;
            s = state;        // state at start
            EolState = state;
                next = (Tokens)Scan();
            state = EolState;
            e = state;       // state at end;
            start = tokPos;
            end = tokEPos - 1; // end is the index of last char.
            return (int)next;
        }
#endif
        // BABEL

        // ======== AbstractScanner<> Implementation =========

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yylex")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yylex")]
        public override int yylex()
        {
            // parserMax is set by reflecting on the Tokens
            // enumeration.  If maxParseToken is defined
            // that is used, otherwise int.MaxValue is used.
            int next;
            do
            {
                next = Scan();
            } while (next >= parserMax);
            return next;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private int yypos
        {
            get { return tokPos; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private int yyline
        {
            get { return tokLin; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private int yycol
        {
            get { return tokCol; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yytext")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yytext")]
        public string yytext
        {
            get
            {
                if (tokTxt == null)
                    tokTxt = buffer.GetString(tokPos, tokEPos);
                return tokTxt;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void yyless(int n)
        {
            buffer.Pos = tokPos;
            // Must read at least one char, so set before start.
            cCol = tokCol - 1;
            GetCode();
            // Now ensure that line counting is correct.
            lNum = tokLin;
            // And count the rest of the text.
            for (var i = 0; i < n; i++) GetCode();
            MarkEnd();
        }

        //
        //  It would be nice to count backward in the text
        //  but it does not seem possible to re-establish
        //  the correct column counts except by going forward.
        //
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void _yytrunc(int n)
        {
            yyless(yyleng - n);
        }

        //
        // This is painful, but we no longer count
        // codepoints.  For the overwhelming majority 
        // of cases the single line code is fast, for
        // the others, well, at least it is all in the
        // buffer so no files are touched. Note that we
        // can't use (tokEPos - tokPos) because of the
        // possibility of surrogate pairs in the token.
        //
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "yyleng")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yyleng")]
        public int yyleng
        {
            get
            {
                if (tokELin == tokLin)
                    return tokECol - tokCol;
                else
                {
                    int ch;
                    var count = 0;
                    buffer.Pos = tokPos;
                    do
                    {
                        ch = buffer.Read();
                        if (!char.IsHighSurrogate((char) ch)) count++;
                    } while (buffer.Pos < tokEPos && ch != ScanBuff.EndOfFile);
                    return count;
                }
            }
        }

        // ============ methods available in actions ==============

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal int YY_START
        {
            get { return currentScOrd; }
            set
            {
                currentScOrd = value;
                currentStart = startState[value];
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void BEGIN(int next)
        {
            currentScOrd = next;
            currentStart = startState[next];
        }

        // ============== The main tokenizer code =================

        private int Scan()
        {
            for (;;)
            {
                int next; // next state to enter                   
#if BACKUP
                var rslt = Result.noMatch;
#endif
                // BACKUP
#if LEFTANCHORS
                    for (;;)
                    {
                        // Discard characters that do not start any pattern.
                        // Must check the left anchor condition after *every* GetCode!
                        state = ((cCol == 0) ? anchorState[currentScOrd] : currentStart);
                        if ((next = NextState()) != goStart) 
                            break; // LOOP EXIT HERE...
                        GetCode();
                    }

#else
                // !LEFTANCHORS
                state = currentStart;
                while ((next = NextState()) == goStart)
                    // At this point, the current character has no
                    // transition from the current state.  We discard 
                    // the "no-match" char.   In traditional LEX such 
                    // characters are echoed to the console.
                    GetCode();
#endif
                // LEFTANCHORS                    
                // At last, a valid transition ...    
                MarkToken();
                state = next;
                GetCode();

                while ((next = NextState()) > eofNum) // Exit for goStart AND for eofNum
#if BACKUP
                    if (state <= maxAccept && next > maxAccept) // need to prepare backup data
                    {
                        // ctx is an object. The fields may be 
                        // mutated by the call to Recurse2.
                        // On return the data in ctx is the
                        // *latest* accept state that was found.

                        rslt = Recurse2(ref ctx, next);
                        if (rslt == Result.noMatch)
                            RestoreStateAndPos(ref ctx);
                        break;
                    }
                    else
#endif
                        // BACKUP
                    {
                        state = next;
                        GetCode();
                    }
                if (state <= maxAccept)
                {
                    MarkEnd();

                    #region ActionSwitch

#pragma warning disable 162
                    switch (state)
                    {
                        case eofNum:
                            if (yywrap())
                                return (int) Tokens.EOF;
                            break;
                        case 1: // Recognized '[\0]+',	Shortest string ""
                            break;
                        case 2: // Recognized '[^)]',	Shortest string "\x01"
                        case 7: // Recognized '[^)]',	Shortest string "!"
                        case 8: // Recognized '[^)]',	Shortest string "\""
                        case 9: // Recognized '[^)]',	Shortest string "#"
                        case 11: // Recognized '[^)]',	Shortest string "&"
                        case 12: // Recognized '[^)]',	Shortest string "'"
                        case 18: // Recognized '[^)]',	Shortest string "."
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.MISC);
                            }
                            break;
                        case 3: // Recognized '"\t"',	Shortest string "\t"
                            break;
                        case 4: // Recognized '[\n]',	Shortest string "\n"
                            break;
                        case 5: // Recognized '[\r]',	Shortest string "\r"
                            break;
                        case 6: // Recognized '" "',	Shortest string "\x20"
                            break;
                        case 10: // Recognized '[$]',	Shortest string "$"
                            if (!comment)
                            {
                                return ((int) Tokens.NONDEF);
                            }
                            break;
                        case 13: // Recognized '[(]',	Shortest string "("
                            if (!comment) return ('(');
                            break;
                        case 14: // Recognized '[)]',	Shortest string ")"
                            if (!comment) return (')');
                            break;
                        case 15: // Recognized '[\*]',	Shortest string "*"
                            if (!comment) return ((int) Tokens.OVERRIDE);
                            break;
                        case 16: // Recognized '[\-\+0-9][0-9]*',	Shortest string "+"
                        case 20: // Recognized '[\-\+0-9][0-9]*',	Shortest string "0"
                        case 63: // Recognized '[\-\+0-9][0-9]*',	Shortest string "00"
                        case 73: // Recognized '[\-\+0-9][0-9]*',	Shortest string "+0"
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.INTEGER);
                            }
                            break;
                        case 17: // Recognized '[,]',	Shortest string ","
                            if (!comment) return (',');
                            break;
                        case 19: // Recognized '[/]',	Shortest string "/"
                            if (!comment) return ('/');
                            break;
                        case 21: // Recognized '[;]',	Shortest string ";"
                            if (!comment) return (';');
                            break;
                        case 22: // Recognized '[=]',	Shortest string "="
                            if (!comment) return ('=');
                            break;
                        case 23: // Recognized '[a-zA-Z0-9_]+',	Shortest string "A"
                        case 24: // Recognized '[a-zA-Z0-9_]+',	Shortest string "D"
                        case 25: // Recognized '[a-zA-Z0-9_]+',	Shortest string "E"
                        case 26: // Recognized '[a-zA-Z0-9_]+',	Shortest string "H"
                        case 27: // Recognized '[a-zA-Z0-9_]+',	Shortest string "I"
                        case 28: // Recognized '[a-zA-Z0-9_]+',	Shortest string "S"
                        case 29: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ST"
                        case 30: // Recognized '[a-zA-Z0-9_]+',	Shortest string "STE"
                        case 31: // Recognized '[a-zA-Z0-9_]+',	Shortest string "STEP"
                        case 33: // Recognized '[a-zA-Z0-9_]+',	Shortest string "IS"
                        case 34: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ISO"
                        case 36: // Recognized '[a-zA-Z0-9_]+',	Shortest string "HE"
                        case 37: // Recognized '[a-zA-Z0-9_]+',	Shortest string "HEA"
                        case 38: // Recognized '[a-zA-Z0-9_]+',	Shortest string "HEAD"
                        case 39: // Recognized '[a-zA-Z0-9_]+',	Shortest string "HEADE"
                        case 40: // Recognized '[a-zA-Z0-9_]+',	Shortest string "HEADER"
                        case 42: // Recognized '[a-zA-Z0-9_]+',	Shortest string "EN"
                        case 43: // Recognized '[a-zA-Z0-9_]+',	Shortest string "END"
                        case 44: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDS"
                        case 45: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDSC"
                        case 46: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDSE"
                        case 47: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDST"
                        case 48: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDSTE"
                        case 49: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDSTEP"
                        case 52: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDSEC"
                        case 54: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDSCO"
                        case 55: // Recognized '[a-zA-Z0-9_]+',	Shortest string "ENDSCOP"
                        case 58: // Recognized '[a-zA-Z0-9_]+',	Shortest string "DA"
                        case 59: // Recognized '[a-zA-Z0-9_]+',	Shortest string "DAT"
                        case 60: // Recognized '[a-zA-Z0-9_]+',	Shortest string "DATA"
                        case 64: // Recognized '[a-zA-Z0-9_]+',	Shortest string "00E"
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.TYPE);
                            }
                            break;
                        case 32: // Recognized 'STEP;',	Shortest string "STEP;"
                            if (!comment) return ((int) Tokens.ISOSTEPSTART);
                            break;
                        case 35: // Recognized 'ISO[0-9\-]*;',	Shortest string "ISO;"
                            comment = false;
                            return ((int) Tokens.ISOSTEPSTART);
                            break;
                        case 41: // Recognized 'HEADER;',	Shortest string "HEADER;"
                            if (!comment) return ((int) Tokens.HEADER);
                            break;
                        case 50: // Recognized 'ENDSTEP;',	Shortest string "ENDSTEP;"
                            comment = false;
                            return ((int) Tokens.ISOSTEPEND);
                            break;
                        case 51: // Recognized '"ENDSTEP;".*',	Shortest string "ENDSTEP;\x01"
                            comment = false;
                            return ((int) Tokens.ISOSTEPEND);
                            break;
                        case 53: // Recognized 'ENDSEC;',	Shortest string "ENDSEC;"
                            if (!comment) return ((int) Tokens.ENDSEC);
                            break;
                        case 56: // Recognized 'ENDSCOPE',	Shortest string "ENDSCOPE"
                            if (!comment) return ((int) Tokens.ENDSCOPE);
                            break;
                        case 57: // Recognized '"END-ISO"[0-9\-]*;',	Shortest string "END-ISO;"
                            comment = false;
                            return ((int) Tokens.ISOSTEPEND);
                            break;
                        case 61: // Recognized 'DATA;',	Shortest string "DATA;"
                            if (!comment) return ((int) Tokens.DATA);
                            break;
                        case 62: // Recognized '[\-\+\.0-9][\.0-9]+',	Shortest string "+."
                        case 68: // Recognized '[\-\+\.0-9][\.0-9]+',	Shortest string ".0"
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.FLOAT);
                            }
                            break;
                        case 65: // Recognized '[\-\+\.0-9][\.0-9]+E[\-\+0-9][0-9]*',	Shortest string "+.E+"
                        case 66: // Recognized '[\-\+\.0-9][\.0-9]+E[\-\+0-9][0-9]*',	Shortest string "00E0"
                        case 72: // Recognized '[\-\+\.0-9][\.0-9]+E[\-\+0-9][0-9]*',	Shortest string ".0E0"
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.FLOAT);
                            }
                            break;
                        case 67: // Recognized '"/*"',	Shortest string "/*"
                            comment = true;
                            break;
                        case 69: // Recognized '[\.][U][\.]',	Shortest string ".U."
                            if (!comment)
                            {
                                return ((int) Tokens.NONDEF);
                            }
                            break;
                        case 70: // Recognized '[\.][A-Z0-9_]+[\.]',	Shortest string ".A."
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.ENUM);
                            }
                            break;
                        case 71: // Recognized '[\.][TF][\.]',	Shortest string ".F."
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.BOOLEAN);
                            }
                            break;
                        case 74: // Recognized '"*/"',	Shortest string "*/"
                            comment = false;
                            break;
                        case 75:
                            // Recognized '[\']([\n]|[\000\011-\046\050-\176\201-\237\240-\377]|[\047][\047])*[\']',	Shortest string "''"
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.STRING);
                            }
                            break;
                        case 76: // Recognized '&SCOPE',	Shortest string "&SCOPE"
                            if (!comment) return ((int) Tokens.SCOPE);
                            break;
                        case 77: // Recognized '#[0-9]+',	Shortest string "#0"
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.IDENTITY);
                            }
                            break;
                        case 78: // Recognized '#[0-9]+/=',	Shortest string "#0="
                            _yytrunc(1);
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.ENTITY);
                            }
                            break;
                        case 79: // Recognized '#[0-9]+[ \t]*/=',	Shortest string "#0\t="
                            _yytrunc(1);
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.ENTITY);
                            }
                            break;
                        case 80: // Recognized '[\"][0-9A-F]+[\"]',	Shortest string "\"0\""
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.HEXA);
                            }
                            break;
                        case 81: // Recognized '![a-zA-Z0-9_]+',	Shortest string "!0"
                            SetValue();
                            if (!comment)
                            {
                                SetValue();
                                return ((int) Tokens.TYPE);
                            }
                            break;
                        default:
                            break;
                    }
#pragma warning restore 162

                    #endregion
                }
            }
        }

#if BACKUP
        private Result Recurse2(ref Context ctx, int next)
        {
            // Assert: at entry "state" is an accept state AND
            //         NextState(state, code) != goStart AND
            //         NextState(state, code) is not an accept state.
            //
            SaveStateAndPos(ref ctx);
            state = next;
            GetCode();

            while ((next = NextState()) > eofNum)
            {
                if (state <= maxAccept && next > maxAccept) // need to update backup data
                    SaveStateAndPos(ref ctx);
                state = next;
                if (state == eofNum) return Result.accept;
                GetCode();
            }
            return (state <= maxAccept ? Result.accept : Result.noMatch);
        }

        private void SaveStateAndPos(ref Context ctx)
        {
            ctx.bPos = buffer.Pos;
            ctx.rPos = readPos;
            ctx.cCol = cCol;
            ctx.lNum = lNum;
            ctx.state = state;
            ctx.cChr = code;
        }

        private void RestoreStateAndPos(ref Context ctx)
        {
            buffer.Pos = ctx.bPos;
            readPos = ctx.rPos;
            cCol = ctx.cCol;
            lNum = ctx.lNum;
            state = ctx.state;
            code = ctx.cChr;
        }

#endif
        // BACKUP

        // ============= End of the tokenizer code ================

#if STACK        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void yy_clear_stack() { scStack.Clear(); }
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal int yy_top_state() { return scStack.Peek(); }
        
        internal void yy_push_state(int state)
        {
            scStack.Push(currentScOrd);
            BEGIN(state);
        }
        
        internal void yy_pop_state()
        {
            // Protect against input errors that pop too far ...
            if (scStack.Count > 0) {
				int newSc = scStack.Pop();
				BEGIN(newSc);
            } // Otherwise leave stack unchanged.
        }
#endif
        // STACK

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal void ECHO()
        {
            Console.Out.Write(yytext);
        }
    }

    // end class $Scanner

// ==============================================================
// <auto-generated>
// This code automatically produced from an embedded resource.
// Do not edit this file, or it will become incompatible with 
// the specification from which it was generated.
// </auto-generated>
// ==============================================================

// Code copied from GPLEX embedded resource
    [Serializable]
    public class BufferException : Exception
    {
        public BufferException()
        {
        }

        public BufferException(string message) : base(message)
        {
        }

        public BufferException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BufferException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public abstract class ScanBuff
    {
        private string fileNm;

        public const int EndOfFile = -1;
        public const int UnicodeReplacementChar = 0xFFFD;

        public bool IsFile
        {
            get { return (fileNm != null); }
        }

        public string FileName
        {
            get { return fileNm; }
            set { fileNm = value; }
        }

        public abstract int Pos { get; set; }
        public abstract int Read();

        public virtual void Mark()
        {
        }

        public abstract string GetString(int begin, int limit);

        public static ScanBuff GetBuffer(string source)
        {
            return new StringBuffer(source);
        }

        public static ScanBuff GetBuffer(IList<string> source)
        {
            return new LineBuffer(source);
        }

        public static ScanBuff GetBuffer(Stream source)
        {
            return new BuildBuffer(source);
        }

#if (!BYTEMODE)
        public static ScanBuff GetBuffer(Stream source, int fallbackCodePage)
        {
            return new BuildBuffer(source, fallbackCodePage);
        }
#endif
    }

    #region Buffer classes

    // ==============================================================
    // =====  Definitions for various ScanBuff derived classes   ====
    // ==============================================================
    // ===============         String input          ================
    // ==============================================================

    /// <summary>
    ///   This class reads characters from a single string as
    ///   required, for example, by Visual Studio language services
    /// </summary>
    internal sealed class StringBuffer : ScanBuff
    {
        private readonly string str; // input buffer
        private int bPos; // current position in buffer
        private readonly int sLen;

        public StringBuffer(string source)
        {
            this.str = source;
            this.sLen = source.Length;
            this.FileName = null;
        }

        public override int Read()
        {
            if (bPos < sLen) return str[bPos++];
            else if (bPos == sLen)
            {
                bPos++;
                return '\n';
            } // one strike, see new line
            else
            {
                bPos++;
                return EndOfFile;
            } // two strikes and you're out!
        }

        public override string GetString(int begin, int limit)
        {
            //  "limit" can be greater than sLen with the BABEL
            //  option set.  Read returns a "virtual" EOL if
            //  an attempt is made to read past the end of the
            //  string buffer.  Without the guard any attempt 
            //  to fetch yytext for a token that includes the 
            //  EOL will throw an index exception.
            if (limit > sLen) limit = sLen;
            if (limit <= begin) return "";
            else return str.Substring(begin, limit - begin);
        }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }

        public override string ToString()
        {
            return "StringBuffer";
        }
    }

    // ==============================================================
    //  The LineBuff class contributed by Nigel Horspool, 
    //  nigelh@cs.uvic.cs
    // ==============================================================

    internal sealed class LineBuffer : ScanBuff
    {
        private readonly IList<string> line; // list of source lines from a file
        private readonly int numLines; // number of strings in line list
        private string curLine; // current line in that list
        private int cLine; // index of current line in the list
        private int curLen; // length of current line
        private int curLineStart; // position of line start in whole file
        private int curLineEnd; // position of line end in whole file
        private int maxPos; // max position ever visited in whole file
        private int cPos; // ordinal number of code in source

        // Constructed from a list of strings, one per source line.
        // The lines have had trailing '\n' characters removed.
        public LineBuffer(IList<string> lineList)
        {
            line = lineList;
            numLines = line.Count;
            cPos = curLineStart = 0;
            curLine = (numLines > 0 ? line[0] : "");
            maxPos = curLineEnd = curLen = curLine.Length;
            cLine = 1;
            FileName = null;
        }

        public override int Read()
        {
            if (cPos < curLineEnd)
                return curLine[cPos++ - curLineStart];
            if (cPos++ == curLineEnd)
                return '\n';
            if (cLine >= numLines)
                return EndOfFile;
            curLine = line[cLine];
            curLen = curLine.Length;
            curLineStart = curLineEnd + 1;
            curLineEnd = curLineStart + curLen;
            if (curLineEnd > maxPos)
                maxPos = curLineEnd;
            cLine++;
            return curLen > 0 ? curLine[0] : '\n';
        }

        // To speed up searches for the line containing a position
        private int cachedPosition;
        private int cachedIxdex;
        private int cachedLineStart;

        // Given a position pos within the entire source, the results are
        //   ix     -- the index of the containing line
        //   lstart -- the position of the first character on that line
        private void findIndex(int pos, out int ix, out int lstart)
        {
            if (pos >= cachedPosition)
            {
                ix = cachedIxdex;
                lstart = cachedLineStart;
            }
            else
            {
                ix = lstart = 0;
            }
            for (;;)
            {
                var len = line[ix].Length + 1;
                if (pos < lstart + len) break;
                lstart += len;
                ix++;
            }
            cachedPosition = pos;
            cachedIxdex = ix;
            cachedLineStart = lstart;
        }

        public override string GetString(int begin, int limit)
        {
            if (begin >= maxPos || limit <= begin) return "";
            int endIx, begIx, endLineStart, begLineStart;
            findIndex(begin, out begIx, out begLineStart);
            var begCol = begin - begLineStart;
            findIndex(limit, out endIx, out endLineStart);
            var endCol = limit - endLineStart;
            var s = line[begIx];
            if (begIx == endIx)
            {
                // the usual case, substring all on one line
                return (endCol <= s.Length)
                           ? s.Substring(begCol, endCol - begCol)
                           : s.Substring(begCol) + "\n";
            }
            // the string spans multiple lines, yuk!
            var sb = new StringBuilder();
            if (begCol < s.Length)
                sb.Append(s.Substring(begCol));
            for (;;)
            {
                sb.Append("\n");
                s = line[++begIx];
                if (begIx >= endIx) break;
                sb.Append(s);
            }
            if (endCol <= s.Length)
            {
                sb.Append(s.Substring(0, endCol));
            }
            else
            {
                sb.Append(s);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public override int Pos
        {
            get { return cPos; }
            set
            {
                cPos = value;
                findIndex(cPos, out cLine, out curLineStart);
                curLine = line[cLine];
                curLineEnd = curLineStart + curLine.Length;
            }
        }

        public override string ToString()
        {
            return "LineBuffer";
        }
    }


    // ==============================================================
    // =====     class BuildBuff : for unicode text files    ========
    // ==============================================================

    internal class BuildBuffer : ScanBuff
    {
        // Double buffer for char stream.
        private class BufferElement
        {
            private StringBuilder bldr = new StringBuilder();
            private StringBuilder next = new StringBuilder();
            private int minIx;
            private int maxIx;
            private int brkIx;
            private bool appendToNext;

            internal int MaxIndex
            {
                get { return maxIx; }
            }

            // internal int MinIndex { get { return minIx; } }

            internal char this[int index]
            {
                get
                {
                    if (index < minIx || index >= maxIx)
                        throw new BufferException("Index was outside data buffer");
                    else if (index < brkIx)
                        return bldr[index - minIx];
                    else
                        return next[index - brkIx];
                }
            }

            internal void Append(char[] block, int count)
            {
                maxIx += count;
                if (appendToNext)
                    this.next.Append(block, 0, count);
                else
                {
                    this.bldr.Append(block, 0, count);
                    brkIx = maxIx;
                    appendToNext = true;
                }
            }

            internal string GetString(int start, int limit)
            {
                if (limit <= start)
                    return "";
                if (start >= minIx && limit <= maxIx)
                    if (limit < brkIx) // String entirely in bldr builder
                        return bldr.ToString(start - minIx, limit - start);
                    else if (start >= brkIx) // String entirely in next builder
                        return next.ToString(start - brkIx, limit - start);
                    else // Must do a string-concatenation
                        return
                            bldr.ToString(start - minIx, brkIx - start) +
                            next.ToString(0, limit - brkIx);
                else
                    throw new BufferException("String was outside data buffer");
            }

            internal void Mark(int limit)
            {
                if (limit > brkIx + 16) // Rotate blocks
                {
                    var temp = bldr;
                    bldr = next;
                    next = temp;
                    next.Length = 0;
                    minIx = brkIx;
                    brkIx = maxIx;
                }
            }
        }

        private readonly BufferElement data = new BufferElement();

        private int bPos; // Postion index in the StringBuilder
        private readonly BlockReader NextBlk; // Delegate that serves char-arrays;

        private string EncodingName
        {
            get
            {
                var rdr = NextBlk.Target as StreamReader;
                return (rdr == null ? "raw-bytes" : rdr.CurrentEncoding.BodyName);
            }
        }

        public BuildBuffer(Stream stream)
        {
            var fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Raw(stream);
        }

#if (!BYTEMODE)
        public BuildBuffer(Stream stream, int fallbackCodePage)
        {
            FileStream fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Get(stream, fallbackCodePage);
        }
#endif

        /// <summary>
        ///   Marks a conservative lower bound for the buffer,
        ///   allowing space to be reclaimed.  If an application 
        ///   needs to call GetString at arbitrary past locations 
        ///   in the input stream, Mark() is not called.
        /// </summary>
        public override void Mark()
        {
            data.Mark(bPos - 2);
        }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }


        /// <summary>
        ///   Read returns the ordinal number of the next char, or 
        ///   EOF (-1) for an end of stream.  Note that the next
        ///   code point may require *two* calls of Read().
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            //
            //  Characters at positions 
            //  [data.offset, data.offset + data.bldr.Length)
            //  are available in data.bldr.
            //
            if (bPos < data.MaxIndex)
            {
                // ch0 cannot be EOF
                return data[bPos++];
            }
            else // Read from underlying stream
            {
                // Experimental code, blocks of page size
                var chrs = new char[4096];
                var count = NextBlk(chrs, 0, 4096);
                if (count == 0)
                    return EndOfFile;
                else
                {
                    data.Append(chrs, count);
                    return data[bPos++];
                }
            }
        }

        public override string GetString(int begin, int limit)
        {
            return data.GetString(begin, limit);
        }

        public override string ToString()
        {
            return "StringBuilder buffer, encoding: " + this.EncodingName;
        }
    }

    // =============== End ScanBuff-derived classes ==================

    public delegate int BlockReader(char[] block, int index, int number);

    // A delegate factory, serving up a delegate that
    // reads a block of characters from the underlying
    // encoded stream, via a StreamReader object.
    //
    public static class BlockReaderFactory
    {
        public static BlockReader Raw(Stream stream)
        {
            return delegate(char[] block, int index, int number)
                       {
                           var b = new byte[number];
                           var count = stream.Read(b, 0, number);
                           var i = 0;
                           var j = index;
                           for (; i < count; i++, j++)
                               block[j] = (char) b[i];
                           return count;
                       };
        }

#if (!BYTEMODE)
        public static BlockReader Get(Stream stream, int fallbackCodePage)
        {
            Encoding encoding;
            int preamble = Preamble(stream);

            if (preamble != 0)  // There is a valid BOM here!
                encoding = Encoding.GetEncoding(preamble);
            else if (fallbackCodePage == -1) // Fallback is "raw" bytes
                return Raw(stream);
            else if (fallbackCodePage != -2) // Anything but "guess"
                encoding = Encoding.GetEncoding(fallbackCodePage);
            else // This is the "guess" option
            {
                int guess = new Guesser(stream).GuessCodePage();
                stream.Seek(0, SeekOrigin.Begin);
                if (guess == -1) // ==> this is a 7-bit file
                    encoding = Encoding.ASCII;
                else if (guess == 65001)
                    encoding = Encoding.UTF8;
                else             // ==> use the machine default
                    encoding = Encoding.Default;
            }
            StreamReader reader = new StreamReader(stream, encoding);
            return reader.Read;
        }

        static int Preamble(Stream stream)
        {
            int b0 = stream.ReadByte();
            int b1 = stream.ReadByte();

            if (b0 == 0xfe && b1 == 0xff)
                return 1201; // UTF16BE
            if (b0 == 0xff && b1 == 0xfe)
                return 1200; // UTF16LE

            int b2 = stream.ReadByte();
            if (b0 == 0xef && b1 == 0xbb && b2 == 0xbf)
                return 65001; // UTF8
            //
            // There is no unicode preamble, so we
            // return denoter for the machine default.
            //
            stream.Seek(0, SeekOrigin.Begin);
            return 0;
        }
#endif
        // !BYTEMODE
    }

    #endregion Buffer classes

    // ==============================================================
    // ============      class CodePageHandling         =============
    // ==============================================================

    public static class CodePageHandling
    {
        public static int GetCodePage(string option)
        {
            var command = option.ToUpperInvariant();
            if (command.StartsWith("CodePage:", StringComparison.OrdinalIgnoreCase))
                command = command.Substring(9);
            try
            {
                if (command.Equals("RAW"))
                    return -1;
                else if (command.Equals("GUESS"))
                    return -2;
                else if (command.Equals("DEFAULT"))
                    return 0;
                else if (char.IsDigit(command[0]))
                    return int.Parse(command, CultureInfo.InvariantCulture);
                else
                {
                    var enc = Encoding.GetEncoding(command);
                    return enc.CodePage;
                }
            }
            catch (FormatException)
            {
                Console.Error.WriteLine(
                    "Invalid format \"{0}\", using machine default", option);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine(
                    "Unknown code page \"{0}\", using machine default", option);
            }
            return 0;
        }
    }

    #region guesser

#if (!BYTEMODE)
    // ==============================================================
    // ============          Encoding Guesser           =============
    // ==============================================================

    internal class Guesser
    {
        ScanBuff buffer;

        public int GuessCodePage() { return Scan(); }

        const int maxAccept = 10;
        const int initial = 0;
        const int eofNum = 0;
        const int goStart = -1;
        const int INITIAL = 0;
        const int EndToken = 0;

        #region user code
        /* 
         *  Reads the bytes of a file to determine if it is 
         *  UTF-8 or a single-byte code page file.
         */
        public long utfX;
        public long uppr;
        #endregion user code

        int state;
        int currentStart = startState[0];
        int code;

        #region ScannerTables
        static int[] startState = new int[] { 11, 0 };

        #region CharacterMap
        static sbyte[] map = new sbyte[256] {
/*     '\0' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x10' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x20' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '0' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '@' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      'P' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '`' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      'p' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x80' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\x90' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xA0' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xB0' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xC0' */ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
/*   '\xD0' */ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
/*   '\xE0' */ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 
/*   '\xF0' */ 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5 };
        #endregion

        static sbyte[][] nextState = new sbyte[][] {
            new sbyte[] {0, 0, 0, 0, 0, 0},
            new sbyte[] {-1, -1, 10, -1, -1, -1},
            new sbyte[] {-1, -1, -1, -1, -1, -1},
            new sbyte[] {-1, -1, 8, -1, -1, -1},
            new sbyte[] {-1, -1, 5, -1, -1, -1},
            new sbyte[] {-1, -1, 6, -1, -1, -1},
            new sbyte[] {-1, -1, 7, -1, -1, -1},
            null,
            new sbyte[] {-1, -1, 9, -1, -1, -1},
            null,
            null,
            new sbyte[] {-1, 1, 2, 3, 4, 2}
        };


        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        // Reason for suppression: cannot have self-reference in array initializer.
        static Guesser()
        {
            nextState[7] = nextState[2];
            nextState[9] = nextState[2];
            nextState[10] = nextState[2];
        }

        int NextState()
        {
            if (code == ScanBuff.EndOfFile)
                return eofNum;
            else
                return nextState[state][map[code]];
        }
        #endregion

        public Guesser(System.IO.Stream file) { SetSource(file); }

        public void SetSource(System.IO.Stream source)
        {
            this.buffer = new BuildBuffer(source);
            code = buffer.Read();
        }

        int Scan()
        {
            for (; ; )
            {
                int next;
                state = currentStart;
                while ((next = NextState()) == goStart)
                    code = buffer.Read();

                state = next;
                code = buffer.Read();

                while ((next = NextState()) > eofNum)
                {
                    state = next;
                    code = buffer.Read();
                }
                if (state <= maxAccept)
                {
                    #region ActionSwitch
#pragma warning disable 162
                    switch (state)
                    {
                        case eofNum:
                            switch (currentStart)
                            {
                                case 11:
                                    if (utfX == 0 && uppr == 0) return -1; /* raw ascii */
                                    else if (uppr * 10 > utfX) return 0;   /* default code page */
                                    else return 65001;                     /* UTF-8 encoding */
                                    break;
                            }
                            return EndToken;
                        case 1: // Recognized '{Upper128}',	Shortest string "\xC0"
                        case 2: // Recognized '{Upper128}',	Shortest string "\x80"
                        case 3: // Recognized '{Upper128}',	Shortest string "\xE0"
                        case 4: // Recognized '{Upper128}',	Shortest string "\xF0"
                            uppr++;
                            break;
                        case 5: // Recognized '{Utf8pfx4}{Utf8cont}',	Shortest string "\xF0\x80"
                            uppr += 2;
                            break;
                        case 6: // Recognized '{Utf8pfx4}{Utf8cont}{2}',	Shortest string "\xF0\x80\x80"
                            uppr += 3;
                            break;
                        case 7: // Recognized '{Utf8pfx4}{Utf8cont}{3}',	Shortest string "\xF0\x80\x80\x80"
                            utfX += 3;
                            break;
                        case 8: // Recognized '{Utf8pfx3}{Utf8cont}',	Shortest string "\xE0\x80"
                            uppr += 2;
                            break;
                        case 9: // Recognized '{Utf8pfx3}{Utf8cont}{2}',	Shortest string "\xE0\x80\x80"
                            utfX += 2;
                            break;
                        case 10: // Recognized '{Utf8pfx2}{Utf8cont}',	Shortest string "\xC0\x80"
                            utfX++;
                            break;
                        default:
                            break;
                    }
#pragma warning restore 162
                    #endregion
                }
            }
        }
    }
    // end class Guesser

#endif
    // !BYTEMODE

    #endregion

// End of code copied from embedded resource
}

// end namespace