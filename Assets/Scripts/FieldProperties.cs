using UnityEngine;

public class FieldProperties
{
    public readonly int[][] Lvl;
    public readonly Vector3[] Pos;
    public readonly int[] CellOfOwners;

    public FieldProperties(Vector3[] pos, int[][] lvl, int[] cellOfOwners)
    {
        this.Pos = pos;
        this.Lvl = lvl;
        this.CellOfOwners = cellOfOwners;
    }
}