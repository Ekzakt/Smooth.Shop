param (
    [string]$SqlUsername,
    [string]$SqlPassword,
    [string]$SqlInstance,
    [string]$DatabaseName,
    [string[]]$StorageAccounts,
    
    [Parameter(Mandatory = $false)]
    [string]$ResourceGroup,
    
    [string[]]$Containers,
    
    [Parameter(Mandatory = $false)]
    [string]$AzuriteConnectionString
)

Write-Host "SqlUsername = " $SqlUsername
Write-Host "SqlPassword = " $SqlPassword

# Build SQL Connection String
if ($SqlUsername -and $SqlPassword) {
    $SqlConnectionString = "Server=$SqlInstance;Database=$DatabaseName;User Id=$SqlUsername;Password=$SqlPassword;Trusted_Connection=False;TrustServerCertificate=True"
} else {
    $SqlConnectionString = "Server=$SqlInstance;Database=$DatabaseName;Integrated Security=True;"
}

Write-Host "SqlConnectionString = " $SqlConnectionString

# Function to Empty All Tables in SQL Database using ADO.NET
function Empty-SqlTables {
    param (
        [string]$ConnectionString
    )

    Write-Host "Connecting to SQL Server..."

    # Load the .NET SQL Client Assembly
    Add-Type -AssemblyName "System.Data"

    try {
        $SqlConnection = New-Object System.Data.SqlClient.SqlConnection
        $SqlConnection.ConnectionString = $ConnectionString
        $SqlConnection.Open()

        # Generate SQL Query to Delete All Data from Tables
        $query = @"
			DECLARE @Sql NVARCHAR(MAX) = ''
			SELECT @Sql += 'DELETE FROM [' + TABLE_SCHEMA + '].[' + TABLE_NAME + '];' + CHAR(13)
			FROM INFORMATION_SCHEMA.TABLES 
			WHERE TABLE_TYPE = 'BASE TABLE'
			EXEC sp_executesql @Sql
"@

        Write-Host "Emptying all tables in database: $DatabaseName"

        $SqlCommand = $SqlConnection.CreateCommand()
        $SqlCommand.CommandText = $query
        $SqlCommand.ExecuteNonQuery()

        Write-Host "Database cleanup successful."
    } catch {
        Write-Host "SQL Error: $_"
    } finally {
        $SqlConnection.Close()
    }
}

# Function to Empty Azure Storage Containers
function Empty-StorageContainers {
    param (
        [string[]]$StorageAccounts,
        [string]$ResourceGroup,
        [string[]]$Containers
    )

	if (-not $ResourceGroup) {
        Write-Host "ResourceGroup is empty. Exiting function..."
        return
    }
	
    Write-Host "Authenticating with Azure..."
    Connect-AzAccount | Out-Null

    foreach ($StorageAccount in $StorageAccounts) {
        foreach ($Container in $Containers) {
            Write-Host "Clearing container '$Container' in storage account '$StorageAccount'..."

            $Context = (Get-AzStorageAccount -ResourceGroupName $ResourceGroup -Name $StorageAccount).Context
            $Blobs = Get-AzStorageBlob -Container $Container -Context $Context

            foreach ($Blob in $Blobs) {
                Remove-AzStorageBlob -Blob $Blob.Name -Container $Container -Context $Context -Force
                Write-Host "Deleted: $($Blob.Name)"
            }
        }
    }

    Write-Host "Azure Storage cleanup complete."
}

# Function to Empty Azurite (Local Storage Emulator) Containers
function Empty-AzuriteContainers {
    param (
        [string]$ConnectionString,
        [string[]]$Containers
    )

	if (-not $AzuriteConnectionString) {
        Write-Host "AzuriteConnectionString is empty. Exiting function..."
        return
    }

    Write-Host "Clearing Azurite (local emulator) storage..."

    # Create Storage Context for Azurite
    $AzuriteContext = New-AzStorageContext -ConnectionString $ConnectionString

    foreach ($Container in $Containers) {
        Write-Host "Clearing container '$Container' in Azurite..."
        
        # Get all blobs in the container
        $Blobs = Get-AzStorageBlob -Container $Container -Context $AzuriteContext

        foreach ($Blob in $Blobs) {
            Remove-AzStorageBlob -Blob $Blob.Name -Container $Container -Context $AzuriteContext -Force
            Write-Host "Deleted (Azurite): $($Blob.Name)"
        }
    }

    Write-Host "Azurite Storage cleanup complete."
}

# Execute Cleanup Tasks
try {
    Empty-SqlTables -ConnectionString $SqlConnectionString
    Empty-StorageContainers -StorageAccounts $StorageAccounts -ResourceGroup $ResourceGroup -Containers $Containers
    Empty-AzuriteContainers -ConnectionString $AzuriteConnectionString -Containers $Containers
    Write-Host "Database and Storage cleanup completed successfully."
} catch {
    Write-Host "Error: $_"
}
