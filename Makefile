build:
	dotnet build
clean:
	dotnet clean
restore:
	dotnet restore
run:
	dotnet run --project ArchaicQuestII.API/ArchaicQuestII.API.csproj
publish:
	dotnet publish ArchaicQuestII.API/ArchaicQuestII.API.csproj -c Release -o deploy