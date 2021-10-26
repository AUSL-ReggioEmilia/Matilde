@ECHO OFF

rem vecchie condizioni, ora il run di questo file è regolato dalle Condition del profilo di ScciEasy.csprj
rem IF "%~1" == "Setup" GOTO DOWORK 
rem GOTO END

:DOWORK
REM controllo percorso di destinazione 
IF "%~2" == "" GOTO USAGE
IF "%~3" == "" GOTO USAGE

REM copia di file nel percorso passato come parametro 2 al percorso passato come parametro 3

FOR /f "usebackq delims=|" %%f in (`dir /b "%~2*.exe*"`) do XCOPY /Y /R "%~2%%f" "%~3"
FOR /f "usebackq delims=|" %%f in (`dir /b "%~2*.dll"`) do XCOPY /Y /R "%~2%%f" "%~3"
FOR /f "usebackq delims=|" %%f in (`dir /b "%~2*.pdb"`) do XCOPY /Y /R "%~2%%f" "%~3"
FOR /f "usebackq delims=|" %%f in (`dir /b "%~2*.vshost.*"`) do DEL "%~3%%f"

REM copia file accessori dalla directory di progetto o da condivisione K
XCOPY /Y /R "%~2Config.xml" "%~3"
XCOPY /Y /I /H /R "%~2zh-CN" "%~3zh-CN\"
XCOPY /Y /I /H /R "%~2x86" "%~3x86\"
XCOPY /Y /I /H /R "%~2x64" "%~3x64\"
XCOPY /Y /I /H /R "%~2it" "%~3it\"


GOTO END

:USAGE
ECHO Uso:
ECHO SetupSteps Setup (Percorso di origine) (Percorso di destinazione)
EXIT /B 255

:END

