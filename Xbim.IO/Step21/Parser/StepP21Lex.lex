/*
   A simple example of using GPLEX to implement the unix "strings" 
   functionality.  Reads a (possibly binary) file, finding sequences
   of alphabetic ASCII characters.
 */
     
%namespace Xbim.IO.Parser

%option verbose, summary

%{
	public static int Pass = 1;
	public static bool emitPass = true;
	public static bool comment = false;
	public void SetValue()
	{
		if (!comment) {
		yylval.strVal=yytext;
		
		}
	}
%}

%%

%{
		
%}
"\t"	    {;}
" "		    {;}
[\n]		{;} 
[\r]        {;} 
[\0]+		{;} 
#[0-9]+/=		    { if (!comment) {SetValue(); return((int)Tokens.ENTITY); }}
#[0-9]+[ \t]*/=	    { if (!comment) {SetValue(); return((int)Tokens.ENTITY); }}
#[0-9]+		        { if (!comment) {SetValue(); return((int)Tokens.IDENTITY);} }
[\-\+0-9][0-9]*	    { if (!comment) {SetValue();  return((int)Tokens.INTEGER); } }
[\-\+\.0-9][\.0-9]+	{ if (!comment) {SetValue(); return((int)Tokens.FLOAT); } }
[\-\+\.0-9][\.0-9]+E[\-\+0-9][0-9]* {if (!comment) { SetValue(); return((int)Tokens.FLOAT); } }
[\']([\n]|[\000\011-\046\050-\176\201-\237\240-\377]|[\047][\047])*[\']	{ if (!comment) { SetValue();  return((int)Tokens.STRING); } }
[\"][0-9A-F]+[\"] 	{if (!comment) {SetValue(); return((int)Tokens.HEXA); } }
[\.][TF][\.]	    {if (!comment) {SetValue(); return((int)Tokens.BOOLEAN); } }
[\.][U][\.]	        {if (!comment) {return((int)Tokens.NONDEF); } }
[\.][A-Z0-9_]+[\.]	{if (!comment) {SetValue(); return((int)Tokens.ENUM); } }
[$]		            {if (!comment) {return((int)Tokens.NONDEF); } }
[(]		{ if (!comment) return ('('); }
[)]		{ if (!comment) return (')'); }
[,]		{ if (!comment) return (','); }
[\*]	{ if (!comment)return((int)Tokens.OVERRIDE);  }
[=]		{ if (!comment) return ('='); }
[;]		{ if (!comment) return (';'); }
"/*"		{ comment=true;  }
"*/"		{ comment=false;  }

STEP;		{ if (!comment) return((int)Tokens.ISOSTEPSTART); }
HEADER;		{ if (!comment) return((int)Tokens.HEADER); }
ENDSEC;		{ if (!comment) return((int)Tokens.ENDSEC); }
DATA;		{ if (!comment) return((int)Tokens.DATA); }
ENDSTEP;	{ comment = false;  return((int)Tokens.ISOSTEPEND); }
"ENDSTEP;".*	{ comment = false;  return((int)Tokens.ISOSTEPEND); }
"END-ISO"[0-9\-]*;	{ comment = false;  return((int)Tokens.ISOSTEPEND); }
ISO[0-9\-]*;	{ comment = false;  return((int)Tokens.ISOSTEPSTART); }

[/]		{ if (!comment) return ('/'); }
&SCOPE		{ if (!comment) return((int)Tokens.SCOPE); }
ENDSCOPE	{ if (!comment) return((int)Tokens.ENDSCOPE); }
[a-zA-Z0-9_]+	{  if (!comment) { SetValue(); return((int)Tokens.TYPE); }}
![a-zA-Z0-9_]+	{ SetValue();  if (!comment) {SetValue(); return((int)Tokens.TYPE); }}
[^)]		{if (!comment) { SetValue();  return((int)Tokens.MISC); } }


%%

