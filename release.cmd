@echo:
@echo off
echo Releasing Glue 
IF exist %1 ( echo    Copying Glue to existing directory: %1 ) ELSE ( mkdir %1 && echo    Copying glue to new directory: %1)
echo    Only new files will be copied...
@echo on
xcopy /Y /D Tube\bin\release\*.exe %1
xcopy /Y /D Tube\bin\release\*.dll %1
xcopy /Y /D Tube\bin\release\*.wav %1
xcopy /Y /D Tube\bin\release\*.glue %1
xcopy /Y /D Tube\bin\release\*.cmd %1
xcopy /Y /D Tube\bin\release\*.config %1
@echo:
