
@ECHO OFF

echo:
echo ===================================
echo Sparkle.LinkedInNET package builder
echo ===================================
echo:

set currentDirectory=%CD%
cd ..
cd build
set outputDirectory=%CD%
cd %currentDirectory%
set nuget=%CD%\..\tools\nuget.exe
set msbuild4="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
set vincrement=%CD%\..\tools\Vincrement.exe


echo Check CLI apps
echo -----------------------------

if not exist %nuget% (
 echo ERROR: nuget could not be found, verify path. exiting.
 echo Configured as: %nuget%
 pause
 exit
)

if not exist %msbuild4% (
 echo ERROR: msbuild 4 could not be found, verify path. exiting.
 echo Configured as: %msbuild4%
 pause
 exit
)

echo Everything is fine.

echo:
echo Clean output directory
echo -----------------------------
if exist lib (
 rmdir /s /q lib
 if not %ERRORLEVEL% == 0 (
  echo ERROR: clean failed. exiting.
  pause
  exit
 )
)
echo Done.

echo:
echo Build solution
echo -----------------------------
cd ..
cd src
set solutionDirectory=%CD%
REM     %msbuild4% Sparkle.LinkedInNET.sln /p:Configuration=Release /nologo /verbosity:q 
REM     
REM     if not %ERRORLEVEL% == 0 (
REM      echo ERROR: build failed. exiting.
REM      cd %currentDirectory%
REM      pause
REM      exit
REM     )
REM     echo Done.
echo SKIPPED!

echo:
echo Copy libs
echo -----------------------------

echo solutionDirectory = %solutionDirectory%
echo outputDirectory   = %outputDirectory%

mkdir %outputDirectory%\lib

mkdir %outputDirectory%\lib\net35
xcopy /Y /Q %solutionDirectory%\NET35.Sparkle.LinkedInNET\bin\Release\Sparkle* %outputDirectory%\lib\net35

mkdir %outputDirectory%\lib\net40
xcopy /Y /Q %solutionDirectory%\NET40.Sparkle.LinkedInNET\bin\Release\Sparkle* %outputDirectory%\lib\net40

mkdir %outputDirectory%\lib\net45
xcopy /Y /Q %solutionDirectory%\NET451.Sparkle.LinkedInNET\bin\Release\Sparkle* %outputDirectory%\lib\net45
echo Done.




echo:
echo Increment version number
echo -----------------------------

echo Hit return to continue...
pause 
%vincrement% -file=%outputDirectory%\version.txt 0.0.1 %outputDirectory%\version.txt
if not %ERRORLEVEL% == 0 (
 echo ERROR: vincrement. exiting.
 cd %currentDirectory%
 pause
 exit
)
set /p version=<%outputDirectory%\version.txt
echo Done: %version%



echo:
echo Build NuGet package
echo -----------------------------

echo UPDATE NUSPEC FILES!
echo Hit return to continue...
pause 
cd %outputDirectory%
%nuget% pack Sparkle.LinkedInNet.nuspec -Version %version%
echo Done.




echo:
echo Push NuGet package
echo -----------------------------

echo Hit return to continue...
pause 
cd %outputDirectory%
%nuget% push Sparkle.LinkedInNet.%version%.nupkg -Source https://www.nuget.org
echo Done.





cd %currentDirectory%
pause



