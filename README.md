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
    - clients will connect to my public server instance for testing purposes. Server address will be changeable in the future
    - the server supports multiple simultaneous games using a lobby system:
        - the server uses an in-memory database for the lobby and online matches
        - each client that enters the lobby must enter a unique name
        - ~the lobby is refreshed automatically every 5 seconds~
        - the lobby needs to be refreshed manually now
        - the server uses inactivity counters to check for players to be online - players will be notified if the opponent leaves a running game
 
### TODO:
- draw validation
- use UCI chess engines
