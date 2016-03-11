#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    StepP21Parser.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using System.Globalization;
using QUT.Gppg;

#endregion

namespace Xbim.IO.Parser
{
    public enum Tokens
    {
        error = 62,
        EOF = 63,
        ISOSTEPSTART = 64,
        HEADER = 65,
        ENDSEC = 66,
        DATA = 67,
        ISOSTEPEND = 68,
        SCOPE = 69,
        ENDSCOPE = 70,
        ENTITY = 71,
        TYPE = 72,
        INTEGER = 73,
        FLOAT = 74,
        STRING = 75,
        BOOLEAN = 76,
        IDENTITY = 77,
        TEXT = 78,
        NONDEF = 79,
        OVERRIDE = 80,
        ENUM = 81,
        HEXA = 82,
        ILLEGALCHAR = 83,
        MISC = 84
    };

    public struct ValueType
#line 10 "StepP21Parser.y"
    {
#line 11 "StepP21Parser.y"
        public string strVal;
#line 12 "StepP21Parser.y"
    }

// Abstract base class for GPLEX scanners
    public abstract class ScanBase : AbstractScanner<ValueType, LexLocation>
    {
        private LexLocation __yylloc = new LexLocation();

        public override LexLocation yylloc
        {
            get { return __yylloc; }
            set { __yylloc = value; }
        }

        protected virtual bool yywrap()
        {
            return true;
        }
    }

    public partial class P21Parser : ShiftReduceParser<ValueType, LexLocation>
    {
#line 2 "StepP21Parser.y"
        public bool InHeader;
#pragma warning disable 649
        private Dictionary<int, string> aliasses;
#pragma warning restore 649

