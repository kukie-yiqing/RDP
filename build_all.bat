@echo off
set CSC_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    echo Error: csc.exe not found at %CSC_PATH%
    pause
    exit /b
)

echo Step 1: Compiling GenIco.cs...
"%CSC_PATH%" /t:exe /out:GenIco.exe GenIco.cs /lib:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF" /r:PresentationCore.dll /r:PresentationFramework.dll /r:WindowsBase.dll /r:System.Xaml.dll

if %ERRORLEVEL% NEQ 0 (
    echo GenIco compilation failed.
    pause
    exit /b
)

echo Step 2: Generating icon files...
GenIco.exe

echo Step 3: Compiling RdpLauncher.cs...
"%CSC_PATH%" /t:winexe /win32icon:app.ico /out:RdpLauncher.exe RdpLauncher.cs /lib:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF" /r:PresentationCore.dll /r:PresentationFramework.dll /r:WindowsBase.dll /r:System.Xaml.dll /r:System.Runtime.Serialization.dll /r:System.Security.dll /r:System.Windows.Forms.dll /r:System.Drawing.dll /resource:avatar.png,RemoteDesk.avatar.png /resource:app_icon.png,RemoteDesk.app_icon.png /resource:app.ico,RemoteDesk.app.ico

if %ERRORLEVEL% EQU 0 (
    echo.
    echo All done! Created RdpLauncher.exe with new icon.
    echo.
) else (
    echo.
    echo RdpLauncher compilation failed.
)
pause
