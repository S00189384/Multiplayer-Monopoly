using UnityEngine;

public class TileDisplay_Chance : TileDisplay
{
    [SerializeField] private SpriteRenderer sprend_QuestionMark;

    public override void UpdateDisplay(TileData tileData)
    {
        TileData_Chance chanceData = (TileData_Chance)tileData;

        sprend_QuestionMark.color = chanceData.questionMarkColour;
    }
}
