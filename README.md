# Chess.NET
*A chess program for Windows written in C# for .NET Framework 4.7+*

### What's working so far (but needs testing):
- play alternately on the same computer
- rotate the board
- move validation
- check validation
- check mate validation
- en passant
- promote pawn
- castling
- network play (a dedicated server / client REST API implementation. Needs testing)
    - added a WebAPI server (ASP.NET)
    - hardcoded to connect to localhost:7002 at the moment (will be changeable in the future. Can be changed in the code of course.)
    - the server supports multiple simultaneous games using a lobby system:
        - the server uses an in-memory database to store each player that is online
        - each client that enters the lobby must enter a unique name
        - ~the lobby is refreshed automatically every 5 seconds~
        - the lobby needs to be refreshed manually now
        - when a player leaves the lobby (= closes the lobby window) he / she will be removed from the lobby and the server
        - the server uses inactivity counters to check for players to be online - players will be notified if the opponent leaves a running game

### TODO:
- draw validation
- use UCI chess engines
