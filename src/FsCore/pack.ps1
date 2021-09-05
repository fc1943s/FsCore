dotnet tool restore
dotnet paket restore
dotnet run --project ../FsCore.Tests/FsCore.Tests.fsproj
dotnet build --configuration Release
dotnet paket pack bin/Release --build-config Release
