# Smairo.MSFS2020.SimConnector
Microsoft Flight Simulator 2020 variable watcher through SimConnect SDK.

Idea here is to dump different sim variables from the game and save them to a file for later processing. As SimConnect SDK has hard dependencies to netframework and window marshalling, this app / library is supposed to work as a wrapper (SDK -> this app -> file) and allow you to use more modern tehnologies for actual sim var usage logic (eg. creating .net core console application that uploads those sim variables from the file the app produced).

Work in progress...

# Prequisitions
1. Microsoft Flight Simulator 2020 game installed
2. SimConnect .dll file
 
You can download SimConnect .dll file from the game:
- Options -> General -> Developers -> DEVELOPER MODE: ON
- Top bar -> Help -> SDK Installer
- After download is complete, install to default location ("C:\MSFS SDK") OR adjust .csproj xcopy paths to reflect .dll location