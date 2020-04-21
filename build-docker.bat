call publish-docker.bat
docker build -f .\Dockerfile-prod -t documentcreator:prod .\publish
FOR /f "tokens=*" %i IN ('docker images -qa -f "dangling=true"') DO docker rmi %i
