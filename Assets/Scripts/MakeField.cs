using System.Collections.Generic;
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
        var cellOfOwners = new[] {2, 42};
        return new FieldProperties(pos, lvl, cellOfOwners);
    }

    public static FieldProperties MakeFieldForFourPlayers(int randomSeed)
    {
        Random.InitState(randomSeed);
        int[] maxCellsWithLvl = new int[] {0, 2, 6, 6, 5, 2, 1};
        Vector3[] pos;
        var cellOfOwners = new int[4];
        var lvl = new int[6][]; 
        List<Vector3> posList = new List<Vector3>();
        while (true)
        {
            int cells = 0;
            lvl = new int[6][];
            int[] cellsWithLvl = new int[7];
            for (int i = 1; i < maxCellsWithLvl.Length; i++)
            {
                cellsWithLvl[i] = Random.Range(1, maxCellsWithLvl[i]);
                var lvlList = new List<int>();
                for (int j = cells; j < cells + cellsWithLvl[i]; j++)
                {
                    lvlList.Add(j);
                }
                cells += cellsWithLvl[i];
                lvl[i - 1] = lvlList.ToArray();
            }
            if (cells < 10) continue;
            posList.Clear();
            
            var fails = 0;
            for (int i = 0; i < cells; i++)
            {
                var newPos = new Vector3(Random.Range(1f, 20f), Random.Range(1f, 10f));
                if (IsCellDistant(newPos, posList))
                {
                    posList.Add(newPos);
                    fails = 0;
                }
                else
                {
                    fails++;
                    i--;
                    if (fails > 100) break;
                }
            }

            if (fails > 100) continue;
            cellOfOwners[0] = lvl[1][0];
            cellOfOwners[1] = cellOfOwners[0] + cells;
            cellOfOwners[2] = cellOfOwners[1] + cells;
            cellOfOwners[3] = cellOfOwners[2] + cells;

            for (int i = 0; i < cells; i++)
            {
                posList.Add(new Vector3(-posList[i].x, posList[i].y));
            }
            for (int i = 0; i < cells; i++)
            {
                posList.Add(new Vector3(-posList[i].x, -posList[i].y));
            }
            for (int i = 0; i < cells; i++)
            {
                posList.Add(new Vector3(posList[i].x, -posList[i].y));
            }

            for (int i = 0; i < 6; i++)
            {
                var newLvlList = new List<int>();
                for (int j = 0; j < 4; j++)
                {
                    foreach (var id in lvl[i])
                    {
                        newLvlList.Add(id + j * cells);
                    }
                }

                lvl[i] = newLvlList.ToArray();
            }

            pos = posList.ToArray();
            break;
        }

        return new FieldProperties(pos, lvl, cellOfOwners);

    }

    private static bool IsCellDistant(Vector3 cellPos, List<Vector3> posList)
    {
        bool ans = true;
        foreach (var pos in posList)
        {
            if (Vector3.Distance(cellPos, pos) < 1)
            {
                ans = false;
                break;
            }
        }

        return ans;
    }
}