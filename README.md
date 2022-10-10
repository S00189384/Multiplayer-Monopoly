# Multiplayer Monopoly

This is my first project using Photon and Unity where I recreated a multiplayer version of monopoly. I worked on this project on and off during the summer holidays (2022) and decided to dedicate the full month of September to try and implement as many features as I could by the end of that month. 

## Features I have implemented so far

- Ability to join a room with a room code of up to 6 players.
- Automatic spawning of tiles on a board using scriptable objects to allow for easy customisation of different board types. 
- Turn based system between players.
- The behaviour of every different type of tile in Monopoly was implemented fully - For example - passing go gives the player money, go to jail sends player to jail etc.
- Purchasable tiles can be bought by a player or be put up for auction to be given to the player with the highest bid.
- Ability to mortgage, unmortgage or construct houses or hotels on property if the player owns all of a property colour type.
- Player possessions are transferred to another player if they are bankrupt by that player.
- All of a players poessessions are put up for auction if the are bankrupt by paying the bank. 
- Player disconnects are supported and do not break the game. The game is over once there is one player left that is not bankrupt or disconnected. 


## Other features that I plan to add when I return to this project:

- Single player option against AI players.
- Joining of random lobbies and "ready up" system for rooms and lobbies. 
- Allowing player to customise their name and monopoly piece.
- 3D models for each monopoly piece.
- Ability to choose which monopoly board to play on.
- Ability to customise various settings of the game - even building rule on or off, money gained for passing go etc. 
- Any disconnected players to be queued up for auctioning their possessions to other players. 

# Gifs showing some of the main features of what I've created so far! 

### Purchasing a tile:
Landing on an unowned purchasable tile will prompt that player with the option to buy or auction the tile. A notification for all players appears when a major event such as a tile purchase is made.

![purchase](https://user-images.githubusercontent.com/47157867/194889384-dac787b1-35ec-48da-a381-f919deaa59bf.gif)

### Auctioning a tile:
Selecting to auction a tile will enable an auction for all active players. Any bankrupt players can only spectate the auction and are not allowed to bid. Players can fold at any time but can only bid when its their turn. 

![auction](https://user-images.githubusercontent.com/47157867/194889423-4e01b7c7-80fc-492e-be82-fa0914f0ca68.gif)

## Community Chest & Chance cards:
The data for Community Chest and Chance cards can be easily made by creating scriptable objects. Any card can then be added to another scriptable object called "Card Collection" which holds a list of card data. An object in the scene called "Card Pile" uses a card collection to act as a card pile you would have in a normal Monopoly board game. Each card pile shuffles the collection of cards it has at the start of the game and a player grabs a card when they land on the corresponding tile. Any card grabbed by the player is executed and added to the bottom of the pile. 

![community chest](https://user-images.githubusercontent.com/47157867/194894242-0335029a-d34f-440e-b1a6-13839b9a49dd.gif)

## Mortgaging tiles:
Any owned tiles can be mortgaged by the player during their turn. Any owned tiles that can be mortgaged or unmortgaged are shown to the player when they click on the mortgage button. The player is shown what their balance change would be when their mouse hovers over the tile. 

![mortgage](https://user-images.githubusercontent.com/47157867/194896935-e3ff62fc-0905-4116-8889-0b1140553976.gif)

## Constructing property buildings
Houses and hotels can be built on a property if they own all the properties of a colour set and none of the properties are mortgaged. Players are also only allowed to build one building at a time as per the "even build" rule. 

![building](https://user-images.githubusercontent.com/47157867/194897896-6c8fd840-f4bd-4a26-916e-81f0441f2f93.gif)

## Auctioning a players possessions
Currently, this auction only gets triggered when a player declares bankrupcy that was caused by a bank payment. All of the tiles and any get out of jail free cards that they own are put up for auction for other players to bid on. In the future I hope to implement a queue system for disconnected or bankrupt players so that their possessions are queued up for auction for other players to acquire when they leave the game. 

![possession auction](https://user-images.githubusercontent.com/47157867/194898257-4fdb7cde-d506-40d2-9ea5-b1c16bfa75d9.gif)