        protected override void Initialize()
        {
            this.InitSpecialTokens((int) Tokens.error, (int) Tokens.EOF);

            this.InitStateTable(96);
            AddState(0, new State(new[] {64, 93}, new[] {-1, 1, -7, 3, -5, 4, -12, 94, -13, 95}));
            AddState(1, new State(new[] {63, 2}));
            AddState(2, new State(-1));
            AddState(3, new State(-11));
            AddState(4, new State(new[] {65, 92}, new[] {-6, 5}));
            AddState(5, new State(new[] {72, 54, 83, 55, 62, 84, 66, 17}, new[] {-8, 6, -9, 85, -14, 91, -15, 81}));
            AddState(6, new State(new[] {66, 17, 72, 54, 83, 55, 62, 84}, new[] {-9, 7, -14, 80, -15, 81}));
            AddState(7, new State(new[] {67, 79}, new[] {-10, 8}));
            AddState(8, new State(new[] {71, 72, 62, 73}, new[] {-11, 9, -22, 77, -23, 19}));
            AddState(9, new State(new[] {66, 17, 71, 72, 62, 73}, new[] {-9, 10, -22, 16, -23, 19}));
            AddState(10, new State(new[] {68, 12}, new[] {-4, 11}));
            AddState(11, new State(-8));
            AddState(12, new State(new[] {32, 15, 63, -4}, new[] {-2, 13}));
            AddState(13, new State(new[] {32, 14, 63, -5}));
            AddState(14, new State(-3));
            AddState(15, new State(-2));
            AddState(16, new State(-42));
            AddState(17, new State(new[] {32, 15, 67, -14, 68, -14}, new[] {-2, 18}));
            AddState(18, new State(new[] {32, 14, 67, -15, 68, -15}));
            AddState(19, new State(new[] {61, 20}));
            AddState(20, new State(new[] {72, 54, 83, 55, 40, 56, 69, 78}, new[] {-24, 21, -25, 23, -15, 28}));
            AddState(21, new State(new[] {59, 22}));
            AddState(22, new State(-43));
            AddState(23, new State(new[] {71, 72, 62, 73, 70, 63}, new[] {-11, 24, -26, 74, -22, 77, -23, 19}));
            AddState(24, new State(new[] {70, 63, 71, 72, 62, 73}, new[] {-26, 25, -22, 16, -23, 19}));
            AddState(25, new State(new[] {72, 54, 83, 55, 40, 56}, new[] {-24, 26, -15, 28}));
            AddState(26, new State(new[] {59, 27}));
            AddState(27, new State(-44));
            AddState(28, new State(new[] {40, 46}, new[] {-16, 29, -19, 30}));
            AddState(29, new State(-49));
            AddState(30,
                     new State(
                         new[]
                             {
                                 62, 52, 41, 51, 77, 36, 73, 37, 74, 38, 75, 39, 76, 40, 81, 41, 82, 42, 79, 43, 80, 44,
                                 40
                                 , 46, 72, 49
                             }, new[] {-20, 31, -21, 32, -17, 53, -16, 45, -19, 30, -18, 47}));
            AddState(31, new State(-35));
            AddState(32, new State(new[] {44, 34, 62, 50, 41, 51}, new[] {-20, 33}));
            AddState(33, new State(-36));
            AddState(34,
                     new State(
                         new[] {77, 36, 73, 37, 74, 38, 75, 39, 76, 40, 81, 41, 82, 42, 79, 43, 80, 44, 40, 46, 72, 49},
                         new[] {-17, 35, -16, 45, -19, 30, -18, 47}));
            AddState(35, new State(-39));
            AddState(36, new State(-21));
            AddState(37, new State(-22));
            AddState(38, new State(-23));
            AddState(39, new State(-24));
            AddState(40, new State(-25));
            AddState(41, new State(-26));
            AddState(42, new State(-27));
            AddState(43, new State(-28));
            AddState(44, new State(-29));
            AddState(45, new State(-30));
            AddState(46, new State(-33));
            AddState(47, new State(new[] {40, 46}, new[] {-16, 48, -19, 30}));
            AddState(48, new State(-31));
            AddState(49, new State(-32));
            AddState(50, new State(-40));
            AddState(51, new State(-34));
            AddState(52, new State(-37));
            AddState(53, new State(-38));
            AddState(54, new State(-59));
            AddState(55, new State(-60));
            AddState(56, new State(new[] {72, 54, 83, 55}, new[] {-27, 57, -15, 61}));
            AddState(57, new State(new[] {41, 58, 72, 54, 83, 55}, new[] {-15, 59}));
            AddState(58, new State(-50));
            AddState(59, new State(new[] {40, 46}, new[] {-16, 60, -19, 30}));
            AddState(60, new State(-48));
            AddState(61, new State(new[] {40, 46}, new[] {-16, 62, -19, 30}));
            AddState(62, new State(-47));
            AddState(63, new State(new[] {47, 71, 72, -56, 83, -56, 40, -56}, new[] {-30, 64}));
            AddState(64, new State(new[] {77, 69}, new[] {-29, 65, -28, 70}));
            AddState(65, new State(new[] {47, 66, 44, 67}));
            AddState(66, new State(-57));
            AddState(67, new State(new[] {77, 69}, new[] {-28, 68}));
            AddState(68, new State(-54));
            AddState(69, new State(-52));
            AddState(70, new State(-53));
            AddState(71, new State(-55));
            AddState(72, new State(-58));
            AddState(73, new State(-46));
            AddState(74, new State(new[] {72, 54, 83, 55, 40, 56}, new[] {-24, 75, -15, 28}));
            AddState(75, new State(new[] {59, 76}));
            AddState(76, new State(-45));
            AddState(77, new State(-41));
            AddState(78, new State(-51));
            AddState(79, new State(-20));
            AddState(80, new State(-17));
            AddState(81, new State(new[] {40, 46}, new[] {-16, 82, -19, 30}));
            AddState(82, new State(new[] {59, 83}));
            AddState(83, new State(-18));
            AddState(84, new State(-19));
            AddState(85, new State(new[] {67, 79}, new[] {-10, 86}));
            AddState(86, new State(new[] {71, 72, 62, 73}, new[] {-11, 87, -22, 77, -23, 19}));
            AddState(87, new State(new[] {62, 90, 66, 17, 71, 72}, new[] {-9, 88, -22, 16, -23, 19}));
            AddState(88, new State(new[] {68, 12}, new[] {-4, 89}));
            AddState(89, new State(-9));
            AddState(90, new State(new[] {63, -10, 62, -46, 66, -46, 71, -46}));
            AddState(91, new State(-16));
            AddState(92, new State(-7));
            AddState(93, new State(-6));
            AddState(94, new State(-12));
            AddState(95, new State(-13));

            var rules = new Rule[61];
            rules[1] = new Rule(-3, new[] {-1, 63});
            rules[2] = new Rule(-2, new[] {32});
            rules[3] = new Rule(-2, new[] {-2, 32});
            rules[4] = new Rule(-4, new[] {68});
            rules[5] = new Rule(-4, new[] {68, -2});
            rules[6] = new Rule(-5, new[] {64});
            rules[7] = new Rule(-6, new[] {65});
            rules[8] = new Rule(-7, new[] {-5, -6, -8, -9, -10, -11, -9, -4});
            rules[9] = new Rule(-12, new[] {-5, -6, -9, -10, -11, -9, -4});
            rules[10] = new Rule(-13, new[] {-5, -6, -9, -10, -11, 62});
            rules[11] = new Rule(-1, new[] {-7});
            rules[12] = new Rule(-1, new[] {-12});
            rules[13] = new Rule(-1, new[] {-13});
            rules[14] = new Rule(-9, new[] {66});
            rules[15] = new Rule(-9, new[] {66, -2});
            rules[16] = new Rule(-8, new[] {-14});
            rules[17] = new Rule(-8, new[] {-8, -14});
            rules[18] = new Rule(-14, new[] {-15, -16, 59});
            rules[19] = new Rule(-14, new[] {62});
            rules[20] = new Rule(-10, new[] {67});
            rules[21] = new Rule(-17, new[] {77});
            rules[22] = new Rule(-17, new[] {73});
            rules[23] = new Rule(-17, new[] {74});
            rules[24] = new Rule(-17, new[] {75});
            rules[25] = new Rule(-17, new[] {76});
            rules[26] = new Rule(-17, new[] {81});
            rules[27] = new Rule(-17, new[] {82});
            rules[28] = new Rule(-17, new[] {79});
            rules[29] = new Rule(-17, new[] {80});
            rules[30] = new Rule(-17, new[] {-16});
            rules[31] = new Rule(-17, new[] {-18, -16});
            rules[32] = new Rule(-18, new[] {72});
            rules[33] = new Rule(-19, new[] {40});
            rules[34] = new Rule(-20, new[] {41});
            rules[35] = new Rule(-16, new[] {-19, -20});
            rules[36] = new Rule(-16, new[] {-19, -21, -20});
            rules[37] = new Rule(-16, new[] {-19, 62});
            rules[38] = new Rule(-21, new[] {-17});
            rules[39] = new Rule(-21, new[] {-21, 44, -17});
            rules[40] = new Rule(-21, new[] {-21, 62});
            rules[41] = new Rule(-11, new[] {-22});
            rules[42] = new Rule(-11, new[] {-11, -22});
            rules[43] = new Rule(-22, new[] {-23, 61, -24, 59});
            rules[44] = new Rule(-22, new[] {-23, 61, -25, -11, -26, -24, 59});
            rules[45] = new Rule(-22, new[] {-23, 61, -25, -26, -24, 59});
            rules[46] = new Rule(-22, new[] {62});
            rules[47] = new Rule(-27, new[] {-15, -16});
            rules[48] = new Rule(-27, new[] {-27, -15, -16});
            rules[49] = new Rule(-24, new[] {-15, -16});
            rules[50] = new Rule(-24, new[] {40, -27, 41});
            rules[51] = new Rule(-25, new[] {69});
            rules[52] = new Rule(-28, new[] {77});
            rules[53] = new Rule(-29, new[] {-28});
            rules[54] = new Rule(-29, new[] {-29, 44, -28});
            rules[55] = new Rule(-30, new[] {47});
            rules[56] = new Rule(-26, new[] {70});
            rules[57] = new Rule(-26, new[] {70, -30, -29, 47});
            rules[58] = new Rule(-23, new[] {71});
            rules[59] = new Rule(-15, new[] {72});
            rules[60] = new Rule(-15, new[] {83});
            this.InitRules(rules);

            this.InitNonTerminals(new[]
                                      {
                                          "", "stepFile", "trailingSpace", "$accept",
                                          "endStep", "beginStep", "startHeader", "stepFile1", "headerEntities", "endSec"
                                          ,
                                          "endOfHeader", "model", "stepFile2", "stepFile3", "headerEntity", "entityType"
                                          ,
                                          "listArgument", "argument", "listType", "beginList", "endList", "argumentList"
                                          ,
                                          "bloc", "entityLabel", "entity", "beginScope", "endScope", "complex",
                                          "uniqueID",
                                          "export", "beginExport",
                                      });
        }

