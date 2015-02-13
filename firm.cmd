@echo off

if "%1"=="help" goto help

if "%1"=="init" goto init
if not exist packages call :init

if "%1"=="clean" goto clean
if "%1"=="build" goto build
if not exist bin call :build

if "%1"=="generate" goto generate
if not exist output call :generate

if "%1"=="preview" goto preview




:init
.paket\paket.exe restore
goto end

:clean
:build
packages\FAKE\tools\FAKE.exe fake.fsx %1
goto end

:generate
rem dummy for now
mkdir output
goto end

:preview
set root=%~dp0
bin\Host.exe "%root%output\\"
goto end


:help
echo Available commands:
echo   init      Installs the required paket dependencies.
echo   clean     Cleans the build directory.
echo   build     Builds the firm application.
echo   generate  Generates the web site.
echo   preview   Starts a local http server for previewing the generated blog.

:end
