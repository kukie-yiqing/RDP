 = Get-Content -Path 'd:\remote\RdpLauncher.cs' -Raw;
# 1. Smooth Circular Shadow
 =  -replace 'BlurRadius = 15', 'BlurRadius = 30';  =  -replace 'ShadowDepth = 2', 'ShadowDepth = 0';
# 2. Sync Subtitle Color
 =  -replace 'this.subTitleTb = new TextBlock', 'this.subTitleTb = new TextBlock';
if ( -notmatch 'this.labels.Add\(this.subTitleTb\)') {  =  -replace 'this.mainStack.Children.Add\(this.subTitleTb\);', 'this.labels.Add(this.subTitleTb); this.mainStack.Children.Add(this.subTitleTb);' };
# 3. Baseline Header Alignment
 =  -replace 'Margin = new Thickness\(0, 0, 8, 0\)', 'Margin = new Thickness(0, 0, 8, -2)';
# 4. Remove Redundant Header Labels
 = .replace('historyHeader.Children.Add(historyTitle);', '').replace('this.mainStack.Children.Add(historyHeader);', '').replace('Grid historyHeader = new Grid {', '');
# 5. Fix Accent Drift
 =  -replace '// Keep subTitleTb a bit more subtle[\s\S]*?this.subTitleTb.Foreground = [^;]*?;', '';
# 6. Correct Resource Loading for Image 2
 =  -replace 'Stream stream = assembly.GetManifestResourceStream\(.RemoteDesk.app_icon.png.\);', 'string[] resNames = assembly.GetManifestResourceNames(); string target = System.Linq.Enumerable.FirstOrDefault(resNames, n => n.Contains(" app_icon.png\)); Stream stream = !string.IsNullOrEmpty(target) ? assembly.GetManifestResourceStream(target) : null;';
[IO.File]::WriteAllText('d:\remote\RdpLauncher.cs', , [Text.Encoding]::UTF8)