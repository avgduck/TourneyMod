using UnityEngine;

namespace TourneyMod.UI;

internal struct StageLayout
{
    private static readonly float STAGE_SCALE_FACTOR = 0.62f;
    private static readonly float STAGE_SCALE_FACTOR_MINI = 0.48f;
    private static readonly float STAGE_SCALE_FACTOR_TINY = 0.41f;
    private static readonly Vector2 STAGE_SIZE = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR;
    private static readonly Vector2 STAGE_SIZE_MINI = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR_MINI;
    private static readonly Vector2 STAGE_SIZE_TINY = new Vector2(500f, 250f) * STAGE_SCALE_FACTOR_TINY;
    private static readonly Vector2 STAGES_POSITION = new Vector2(0f, 0f);
    private static readonly Vector2 STAGES_SPACING = new Vector2(6f, 6f);
    private const float STAGE_CATEGORY_SPACING = 20f;
    private const float STAGE_CATEGORY_SPACING_MINI = 10f;

    internal readonly Vector2 position;
    internal readonly Vector2 spacing;
    
    internal readonly Vector2 stageSize;
    internal readonly float stageScaleFactor;
    internal readonly float stageCategorySpacing;
    internal readonly float totalHeight;
    internal readonly bool useBothCategories;
    
    internal readonly int[] rowLengthsNeutral;
    internal readonly int[] rowLengthsCounterpick;
    internal readonly int maxRowLength;
    internal readonly int numRowsTotal;

    internal StageLayout(int[] rowLengthsNeutral, int[] rowLengthsCounterpick, int maxRowLength, int numRowsTotal)
    {
        this.rowLengthsNeutral = rowLengthsNeutral;
        this.rowLengthsCounterpick = rowLengthsCounterpick;
        this.maxRowLength = maxRowLength;
        this.numRowsTotal = numRowsTotal;
        
        useBothCategories = rowLengthsNeutral.Length > 0 && rowLengthsCounterpick.Length > 0;

        position = STAGES_POSITION;
        spacing = STAGES_SPACING;

        stageSize = STAGE_SIZE;
        if (maxRowLength > 4 || numRowsTotal > 3) stageSize = STAGE_SIZE_MINI;
        if (maxRowLength > 5) stageSize = STAGE_SIZE_TINY;
        
        stageScaleFactor = STAGE_SCALE_FACTOR;
        if (maxRowLength > 4 || numRowsTotal > 3) stageScaleFactor = STAGE_SCALE_FACTOR_MINI;
        if (maxRowLength > 5) stageScaleFactor = STAGE_SCALE_FACTOR_TINY;

        stageCategorySpacing = STAGE_CATEGORY_SPACING;
        if (maxRowLength > 4 || numRowsTotal > 3) stageCategorySpacing = STAGE_CATEGORY_SPACING_MINI;
        
        totalHeight = numRowsTotal * stageSize.y + (numRowsTotal - 1) * spacing.y;
        if (useBothCategories) totalHeight += stageCategorySpacing;
    }

    internal float GetRowWidth(int rowLength)
    {
        return rowLength * stageSize.x + (rowLength - 1) * STAGES_SPACING.x;
    }
    
    internal static StageLayout Create(int numStagesNeutral, int numStagesCounterpick)
    {
        int[] rowLengthsNeutral;
        int[] rowLengthsCounterpick;

        switch (numStagesCounterpick)
        {
            case 0:
                rowLengthsCounterpick = [];
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [3, 3, 3],
                    10 => [3, 4, 3],
                    11 => [4, 4, 3],
                    12 => [4, 4, 4],
                    13 => [4, 5, 4],
                    14 => [5, 5, 4],
                    15 => [5, 5, 5],
                    16 => [4, 4, 4, 4],
                    17 => [5, 4, 4, 4],
                    _ => []
                };
                break;
            
            case >= 1 and <= 4:
                rowLengthsCounterpick = [numStagesCounterpick];
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    11 => [4, 4, 3],
                    12 => [4, 4, 4],
                    13 => [4, 5, 4],
                    14 => [5, 5, 4],
                    15 => [5, 5, 5],
                    16 => [5, 6, 5],
                    _ => []
                };
                break;
            
            case 5:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 10 => [3, 2],
                    >= 9 => [5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    11 => [4, 4, 3],
                    12 => [4, 4, 4],
                    _ => []
                };
                break;
            
            case 6:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 10 => [3, 3],
                    11 => [6],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    >= 5 and <= 6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    11 => [4, 4, 3],
                    _ => []
                };
                break;
            
            case >= 7 and <= 8:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 10 => [4, numStagesCounterpick - 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 5 => [numStagesNeutral],
                    6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    9 => [5, 4],
                    10 => [5, 5],
                    _ => []
                };
                break;
            
            case 9:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 5 => [3, 3, 3],
                    >= 6 and <= 8 => [5, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 5 => [numStagesNeutral],
                    6 => [3, numStagesNeutral - 3],
                    >= 7 and <= 8 => [4, numStagesNeutral - 4],
                    _ => []
                };
                break;
            
            case 10:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 5 => [3, 4, 3],
                    >= 6 and <= 7 => [5, 5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 5 => [numStagesNeutral],
                    6 => [3, 3],
                    7 => [4, 3],
                    _ => []
                };
                break;
            
            case 11:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 6 => [4, 4, 3],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 6 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 12:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 5 => [4, 4, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 5 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 13:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 4 => [4, 5, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 4 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 14:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 3 => [5, 5, 4],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 3 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 15:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    >= 0 and <= 2 => [5, 5, 5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    >= 1 and <= 2 => [numStagesNeutral],
                    _ => []
                };
                break;
            
            case 16:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    0 => [4, 4, 4, 4],
                    1 => [5, 6, 5],
                    _ => []
                };
                rowLengthsNeutral = numStagesNeutral switch
                {
                    1 => [1],
                    _ => []
                };
                break;
            
            case 17:
                rowLengthsCounterpick = numStagesNeutral switch
                {
                    0 => [5, 4, 4, 4],
                    _ => []
                };
                rowLengthsNeutral = [];
                break;
            
            default:
                rowLengthsCounterpick = [];
                rowLengthsNeutral = [];
                break;
        }

        int maxRowLength = 0;
        foreach (int l in rowLengthsNeutral)
        {
            if (l > maxRowLength) maxRowLength = l;
        }
        foreach (int l in rowLengthsCounterpick)
        {
            if (l > maxRowLength) maxRowLength = l;
        }
        
        int numRowsTotal = rowLengthsNeutral.Length + rowLengthsCounterpick.Length;

        return new StageLayout(rowLengthsNeutral, rowLengthsCounterpick, maxRowLength, numRowsTotal);
    }
}