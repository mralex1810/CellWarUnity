using UnityEngine;

public static class MakeField
{
    public static FieldProperties MakeCommon()
    {
        var pos = new[]
        {
            new Vector3(1.00f, 1.50f),
            new Vector3(1.00f, 3.50f),
            new Vector3(1.00f, 5.00f),
            new Vector3(1.00f, 6.50f),
            new Vector3(1.00f, 8.50f),
            new Vector3(3.00f, 1.00f),
            new Vector3(3.00f, 3.00f),
            new Vector3(3.00f, 5.00f),
            new Vector3(3.00f, 7.00f),
            new Vector3(3.00f, 9.00f),
            new Vector3(5.00f, 1.50f),
            new Vector3(5.00f, 3.50f),
            new Vector3(5.00f, 5.00f),
            new Vector3(5.00f, 6.50f),
            new Vector3(5.00f, 8.50f),
            new Vector3(7.00f, 1.00f),
            new Vector3(7.00f, 3.00f),
            new Vector3(7.00f, 5.00f),
            new Vector3(7.00f, 7.00f),
            new Vector3(7.00f, 9.00f),
            new Vector3(9.50f, 0.50f),
            new Vector3(9.50f, 2.50f),
            new Vector3(9.50f, 5.00f),
            new Vector3(9.50f, 7.50f),
            new Vector3(9.50f, 9.50f),
            new Vector3(12.00f, 1.00f),
            new Vector3(12.00f, 3.00f),
            new Vector3(12.00f, 5.00f),
            new Vector3(12.00f, 7.00f),
            new Vector3(12.00f, 9.00f),
            new Vector3(14.00f, 1.50f),
            new Vector3(14.00f, 3.50f),
            new Vector3(14.00f, 5.00f),
            new Vector3(14.00f, 6.50f),
            new Vector3(14.00f, 8.50f),
            new Vector3(16.00f, 1.00f),
            new Vector3(16.00f, 3.00f),
            new Vector3(16.00f, 5.00f),
            new Vector3(16.00f, 7.00f),
            new Vector3(16.00f, 9.00f),
            new Vector3(18.00f, 1.50f),
            new Vector3(18.00f, 3.50f),
            new Vector3(18.00f, 5.00f),
            new Vector3(18.00f, 6.50f),
            new Vector3(18.00f, 8.50f)
        };
        int[][] lvl = new[]
        {
            new[] {0, 1, 3, 4, 40, 41, 43, 44},
            new[] {2, 6, 7, 8, 36, 37, 38, 42},
            new[] {5, 9, 11, 12, 13, 31, 32, 33, 35, 39},
            new[] {10, 14, 16, 17, 18, 26, 27, 28, 30, 34},
            new[] {15, 19, 21, 23, 25, 29},
            new[] {20, 22, 24}
        };
        var greenCell = 2;
        var redCell = 42;
        return new FieldProperties(pos, lvl, greenCell, redCell);
    }
}