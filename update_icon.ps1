$imgPath = 'C:\Users\admin\.gemini\antigravity\brain\1ee27031-63c8-4106-ab36-88ab86460e32\media__1775027919988.png'
$csPath = 'd:\remote\RdpLauncher.cs'
$b64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes($imgPath))
$content = Get-Content $csPath -Encoding UTF8 -Raw
$content = $content -replace 'private const string ICON_B64 = "[^"]*";', "private const string ICON_B64 = `"$b64`";"
[IO.File]::WriteAllText($csPath, $content, [Text.Encoding]::UTF8)
