# MAME Client

This is a client for launching MAME games on windows.

The default MAME client does not show all games at once, and I wanted something the kids could use 
by point-and clicking the games in an alphabetical list.

## Assumptions

- Your mame folder is here: c:\mame
- You have roms in c:\mame\roms

## Installation

You have to build from source. This was authored in the free Visual Studio 2015 community edition:

https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx

Then leave the .exe on your desktop and create a shortcut in the Startup folder to launch the client 
when windows starts. Resize the window the way you like it and it will remember its size and position 
for future loads.

## Extending

Roms do not contain their own descriptions. For nice names in the client's list of games instead 
of the default which is just the rom's cryptic name, you need to produce and scrape XML. Furtunately 
there's an app for that:

https://github.com/MyroStadler/MAME-Tool


