$bgUrl = 'https://images.unsplash.com/photo-1503264116251-35a269479413?auto=format&fit=crop&w=800&h=600'
Get-ChildItem -Path 'C:/Users/admin/OneDrive/Desktop/SingleProject/FixNearMe/src/app' -Recurse -Filter '*.component.css' | ForEach-Object {
    $file = $_.FullName
    $content = Get-Content $file -Raw
    $bgRule = ":host {`n    background: url('$bgUrl') no-repeat center center;`n    background-size: cover;`n}`n"
    Set-Content -Path $file -Value ($bgRule + $content)
    Write-Host "Added background to $file"
}
