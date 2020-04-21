dotnet publish -o .\publish -c Release .\DocumentCreatorAPI\DocumentCreatorAPI.csproj
@cd .\DocumentCreatorNgApp
call ng build --configuration=docker --output-path ..\publish\wwwroot
@cd ..

