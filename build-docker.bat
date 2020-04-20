dotnet publish -o .\publish -c Release .\DocumentCreatorAPI\DocumentCreatorAPI.csproj
@cd .\DocumentCreatorNgApp
call ng build --configuration=docker --output-path ..\publish\wwwroot
@cd ..
docker build -f .\Dockerfile-prod -t documentcreator:prod .\publish
FOR /f "tokens=*" %i IN ('docker images -qa -f "dangling=true"') DO docker rmi %i
