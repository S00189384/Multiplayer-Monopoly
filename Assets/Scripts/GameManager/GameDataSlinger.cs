//Class to hold various data for the game.
//Could move these values into seperate scripts but leaving them here for now.

public class GameDataSlinger 
{
    //Room.
    public const int ROOM_NAME_CHARACTER_LIMIT = 12;
    public const int NUM_MIN_PLAYERS = 2;
    public const int NUM_MAX_PLAYERS = 6;

    //Player name.
    public const int PLAYER_NAME_CHARACTER_LIMIT = 10;

    //Board / Tile.
    public const int NUM_TILES = NUM_CORNER_TILES + (NUM_TILES_PER_ROW * NUM_ROWS);
    public const int NUM_CORNER_TILES = 4;
    public const int NUM_TILES_PER_ROW = 9;
    public const int NUM_ROWS = 4;
    public const int PLAYER_START_TILE_INDEX = 0;

    //Default Money.
    public const int PLAYER_STARTING_MONEY = 2000;
    public const int PASS_GO_MONEY_RECIEVED = 200;

    //Utility.
    public static readonly int[] UTILITY_RENT_DICE_MULTIPLIERS = { 4, 10 };

    //Station.
    //How much should the default station rent be multiplied based on how many stations the owner possesses.
    public const int STATION_OWNED_RENT_MULTIPLIER = 2;

    //Auction.
    //Starting auction bet amount.
    public const int MIN_AUCTION_BET = 1;
    //How much to add onto the minimum bet after a bet has taken place in an auction.
    public const int MIN_AUCTION_BET_ADDITION = 1; 

    //Buildings on property.
    public const int NUM_MAX_HOUSES_PER_PROPERTY = 4;
    //How much selling a house / hotel gives to the player as a percentage of the house original cost.
    public const float PROPERTY_BUILDING_SELL_VALUE_PERCENTAGE_OF_PURCHASE_COST = 0.5f; 

    //Unmortgaging.
    public const float DEFAULT_UNMORTGAGE_INTEREST_COST = 0.1f;
    //What the interest cost is for unmortgaging if a player acquires a property that is mortgaged and chooses to not unmortgage immediately. Not implemented.
    public const float LATE_UNMORTGAGE_INTEREST_COST_FOR_NEW_OWNER = 0.2f;

    //Jail.
    public const int MAX_NUM_GET_OUT_OF_JAIL_FREE_CARDS_PLAYER_INVENTORY = 3;
    public const int GET_OUT_OF_JAIL_FREE_CARD_SELL_VALUE = 50;
    public const int MAX_ROLL_DOUBLE_ATTEMPTS_TO_LEAVE_JAIL = 3;
    public const int LEAVE_JAIL_FEE_COST = 50;
}

//My photon event codes.
public class EventCodes
{
    public const byte NewTurnEventCode = 1;
    public const byte PlayerWonGameEvent = 2;
}

//Names assigned to objects layer mask name / sorting group name.
//Used by tiles and canvas for selection when mortgaging, unmortgaging, selling and constructing buildings.
public class CustomLayerMasks
{
    public const string ownedLayerMaskName = "Owned";
    public const string unownedLayerMaskName = "Unowned";
    
    public const string mortgageableLayerName = "Mortgageable";
    public const string unmortgageableLayerName = "Unmortgageable";

    public const string canSellBuildingLayerName = "CanSellBuilding";
    public const string canConstructBuildingLayerName = "CanConstructBuilding";

    public const string canvasDefaultSortingLayerName = "Canvas";
    public const string canvasMortgageSelectionLayerName = "CanvasDuringMortgageSelection";
    public const string canvasSellBuildingSortingLayerName = "CanvasDuringPropertyBuildingSelling";
    public const string canvasConstructBuildingSortingLayerName = "CanvasDuringPropertyBuilding";
}