        protected override void DoAction(int action)
        {
            switch (action)
            {
                case 4: // endStep -> ISOSTEPEND
#line 38 "StepP21Parser.y"
                    {
                        EndParse();
                    }
                    break;
                case 5: // endStep -> ISOSTEPEND, trailingSpace
#line 40 "StepP21Parser.y"
                    {
                        EndParse();
                    }
                    break;
                case 6: // beginStep -> ISOSTEPSTART
#line 43 "StepP21Parser.y"
                    {
                        BeginParse();
                    }
                    break;
                case 7: // startHeader -> HEADER
#line 48 "StepP21Parser.y"
                    {
                        InHeader = true;
                        BeginHeader();
                    }
                    break;
                case 14: // endSec -> ENDSEC
#line 57 "StepP21Parser.y"
                    {
                        EndSec();
                    }
                    break;
                case 15: // endSec -> ENDSEC, trailingSpace
#line 58 "StepP21Parser.y"
                    {
                        EndSec();
                    }
                    break;
                case 18: // headerEntity -> entityType, listArgument, ';'
#line 64 "StepP21Parser.y"
                    {
                        EndHeaderEntity();
                    }
                    break;
                case 20: // endOfHeader -> DATA
#line 68 "StepP21Parser.y"
                    {
                        InHeader = false;
                        EndHeader();
                    }
                    break;
                case 21: // argument -> IDENTITY
#line 71 "StepP21Parser.y"
                    {
                        SetObjectValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 22: // argument -> INTEGER
#line 72 "StepP21Parser.y"
                    {
                        SetIntegerValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 23: // argument -> FLOAT
#line 73 "StepP21Parser.y"
                    {
                        SetFloatValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 24: // argument -> STRING
#line 74 "StepP21Parser.y"
                    {
                        SetStringValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 25: // argument -> BOOLEAN
#line 75 "StepP21Parser.y"
                    {
                        SetBooleanValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 26: // argument -> ENUM
#line 76 "StepP21Parser.y"
                    {
                        SetEnumValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 27: // argument -> HEXA
#line 77 "StepP21Parser.y"
                    {
                        SetHexValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 28: // argument -> NONDEF
#line 78 "StepP21Parser.y"
                    {
                        SetNonDefinedValue();
                    }
                    break;
                case 29: // argument -> OVERRIDE
#line 79 "StepP21Parser.y"
                    {
                        SetOverrideValue();
                    }
                    break;
                case 31: // argument -> listType, listArgument
#line 81 "StepP21Parser.y"
                    {
                        EndNestedType(CurrentSemanticValue.strVal);
                    }
                    break;
                case 32: // listType -> TYPE
#line 85 "StepP21Parser.y"
                    {
                        BeginNestedType(CurrentSemanticValue.strVal);
                    }
                    break;
                case 33: // beginList -> '('
#line 88 "StepP21Parser.y"
                    {
                        BeginList();
                    }
                    break;
                case 34: // endList -> ')'
#line 91 "StepP21Parser.y"
                    {
                        EndList();
                    }
                    break;
                case 40: // argumentList -> argumentList, error
#line 99 "StepP21Parser.y"
                    {
                        SetErrorMessage();
                    }
                    break;
                case 43: // bloc -> entityLabel, '=', entity, ';'
#line 104 "StepP21Parser.y"
                    {
                        EndEntity();
                    }
                    break;
                case 44: // bloc -> entityLabel, '=', beginScope, model, endScope, entity, ';'
#line 105 "StepP21Parser.y"
                    {
                        EndEntity();
                    }
                    break;
                case 45: // bloc -> entityLabel, '=', beginScope, endScope, entity, ';'
#line 106 "StepP21Parser.y"
                    {
                        EndEntity();
                    }
                    break;
                case 46: // bloc -> error
#line 107 "StepP21Parser.y"
                    {
                        SetErrorMessage();
                        EndEntity();
                    }
                    break;
                case 52: // uniqueID -> IDENTITY
#line 119 "StepP21Parser.y"
                    {
                        SetObjectValue(CurrentSemanticValue.strVal);
                    }
                    break;
                case 55: // beginExport -> '/'
#line 125 "StepP21Parser.y"
                    {
                        BeginList();
                    }
                    break;
                case 58: // entityLabel -> ENTITY
#line 134 "StepP21Parser.y"
                    {
                        NewEntity(CurrentSemanticValue.strVal);
                    }
                    break;
                case 59: // entityType -> TYPE
#line 137 "StepP21Parser.y"
                    {
                        SetType(CurrentSemanticValue.strVal);
                    }
                    break;
                case 60: // entityType -> ILLEGALCHAR
#line 140 "StepP21Parser.y"
                    {
                        CharacterError();
                    }
                    break;
            }
        }

        protected override string TerminalToString(int terminal)
        {
            if (aliasses != null && aliasses.ContainsKey(terminal))
                return aliasses[terminal];
            else if (((Tokens) terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
                return ((Tokens) terminal).ToString();
            else
                return CharToString((char) terminal);
        }

#line 144 "StepP21Parser.y"

#line 145 "StepP21Parser.y"
    }
}