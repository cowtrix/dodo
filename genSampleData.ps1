git pull
rm manifest.log -Force
dotnet run --project .\test\GenerateSampleData\GenerateSampleData.csproj > manifest.log
echo "Generated sample data. See manifest.log for GUIDs."