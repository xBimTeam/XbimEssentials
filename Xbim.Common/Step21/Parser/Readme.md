## The xbim Step21 Parser

**Remember to run makeparser.bat to generate an updated Scanner/Parser from the lex and yacc (*.y) source**

GPLEX and GPPG (Gardens Point Parser Generator) is now maintained at https://github.com/ernstc/gplex

For the original manual on GPLEX see https://github.com/ernstc/gplex/blob/main/GPLEX/gplex.pdf

We use a relatively old version (1.2.2) of GPLEX but have tweaked the output to support parsing files > 2GB by changing `int` indexes to `long`s
in the generated code. Since we've not backported this to the origin but need to support regneration of the Parser code, this is the strategy:

The _gplex.frame_ file contains the extracted template code normally embedded in gplex.exe incorporating the int -> long fixes. 
This ends up generating Step21Lex.cs with the 'long' amendments upon any regeneration since we pass in `/frame:gplex.frame` in the args in makeparser.bak.

_GplexBuffers.cs_ is the 'static' GPLEX dependencies of the Scanner in Step21Lex.cs - which have also had the int->long fix applied manually. 
This file is now generated as a distinct file by gplex.exe (because we specify '/noEmbedBuffers' args to gplex so the classes are not mergeded into 
the re-generated Step21Lex.cs file).
In order to retain our changes it's important this buffers file is not committed. As such we roll back the edits to 'GplexBuffers.cs' in the batch script.