cd %1
GPLEX /noPersistBuffer  /noEmbedBuffers /frame:gplex.frame StepP21Lex.LEX 
GPPG /gplex StepP21Parser.y > StepP21Parser.cs

git restore GplexBuffers.cs
pause
