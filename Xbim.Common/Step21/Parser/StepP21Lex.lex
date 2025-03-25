%namespace Xbim.IO.Parser
%using QUT.Gppg;

%x COMMENT
%option verbose, summary, noCompressNext, noPersistBuffer

%{
	public static int Pass = 1;
	public static bool emitPass = true;
	public void SetValue()
	{
		yylval.strVal=yytext;
	}
%}

whitespace             [ \t]
new_line               "\n\r"|\r|\n|"\r\n"
nullbytes              [\0]+

sign                   ("-"|"+")?

digit                  [0-9]
digit10                {digit}
digit16                [0-9a-fA-F]
uinteger10             ({digit10})+
uinteger16             ({digit16})+
integer                {sign}{uinteger10}
exponent_marker        [Ee]
exponent_suffix        ({exponent_marker}{sign}({digit10})+)?
infinite               "#INF"
not_a_number           "#IND"
ureal                  (({uinteger10}{exponent_suffix})|("."({digit10})+{exponent_suffix})|(({digit10})+"."({digit10})*{exponent_suffix}))
real                   ({sign}{ureal}({infinite}|{not_a_number})?)
hexid                    \"{uinteger16}\"

equals                 /=
entity_identifier      #{uinteger10}
entity                 {entity_identifier}{equals}
entity_ws              ({entity_identifier}({whitespace})*{equals})

single_string_char     [^\\\'\r\n]
string_escape_char     \\\\|\'\'

string_solidus_s       \\S\\[\040-\176]
string_solidus_p       \\P[A-I]\\
string_solidus_x       \\X\\({digit16}{2})
string_x_close         \\X0\\
string_solidus_x2      \\X2\\({digit16}{4})*{string_x_close}
string_solidus_x4      \\X4\\({digit16}{8})*{string_x_close}
string_encoding_char   {string_solidus_s}|{string_solidus_p}|{string_solidus_x}|{string_solidus_x2}|{string_solidus_x4}
string_invalid_tolerated         \\[^SPX\\]

reg_string_char        {single_string_char}|{string_escape_char}|{string_encoding_char}|{string_invalid_tolerated}
string_literal         \'({reg_string_char})*\'

dot                    "."
boolean                [TF]
undefined              "U"
notdefined             "$"

%%

%{
		
%}

{whitespace}           {;}
{new_line}             {;} 
{nullbytes}            {;}
{entity}               { SetValue(); return((int)Tokens.ENTITY); }
#[0-9]+[ \t]*/=	       { SetValue(); return((int)Tokens.ENTITY); }
{entity_identifier}    { SetValue(); return((int)Tokens.IDENTITY);} 
{integer}              { SetValue(); return((int)Tokens.INTEGER); } 
{real}                 { SetValue(); return((int)Tokens.FLOAT); }

{string_literal}       { SetValue();  return((int)Tokens.STRING); }

{hexid}                { SetValue(); return((int)Tokens.HEXA); } 
{dot}{boolean}{dot}    { SetValue(); return((int)Tokens.BOOLEAN); } 
{dot}{undefined}{dot}  { return((int)Tokens.NONDEF); } 
{dot}[a-zA-Z0-9_ ]+{dot} { SetValue(); return((int)Tokens.ENUM); } 
{notdefined}           { return((int)Tokens.NONDEF); } 
[(]		{ return ('('); }
[)]		{ return (')'); }
[,]		{ return (','); }
[\*]	{ return((int)Tokens.OVERRIDE);  }
[=]		{ return ('='); }
[;]		{ return (';'); }

"/*"				{ BEGIN(COMMENT);  }
<COMMENT>
{
	"*/"			{ BEGIN(INITIAL); }
}

STEP;			{ return((int)Tokens.ISOSTEPSTART); }
HEADER;			{ return((int)Tokens.HEADER); }
ENDSEC;			{ return((int)Tokens.ENDSEC); }
DATA;			{ return((int)Tokens.DATA); }
ENDSTEP;		{ return((int)Tokens.ISOSTEPEND); }
"ENDSTEP;".*	{ return((int)Tokens.ISOSTEPEND); }
"END-ISO"[0-9\-]*;	{ return((int)Tokens.ISOSTEPEND); }
ISO[0-9\-]*;	{ return((int)Tokens.ISOSTEPSTART); }

[/]				{ return ('/'); }
&SCOPE			{ return((int)Tokens.SCOPE); }
ENDSCOPE		{ return((int)Tokens.ENDSCOPE); }
[a-zA-Z0-9_]+	{ SetValue(); return((int)Tokens.TYPE); }
![a-zA-Z0-9_]+	{ SetValue(); return((int)Tokens.TYPE); }
[^)]			{ SetValue(); return((int)Tokens.MISC); } 

%{
	yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
%}

%%

