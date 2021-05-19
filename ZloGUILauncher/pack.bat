@echo off

SET dest="C:\Users\John\Desktop"
set archivename="Easy Launcher.zip"
7z.exe a -sfx -tzip -ssw -mx1 %dest%\%archivename% "D:\VS\Easy Launcher\ZloGUILauncher\bin\Release\*.exe"
7z.exe a -tzip -ssw -mx1 %dest%\%archivename% "D:\VS\Easy Launcher\ZloGUILauncher\bin\Release\*.dll
7z.exe a -tzip -ssw -mx1 %dest%\%archivename% "D:\VS\Easy Launcher\ZloGUILauncher\bin\Release\*.cmd
7z.exe a -tzip -ssw -mx1 %dest%\%archivename% "D:\VS\Easy Launcher\ZloGUILauncher\bin\Release\*txt
