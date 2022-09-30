using UnityEngine;

//Base class scriptable object for a card the player can receive by landing on a chance or community chest tile.
//Contains a method for executing the card.

public abstract class CardData : ScriptableObject
{
    public string CardDescription;
    public abstract void Execute(string playerID);
}