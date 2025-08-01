param(
    [Parameter(Mandatory = $false)]
    [string]$Name
)

if (-not $Name) {
    $Name = Read-Host "Enter migration name"
    
    if ([string]::IsNullOrWhiteSpace($Name)) {
        Write-Host "Migration name cannot be empty. Exiting." -ForegroundColor Red
        exit
    }
}

$projectPath = "src\modules\elements\Modules.Elements.Data.EntityFramework"
$contextName = "ElementsContext"
$outputDir = "Migrations"

Write-Host "Adding a new migration: '$Name' to $contextName..." -ForegroundColor Cyan

try {
    # run the migration command with the output directory specified
    dotnet ef migrations add $Name --project $projectPath --context $contextName --output-dir $outputDir
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration '$Name' added successfully to $contextName in $outputDir folder." -ForegroundColor Green

        # prompt if the user wants to update the database
        $updateDb = Read-Host "Do you want to update the database with this migration? (y/n)"
        if ($updateDb -eq 'y') {

            # prompt for connection string
            $connection = Read-Host "Enter connection string"                
            if ([string]::IsNullOrWhiteSpace($connection)) {
                Write-Host "connection cannot be empty. Exiting." -ForegroundColor Red
                exit
            }

            # run the database update command
            dotnet ef database update --project $projectPath --context $contextName --connection $connection
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Database updated successfully." -ForegroundColor Green
            }
            else {
                Write-Host "Failed to update the database." -ForegroundColor Red
            }
        }
    }
    else {
        throw "dotnet ef migrations add failed with exit code $LASTEXITCODE."
    }
}
catch {
    Write-Host "Error adding migration: $_" -ForegroundColor Red
}