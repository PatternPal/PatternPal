@echo off

rem Update the tool if the first argument equals 'update'.
if "%1"=="update" (
    set COMMAND=%1
) else (
    set COMMAND="install"
)

rem Get the root directory of the repository.
set REPO_ROOT_DIR="%~dp0.."

rem Switch to the directory of the test runner tool.
cd "%REPO_ROOT_DIR%\PatternPal\PatternPal.TestRunner"

rem Pack the tool (required to use it as a dotnet tool).
dotnet pack

rem Install the tool.
dotnet tool %COMMAND% --global --add-source .\bin\Release\ Patternpal.Testrunner
