dotnet publish -o ..\DocumentCreatorProd -c Release .\DocumentCreatorAPI\DocumentCreatorAPI.csproj
@cd .\DocumentCreatorNgApp
call ng build --configuration=production --output-path ..\..\DocumentCreatorProd\wwwroot
@cd ..

