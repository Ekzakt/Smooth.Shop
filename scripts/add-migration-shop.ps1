param (
    [Parameter(Mandatory = $true)]
    [string]$MigrationName
)

$ProjectPath = "../src/Smooth.Shop.Infrastructure/Smooth.Shop.Infrastructure.csproj"
$StartupProjectPath = "../src/Smooth.Shop/Smooth.Shop.csproj"
$DbContext = "SmoothWebDbContext"
$MigrationsPath = "Data/Migrations"

Write-Host "--- Adding migration '$MigrationName' to '$MigrationsPath'..."

# Run the EF Core migration command
dotnet ef migrations add $MigrationName `
  --project $ProjectPath `
  --startup-project $StartupProjectPath `
  --context $DbContext `
  --output-dir $MigrationsPath

if ($?) {
    Write-Host "--- Migration '$MigrationName' created successfully!"
} else {
    Write-Host "--- Migration failed. Please check the error messages."
}