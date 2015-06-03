@echo off
setlocal

set root=%~dp0

if "%1"=="reset" goto :reset
if "%1"=="init" goto :init
if "%1"=="clean" goto :clean
if "%1"=="build" goto :build
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

:generate
if not exist bin call :build build
echo Generating site...
bin\Generator.exe "%root%"
goto :end

:preview
if not exist output call :generate
echo Starting HTTP server to preview site...
bin\Host.exe "%root%output\\"
echo Done!
goto :end

:help
echo Available commands:
echo   reset       Removes all downloaded and generated files/folders.
echo   init        Installs the required paket dependencies.
echo   clean       Cleans the build directory.
echo   build       Builds the firm application.
echo   generate    Generates the web site.
echo   preview     Starts a local http server for previewing the generated blog.

:end
endlocal