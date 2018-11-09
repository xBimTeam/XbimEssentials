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

%%

%{
		
%}
"\t"	    {;}
" "		    {;}
[\n]		{;} 
[\r]        {;} 
[\0]+		{;} 
#[0-9]+/=		    { SetValue(); return((int)Tokens.ENTITY); }
#[0-9]+[ \t]*/=	    { SetValue(); return((int)Tokens.ENTITY); }
#[0-9]+		        { SetValue(); return((int)Tokens.IDENTITY);} 
[\-\+0-9][0-9]*	    { SetValue();  return((int)Tokens.INTEGER); } 
[\-\+\.0-9][\.0-9]+((#INF)|(#IND))?	{ SetValue(); return((int)Tokens.FLOAT); } 
[\-\+\.0-9][\.0-9]+E[\-\+0-9][0-9]* { SetValue(); return((int)Tokens.FLOAT); } 
[\']([\001-\046\050-\377]|(\'\')|(\\S\\.))*[\'] { SetValue();  return((int)Tokens.STRING); } 
[\"][0-9A-Fa-f]+[\"] 	{SetValue(); return((int)Tokens.HEXA); } 
[\.][TF][\.]	    {SetValue(); return((int)Tokens.BOOLEAN); } 
[\.][U][\.]	        {return((int)Tokens.NONDEF); } 
[\.][a-zA-Z0-9_ ]+[\.]	{SetValue(); return((int)Tokens.ENUM); } 
[$]		            {return((int)Tokens.NONDEF); } 
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


STEP;		{ return((int)Tokens.ISOSTEPSTART); }
HEADER;		{ return((int)Tokens.HEADER); }
ENDSEC;		{ return((int)Tokens.ENDSEC); }
DATA;		{ return((int)Tokens.DATA); }
ENDSTEP;	{ return((int)Tokens.ISOSTEPEND); }
"ENDSTEP;".*	{ return((int)Tokens.ISOSTEPEND); }
"END-ISO"[0-9\-]*;	{ return((int)Tokens.ISOSTEPEND); }
ISO[0-9\-]*;	{ return((int)Tokens.ISOSTEPSTART); }

[/]		{ return ('/'); }
&SCOPE		{ return((int)Tokens.SCOPE); }
ENDSCOPE	{ return((int)Tokens.ENDSCOPE); }
[a-zA-Z0-9_]+	{  SetValue(); return((int)Tokens.TYPE); }
![a-zA-Z0-9_]+	{ SetValue(); return((int)Tokens.TYPE); }
[^)]		{SetValue();  return((int)Tokens.MISC); } 


%{
	yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
%}

%%

