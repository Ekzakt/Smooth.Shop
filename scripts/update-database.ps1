$DbContext = "SmoothWebDbContext"

dotnet ef database update `
  --project ../src/Smooth.Shop.Infrastructure/Smooth.Shop.Infrastructure.csproj `
  --startup-project ../src/Smooth.Shop/Smooth.Shop.csproj `
  --context $DbContext