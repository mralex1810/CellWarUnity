using UnityEngine;

public class FieldProperties
{
    public int greenCell;
    public int[][] lvl;
    public Vector3[] pos;
    public int redCell;

    public FieldProperties(Vector3[] pos, int[][] lvl, int greenCell, int redCell)
    {
        this.pos = pos;
        this.lvl = lvl;
        this.greenCell = greenCell;
        this.redCell = redCell;
    }
}