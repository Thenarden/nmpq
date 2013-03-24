pushd nuget

mkdir lib
del /Q /S lib\*
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild /v:n /p:Configuration=Release ..\Nmpq\Nmpq.csproj
ILMerge.exe /out:lib\Nmpq.dll /t:library ..\Nmpq\bin\Release\Nmpq.dll ..\Nmpq\bin\Release\ICSharpCode.SharpZipLib.dll

popd