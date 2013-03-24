pushd nuget

mkdir build
del /Q /S build\*
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild /v:n /p:Configuration=Release ..\Nmpq\Nmpq.csproj
ILMerge.exe /out:build\Nmpq.dll /t:library ..\Nmpq\bin\Release\Nmpq.dll ..\Nmpq\bin\Release\ICSharpCode.SharpZipLib.dll



popd