param (
    [string]$SqlUsername,
    [string]$SqlPassword
)


$SqlInstance = "localhost"
$DatabaseName = "smooth.db"
$StorageAccounts = @("stsmoothstage")
$ResourceGroup = ""
$Containers = @("data", "new-media", "webp")
$AzuriteConnectionString = "UseDevelopmentStorage=true"

.\clean.ps1 `
	-SqlUsername $SqlUsername `
	-SqlPassword $SqlPassword `
	-SqlInstance $SqlInstance `
	-DatabaseName $DatabaseName `
	-StorageAccounts $StorageAccounts `
	-ResourceGroup $ResourceGroup `
	-Containers $Containers `
	-AzuriteConnectionString $AzuriteConnectionString
	
