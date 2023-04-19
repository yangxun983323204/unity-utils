@echo off
echo installing %1
%~dp0\adb install -r -t -d %1
pause