using System.Collections.Generic;
using UnityEngine;

public class AI
{
    public const int Owner = 2;

    private static readonly sbyte[,] CellOption =
    {
        {
            56, 116, 71, -18, -102, 29, -5, -96, 30, -89, 0, 48, -60, -75, 37, 42, 126, 46, 28, -37, -13, 34, -59, 0,
            96,
            99, 2, 54, 52, 85, 86, 31
        },
        {
            -56, 5, -91, -126, 78, 67, 64, -40, 40, 110, -48, -22, 88, -113, -18, 23, -7, -81, -98, -61, 20, -39, -33,
            -113,
            -22, -22, -106, -62, -100, -100, -14, -29
        },
        {
            111, -87, 53, 75, -125, 84, 3, 117, -79, -122, 64, -50, 24, 37, 75, 93, 23, 43, -62, -2, 34, 10, -10, -94,
            38,
            -20, -76, -99, -117, -39, -103, -65
        },
        {
            -29, -8, -60, 61, 19, 116, -44, -75, -45, 14, -29, -79, 60, -23, -22, 68, 71, -78, 81, -16, 33, -69, -80,
            -107,
            -44, 54, -113, -2, 49, -4, 89, 103
        },
        {
            -49, 105, -68, 76, 41, -108, 6, -36, -102, 74, 57, 71, 21, -60, -65, -11, -7, 104, -54, 47, 10, -11, 114,
            -32,
            -18, -19, -5, -39, 22, 111, -68, 99
        },
        {
            39, 71, -96, 30, 126, 5, -75, -122, 73, -29, 93, 33, 95, -59, -120, -17, 0, -30, -6, -24, 40, -47, -44, -84,
            -12, 118, -92, 25, -92, -10, 96, 32
        },
        {
            -57, -10, 63, 66, 49, 91, 68, 104, -5, 69, 11, 45, -73, -2, -44, -106, 28, -97, 20, -91, -101, -86, 80, 42,
            58,
            88, -11, -124, -61, 111, -47, -8
        },
        {
            56, -87, -111, -27, 75, 87, -30, -106, 94, -109, -64, 1, -34, -127, -109, 78, -117, 12, -13, 71, -50, -32,
            -17,
            -41, 55, -54, -39, 1, -96, 43, -127, 63
        },
        {
            -48, -15, 34, -102, 65, 116, -81, 37, 21, -10, -100, -19, -108, -121, 83, 121, -26, -103, 61, 36, -126, -79,
            -8,
            -89, -97, -87, 66, -1, 119, -15, 16, 116
        },
        {
            -25, -40, 90, 14, -82, 44, 49, -95, -62, 118, 51, -56, -49, -102, -69, 97, -9, -8, -14, -63, 21, 17, 103,
            41,
            -37, -101, -21, -24, 35, 55, -29, -104
        }
    };

    private static readonly sbyte[,] TentacleOption =
    {
        {
            -66, 124, 60, -96, 21, 91, 78, -115, 114, -67, 15, -123, 96, 49, -121, -122, -104, 78, 66, 95, 74, 22, 7,
            101,
            -30, -111, -28, 71, -50, -98, -24, -104
        },
        {
            65, -74, -23, 10, 72, -46, -86, 79, 123, -21, -103, -55, 66, 113, 99, -13, -41, -73, 62, 28, 91, -60, 7,
            -75,
            38, -61, -23, 117, -52, -77, -9, -103
        },
        {
            76, -1, 120, -9, -107, 51, -124, 59, -123, -28, -30, -31, 18, 113, 65, 16, 35, 36, -68, 64, -40, -21, 120,
            89,
            -85, -119, -98, 86, 13, 66, -67, -11
        },
        {
            -27, -109, 90, 47, -114, 9, -39, -46, 4, -110, -110, 28, -81, -19, -98, -81, 118, -93, 91, -3, -81, -100,
            98,
            124, 96, 101, -6, 70, -21, 36, -96, -77
        },
        {
            97, -91, 60, 117, 85, -11, -26, -50, -81, -7, 4, 51, 117, 6, 33, -87, -82, 124, -85, -91, -10, -40, 33, -15,
            86,
            -70, 28, 109, 30, -98, 114, 101
        },
        {
            76, 108, -102, -30, 73, -4, -99, -56, -63, 56, -15, -83, 6, 64, -110, 121, 119, 51, -6, -77, 21, -36, -97,
            -106,
            93, -45, 9, -38, 55, -12, 23, 11
        },
        {
            44, -30, -37, 103, -67, -56, 64, -29, 89, 73, -45, -113, -29, 10, -9, -58, -70, -101, 51, -60, -107, -89,
            13,
            114, -125, -14, 54, -104, -110, 79, 2, -57
        },
        {
            85, 71, -73, 50, -64, 67, 63, -95, 39, 65, -38, 52, -13, -70, 123, 105, 101, -22, 116, -29, -98, -119, -38,
            41,
            35, 56, 42, -58, 20, -62, 19, 107
        },
        {
            -94, 109, -84, -52, -77, 6, -72, 94, 56, -74, 10, 51, -63, -46, 4, -76, 56, -36, -120, 14, 14, 58, -74, 59,
            98,
            -12, -114, 104, 96, 20, -94, 57
        },
        {
            -8, 22, 117, -85, 101, -62, 38, -121, -31, 35, -82, 18, -67, -15, 27, -65, 60, 32, 119, -24, -76, -41, -20,
            -46,
            -113, 13, 66, 16, 55, 118, 89, 2
        }
    };

