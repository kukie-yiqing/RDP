$c = Get-Content -Path $avarPath -Raw;
$c = $c -replace 'BlurRadius = 15', 'BlurRadius = 30';
$c = $c -replace 'ShadowDepth = 2', 'ShadowDepth = 0';
$c = $c -replace 'Margin = new Thickness\0, 0, 8, 0\', 'Margin = new Thickness(0, 0, 8, -2)';
$c = $c.replace('headerStack.Children.Add(historyTitle)', '').replace('addheader', '');
[IO.File]::WriteAllText('d:\remote\RdpLauncher.cs', $c, [Text.Encoding]::UTF8)
