#Requires -RunAsAdministrator
New-Service -Name "PNetService" -BinaryPathName ../PNetService.exe
Pause