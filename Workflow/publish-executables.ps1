dotnet publish ..\Dev\FigmaVisk\FigmaVisk.csproj -o ..\Dev\FigmaVisk\bin\Release\net5.0\publish\ -r win-x64 -c Release --self-contained true

dotnet publish ..\Dev\ViskVectorRenderer\ViskVectorRenderer.csproj -o ..\Dev\ViskVectorRenderer\bin\Release\net5.0\publish\ -r win-x64 -c Release --self-contained true

rmdir .\publish
mkdir .\publish

cp ..\Dev\FigmaVisk\bin\Release\net5.0\publish\*.exe .\publish\
cp ..\Dev\FigmaVisk\bin\Release\net5.0\publish\*.dll .\publish\
cp ..\Dev\ViskVectorRenderer\bin\Release\net5.0\publish\*.exe .\publish\
cp ..\Dev\ViskVectorRenderer\bin\Release\net5.0\publish\*.dll .\publish\