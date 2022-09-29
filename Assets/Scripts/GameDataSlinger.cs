using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataSlinger 
{
    public const int ROOM_NAME_CHARACTER_LIMIT = 12;
    public const int PLAYER_NAME_CHARACTER_LIMIT = 10;

    public const int NUM_TILES = NUM_CORNER_TILES + (NUM_TILES_PER_ROW * NUM_ROWS);
    public const int NUM_CORNER_TILES = 4;
    public const int NUM_TILES_PER_ROW = 9;
    public const int NUM_ROWS = 4;

    public const int PLAYER_START_TILE_INDEX = 0;

    public const int PLAYER_STARTING_MONEY = 2000;
    public const int PASS_GO_MONEY_RECIEVED = 200;

    //What if more than two utilities on board?
    public static readonly int[] UTILITY_RENT_DICE_MULTIPLIERS = { 4, 10 };

    //How much should the default station rent be multiplied based on how many stations the owner possesses.
    public const int STATION_OWNED_RENT_MULTIPLIER = 2;

    public const int NUM_MIN_PLAYERS = 2;
    public const int NUM_MAX_PLAYERS = 6;

    public const int NUM_MAX_HOUSES_PER_PROPERTY = 4;

    public const int MIN_AUCTION_BET = 1; //Starting auction bet amount.
    public const int MIN_AUCTION_BET_ADDITION = 1; //How much to add onto the minimum bet after a bet has taken place in an auction.

    public const float PROPERTY_BUILDING_SELL_VALUE_PERCENTAGE_OF_PURCHASE_COST = 0.5f; //How much selling a house / hotel gives to the player as a percentage of the house original cost.

    public const float UNMORTGAGE_INTEREST_COST = 0.1f;

    //Jail.
    public const int MAX_NUM_GET_OUT_OF_JAIL_FREE_CARDS_PLAYER_INVENTORY = 3;
    public const int GET_OUT_OF_JAIL_FREE_CARD_SELL_VALUE = 50;
    public const int MAX_ROLL_DOUBLE_ATTEMPTS_TO_LEAVE_JAIL = 3;
    public const int LEAVE_JAIL_FEE_COST = 50;
}

public class EventCodes
{
    public const byte NewTurnEventCode = 1;
    public const byte PlayerWonGameEvent = 2;
}

public class CustomLayerMasks
{
    public const int ownedLayerMaskValue = 6;
    public const int notOwnedLayerMaskValue = 7;

    public const string ownedLayerMaskName = "Owned";
    public const string unownedLayerMaskName = "Unowned";
    
    public const string mortgagedSortingLayerName = "Mortgaged";
    public const string unmortgagedSortingLayerName = "Unmortgaged";

    public const string mortgageableLayerName = "Mortgageable";
    public const string unmortgageableLayerName = "Unmortgageable";

    public const string canvasDefaultSortingLayerName = "Canvas";
    public const string canvasSellBuildingSortingLayerName = "CanvasDuringPropertyBuildingSelling";
    public const string canvasConstructBuildingSortingLayerName = "CanvasDuringPropertyBuilding";

    public const string ownedSortingLayerName = "Owned";
    public const string unownedSortingLayerName = "Unowned";
}