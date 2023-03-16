rem Note that we redirect stdout to NUL, this means that stdout will not be printed to the console,
rem whereas stderr will still be printed to the console. This leads to less noise when building.

rem The output path where the background service will be copied to.
set OUTPUT_PATH="%2PatternPal\PatternPal.Extension\PatternPal\"

rem Remove the previous background service build output.
if exist %OUTPUT_PATH% (
    del /F /S /Q %OUTPUT_PATH% > NUL
    rmdir /S /Q %OUTPUT_PATH% > NUL
)

rem Recreate the output directory.
mkdir %OUTPUT_PATH%

rem Copy the build output of the background service to the output folder.
xcopy "%1" %OUTPUT_PATH% /S > NUL
