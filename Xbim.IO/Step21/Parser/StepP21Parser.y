%{
 public bool InHeader = false;
%}
%namespace Xbim.IO.Parser
%partial   
%parsertype P21Parser

%start stepFile

%union{
		public string strVal;
	  }
%token 	ISOSTEPSTART	
%token	HEADER	
%token	ENDSEC	
%token	DATA	
%token	ISOSTEPEND	
%token	SCOPE	
%token	ENDSCOPE	
%token	ENTITY	
%token	TYPE	
%token	INTEGER	
%token	FLOAT	
%token	STRING	
%token	BOOLEAN	
%token	IDENTITY	
%token	TEXT	
%token	NONDEF	
%token	OVERRIDE	
%token	ENUM	
%token	HEXA	
%token  ILLEGALCHAR
%token	MISC	

%%
trailingSpace	: ' '
	| trailingSpace ' ' ;
endStep	: ISOSTEPEND{EndParse();}
	| ISOSTEPEND trailingSpace 
	{EndParse();}
	;	
beginStep : ISOSTEPSTART	
    {BeginParse(); }
   ;
   
    
startHeader : HEADER
    {InHeader=true; BeginHeader();}
    ;
    

stepFile1	: beginStep startHeader headerEntities endSec endOfHeader model endSec endStep;
stepFile2	: beginStep startHeader endSec endOfHeader model endSec endStep ;
stepFile3	: beginStep startHeader endSec endOfHeader model error ;
stepFile	: stepFile1 | stepFile2 | stepFile3 | model;

endSec	: ENDSEC{EndSec();}
	| ENDSEC trailingSpace {EndSec();}	
	;  
	
headerEntities	: headerEntity
	| headerEntities headerEntity
	;
headerEntity : entityType listArgument ';' {EndHeaderEntity();}
	| error  			
	;
endOfHeader : DATA
	{  InHeader=false; EndHeader();  }
	;
argument	
    : IDENTITY		            {SetObjectValue(CurrentSemanticValue.strVal);}
	| INTEGER		            {SetIntegerValue(CurrentSemanticValue.strVal);}
	| FLOAT		                {SetFloatValue(CurrentSemanticValue.strVal);}
	| STRING		            {SetStringValue(CurrentSemanticValue.strVal);}
	| BOOLEAN		            {SetBooleanValue(CurrentSemanticValue.strVal);}
	| ENUM		                {SetEnumValue(CurrentSemanticValue.strVal);}
	| HEXA		                {SetHexValue(CurrentSemanticValue.strVal);}
	| NONDEF		            {SetNonDefinedValue();}
	| OVERRIDE		            {SetOverrideValue();}          
	| listArgument	       
	| listType listArgument		{EndNestedType(CurrentSemanticValue.strVal);}
	;
	
listType	: TYPE
	{  BeginNestedType(CurrentSemanticValue.strVal);  }
	;
beginList	: '('
	{  BeginList(); }
	;
endList	: ')'
	{ EndList(); }
	;
listArgument	: beginList endList		
	| beginList argumentList endList	
	| beginList error	
	;
argumentList	: argument
	| argumentList ',' argument
	| argumentList error {SetErrorMessage();}
	;
model	: bloc
	| model bloc
	;
bloc	: entityLabel '=' entity ';' {EndEntity();}
	| entityLabel '=' beginScope model endScope entity ';'{EndEntity();}
	| entityLabel '=' beginScope endScope entity ';'{EndEntity();}
	| error		{SetErrorMessage();EndEntity();}
	;
complex	: entityType listArgument /*{EndComplex();} */
	| complex entityType listArgument /* {EndComplex();} */        
	;
entity   : entityType listArgument               /*    Simple Entity    */
	| '(' complex ')' /*{BeginComplex();}*/                 /*    Complex  */
	;
beginScope	: SCOPE
	/*{  BeginScope();  }*/
	;
uniqueID	: IDENTITY
	{   SetObjectValue(CurrentSemanticValue.strVal);  }
	;
export	: uniqueID
	| export ',' uniqueID
	;
beginExport	: '/'
	{  BeginList();  }
	;
endScope	: ENDSCOPE
	/*{  EndScope();  }*/
	| ENDSCOPE beginExport export '/'
	/*{  Console.WriteLine("***  Warning : Export List not yet processed\n");
	   NewEntity();  EndScope() ; }*/
	;
entityLabel	: ENTITY
	{  NewEntity(CurrentSemanticValue.strVal);  }
	;
entityType	: TYPE
	{  SetType(CurrentSemanticValue.strVal);  }
	;
entityType	: ILLEGALCHAR
	{  CharacterError();  }
	;

%%


