param(
    [Parameter(Mandatory = $false)]
    [string]$Name
)

if (-not $Name) {
    $Name = Read-Host "Enter migration name for the elements context"

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
    }
    else {
        throw "dotnet ef migrations add failed with exit code $LASTEXITCODE."
    }
}
catch {
    Write-Host "Error adding migration: $_" -ForegroundColor Red
}