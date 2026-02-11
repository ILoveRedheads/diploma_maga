cd ./src/CSService.API/
dotnet publish -o ../../publish
cd ../CSService.Migrations/
New-Item -Path . -Name "crime-scene.db" -ItemType "file" -Force
dotnet run -c Release
cd ../../
Copy-Item "./src/CSService.Migrations/crime-scene.db" -Destination "./publish"
Copy-Item "./vosk-model-ru-0.42" -Destination "./publish" -Recurse
