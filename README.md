docker-compose --env-file .env up --build

dotnet watch run --launch-profile https

dotnet publish -c Release -r win-x64 --self-contained -o ./publish