 = Get-Content -Path 'd:\remote\RdpLauncher.cs' -Raw;
# 1. Smooth Avatar Shadow
 =  -replace 'BlurRadius = 15', 'BlurRadius = 30';  =  -replace 'ShadowDepth = 2', 'ShadowDepth = 0';
# 2. Add Subtitle to labels for theme sync
 =  -replace 'this.subTitleTb = new TextBlock', 'this.subTitleTb = new TextBlock';
 =  -replace 'this.mainStack.Children.Add\(this.subTitleTb\);', 'this.labels.Add(this.subTitleTb); this.mainStack.Children.Add(this.subTitleTb);';
# 3. Adjust Header alignment
 =  -replace 'Margin = new Thickness\(0, 0, 8, 0\)', 'Margin = new Thickness(0, 0, 8, -2)';
# 4. Remove History labels
 =  -replace '// History Section Header[\s\S]*historyHeader.Children.Add\(historyTitle\);', '';
# 5. Fix theme sync desaturation
 =  -replace '// Keep subTitleTb a bit more subtle[\s\S]*this.subTitleTb.Foreground = [^;]*;', '';
[IO.File]::WriteAllText('d:\remote\RdpLauncher.cs', , [Text.Encoding]::UTF8)