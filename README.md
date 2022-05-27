# Verlet Integration

![Simulation Demo][gif]

This demo shows off a verlet integration solver for real time particle physics. It features collision resolution between particles, a constraint solver to keep everything in a circle, and a chain constraint solver.

This is a monogame implementation of the demo from [this video][video].

## Building
I highly recommend using the dotnet CLI to build this monogame project. You can clone this repo and follow the steps in the [monogame docs][docs]. You can also open the folder in Visual Studio Code and run it from there.

## Notes
There are several demos commented in the code. The default one shows streaming particles and a rope bridge.

The code in this repository is public domain.

[gif]: verlet.gif "Simulation Demo"
[video]: https://youtu.be/lS_qeBy3aQI
[docs]: https://docs.monogame.net/articles/packaging_games.html