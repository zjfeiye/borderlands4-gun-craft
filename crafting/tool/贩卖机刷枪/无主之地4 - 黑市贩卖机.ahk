#Requires AutoHotkey v2.0

; Force the script to run as Administrator
if !A_IsAdmin
{
  try Run('*RunAs "' A_ScriptFullPath '"')
  ExitApp()
}

; Press F8 to move time forward by 1 hour
F8::
{
  ; We use single quotes for the PowerShell string to avoid syntax errors
  RunWait('powershell.exe -Command "Set-Date (Get-Date).AddHours(1)"', , "Hide")
   
  ; Visual feedback near your mouse cursor
  ToolTip("Time +1 Hour")
  SetTimer(() => ToolTip(), -2000) 
}

; Press Ctrl + F8 to close this script entirely
^F8::ExitApp()