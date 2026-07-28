# PowerShell 範例：顯示系統資訊
Write-Host "=== 系統資訊 ==="
Write-Host "使用者: $env:USERNAME"
Write-Host "電腦名稱: $env:COMPUTERNAME"
Write-Host "目前目錄: $(Get-Location)"
