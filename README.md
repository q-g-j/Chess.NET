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

### TODO:
- draw validation
- network play (a dedicated server / client REST API implementation is in progress)<br/>
    **Done:**
    - added a WebAPI server (ASP.NET)
    - added a lobby:
        - needs the server running on localhost:7002 at the moment (will be changeable in the future)
        - the server uses an in-memory database to store each player that is online
        - each client that enters the lobby must enter a unique name
        - ~the lobby is refreshed automatically every 5 seconds~
        - lobby needs to be refreshed manually now
        - when a player leaves the lobby (= closes the lobby window) he / she will be removed from the lobby and the server
        - added the possiblity to invite players - invitations are shown in a separate list (still no actual multiplayer - will come next)
- use UCI chess engines
