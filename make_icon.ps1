$text = Get-Content 'd:\remote\RdpLauncher.cs' | Out-String
if ($text -match 'private const string ICON_B64 = "(.*?)";') {
    $base64 = $matches[1]
    $pngBytes = [Convert]::FromBase64String($base64)
    
    $ms = New-Object System.IO.MemoryStream(,$pngBytes)
    [void][System.Reflection.Assembly]::LoadWithPartialName("System.Drawing")
    $bmp = New-Object System.Drawing.Bitmap($ms)
    $w = $bmp.Width
    $h = $bmp.Height
    $ms.Close()
    
    $wByte = if ($w -ge 256) { [byte]0 } else { [byte]$w }
    $hByte = if ($h -ge 256) { [byte]0 } else { [byte]$h }
    
    $fs = New-Object System.IO.FileStream("d:\remote\app.ico", [System.IO.FileMode]::Create)
    $writer = New-Object System.IO.BinaryWriter($fs)
    
    $writer.Write([uint16]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]1)
    
    $writer.Write([byte]$wByte)
    $writer.Write([byte]$hByte)
    $writer.Write([byte]0)
    $writer.Write([byte]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]32)
    $writer.Write([uint32]$pngBytes.Length)
    $writer.Write([uint32]22)
    
    $writer.Write($pngBytes)
    
    $writer.Close()
    $fs.Close()
    Write-Host "Perfect app.ico generated successfully."
} else {
    Write-Host "Match failed."
}
