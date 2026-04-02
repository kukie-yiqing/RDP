@echo off
set CSC_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    echo Error: csc.exe not found at %CSC_PATH%
    echo Please check your .NET Framework installation.
    pause
    exit /b
)

echo Compiling RdpLauncher.cs...
"%CSC_PATH%" /t:winexe /win32icon:app.ico /out:RdpLauncher.exe RdpLauncher.cs /lib:"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF" /r:PresentationCore.dll /r:PresentationFramework.dll /r:WindowsBase.dll /r:System.Xaml.dll /r:System.Runtime.Serialization.dll /r:System.Security.dll /r:System.Windows.Forms.dll /resource:avatar.png,RemoteDesk.avatar.png /resource:app_icon.png,RemoteDesk.app_icon.png

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Compilation successful! Created RdpLauncher.exe
    echo.
) else (
    echo.
    echo Compilation failed.
)