    private readonly sbyte[] _gens =
    {
        -55, 69, -29, -92, 2, -96, -17, 105, 114, 87, -21, -106, -1, 121, 36, -45, 115, -35, -35, 53, -102, 90, -101,
        -103, -79, 103, 97, -53, 18, 10, -105, -85, -28, -18, -31, -64, 33, -62, -33, -113, -17, -37, 13, -49, -39, -63,
        30, 122, 76, -33, -26, 27, -24, 61, -11, -53, -67, 19, 36, -33, 24, -14, 114, 95
    };

    public List<(int beginCell, int endCell)> Process(GameObject[] cells, bool[,] tentacles, int tick)
    {
        var ans = new List<(int, int)>();
        foreach (var beginCell in cells)
        {
            var beginCellController = beginCell.GetComponent<Cell>();
            if (beginCellController.owner != Owner) continue;
            foreach (var endCell in cells)
            {
                var endCellController = endCell.GetComponent<Cell>();
                if (beginCellController.id == endCellController.id) continue;
                byte isEnemy = 0;
                byte isGray = 0;
                byte isFriend = 0;
                if (endCellController.owner == 0)
                    isGray = 1;
                else if (endCellController.owner != beginCellController.owner)
                    isEnemy = 1;
                else
                    isFriend = 1;
                var way = (int) (Vector3.Distance(beginCell.transform.position, endCell.transform.position) * 10);
                if (!tentacles[beginCellController.id, endCellController.id])
                {
                    if (way > beginCellController.score) continue;
                    int[] signsCell =
                    {
                        way / 20, 15000 / way, 100 * isEnemy, 100 * isGray, 100 * isFriend,
                        endCellController.score / 10, beginCellController.score / 10,
                        (endCellController.score - beginCellController.score) / 10,
                        tick / 80, (beginCellController.lvl - beginCellController.tentaclesCount) * 100 / 7
                    };
                    var battle = 0;
                    for (var i = 0; i < signsCell.Length; i++)
                    for (var j = 0; j < _gens.Length / 2; j++)
                        battle += _gens[j] * signsCell[i] * CellOption[i, j];
                    if (battle >= 0) ans.Add((beginCellController.id, endCellController.id));
                }
                else
                {
                    int[] signsTentacle =
                    {
                        way / 20, 15000 / way, 100 * isEnemy, 100 * isGray, 100 * isFriend,
                        endCellController.score / 10, beginCellController.score / 10,
                        (endCellController.score - way / 2) / 8,
                        tick / 80, (beginCellController.lvl - beginCellController.tentaclesCount) * 100 / 7
                    };
                    var battle = 0;
                    for (var i = 0; i < signsTentacle.Length; i++)
                    for (var j = _gens.Length / 2; j < _gens.Length; j++)
                        battle += _gens[j] * signsTentacle[i] * TentacleOption[i, j - _gens.Length / 2];
                    if (battle >= 0)
                    {
                        Debug.Log("HELLO");
                        ans.Add((beginCellController.id, endCellController.id));
                    }
                }
            }
        }

        return ans;
    }
}