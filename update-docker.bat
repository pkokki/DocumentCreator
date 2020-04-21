call .\publish-docker.bat
@cd publish
rd /s /q .\dcfs
docker cp . de44d844bae0:/app/
@cd ..
