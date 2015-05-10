@echo off
setlocal

set root=%~dp0

if "%1"=="reset" goto :reset
if "%1"=="init" goto :init
if "%1"=="clean" goto :clean
if "%1"=="build" goto :build
if "%1"=="regenerate" goto :regenerate
if "%1"=="generate" goto :generate
if "%1"=="preview" goto :preview
goto :help

:reset
if exist packages (
    echo Removing packages...
    rd /q /s packages
)
if exist bin (
    echo Removing bin...
    rd /s /q bin
)
if exist output (
    echo Removing output...
    rd /s /q output
)
echo Done!
goto :end

:init
echo Initializing packages...
.paket\paket.exe restore
echo Done!
goto :end

:clean
:build
if not exist packages call :init
echo Calling FAKE with target "%1"...
packages\FAKE\tools\FAKE.exe fake.fsx %1
echo Done!
goto :end

:regenerate
if exist output (
    if exist output\.git (
        echo Backing up .git and removing output...
        if exist output-temp (
            echo Error! The directory output-temp already exists, please remove it manually before retrying regenerate.
            goto :end
        )
        md output-temp\.git
        xcopy output\.git output-temp\.git /E /H /Q /K
        rd /s /q output
        echo Restoring output\.git...
        md output\.git
        xcopy output-temp\.git output\.git /E /H /Q /K
        rd /s /q output-temp
    ) else (
        echo Removing output...
        rd /s /q output
    )
    echo Done!
)
:generate
if not exist bin call :build build
echo Generating site...
bin\Generator.exe "%root%"
goto :end

:preview
if not exist output call :generate
echo Starting HTTP server to preview site...
bin\Host.exe "%root%output\\" %2
echo Done!
goto :end

:help
echo Available commands:
echo   reset             Removes all downloaded and generated files/folders.
echo   init              Installs the required paket dependencies.
echo   clean             Cleans the build directory.
echo   build             Builds the firm application.
echo   generate          Generates the web site.
echo   regenerate        Removes the output directory and generates the web site.
echo   preview <baseUri> Starts a local http server for previewing the generated blog.

:end
endlocal