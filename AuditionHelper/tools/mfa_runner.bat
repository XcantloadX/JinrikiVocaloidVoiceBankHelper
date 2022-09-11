@echo off
cd {{root}}
CALL misc\src\MiniConda\condabin\conda.bat activate vvTalkEnv
mfa align {{in}} .\misc\mfa\mfa_model_v20.dict .\misc\mfa\mandarin_mfa.zip {{out}} --beam 1000 --clean