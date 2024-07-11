# mute-mic-win

This application listens for a hotkey (`Shift` + `PAUSE` by default) to toggle mute status of the primary input device on Windows on the driver level. Optionally shows mute status on an [awtrix](https://github.com/Blueforcer/awtrix-light) pixel clock.

## Requirements

- .NET 8.0 [Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Usage

- Set Awtrix host config and flag color in `MicMuteWin.dll.config`
- Run application, use hotkey as needed
- Right click tray icon to exit

## Building

- Set Awtrix host config and flag color in `App.config`
- Set preferred hotkey constants in `Program.cs`
- `dotnet publish .\GlobalMute.sln -p:PublishSingleFile=true --self-contained false`
