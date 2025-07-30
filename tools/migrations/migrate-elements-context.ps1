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
$connection = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=starlights" # local test db until aspire connected

Write-Host "Adding a new migration: '$Name' to $contextName..." -ForegroundColor Cyan

try {
    # Run the migration command with the output directory specified
    dotnet ef migrations add $Name --project $projectPath --context $contextName --output-dir $outputDir
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration '$Name' added successfully to $contextName in $outputDir folder." -ForegroundColor Green

        # ask here to update the database
        $updateDb = Read-Host "Do you want to update the database with this migration? (y/n)"
        if ($updateDb -eq 'y') {
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