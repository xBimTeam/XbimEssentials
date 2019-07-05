cd %1
GPLEX /noPersistBuffer StepP21Lex.LEX 
GPPG /gplex StepP21Parser.y > StepP21Parser.cs
pause