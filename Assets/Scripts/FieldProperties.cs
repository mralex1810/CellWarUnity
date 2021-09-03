using UnityEngine;

public class FieldProperties
{
    public readonly int[] CellOfOwners;
    public readonly int[][] Lvl;
    public readonly Vector3[] Pos;

    public FieldProperties(Vector3[] pos, int[][] lvl, int[] cellOfOwners)
    {
        Pos = pos;
        Lvl = lvl;
        CellOfOwners = cellOfOwners;
    }
}