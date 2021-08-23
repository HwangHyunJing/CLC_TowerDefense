using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow = default;

    GameTile north, east, south, west;

    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        // 첫 인자가 true인지 확인
        Debug.Assert(
            west.east == null && east.west == null, "Refined neighbors!");

        // 자신의 서쪽 타일의 동쪽 성분에, ....??
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(
            south.north == null && north.south == null, "Refined neighbors!");

        //
        south.north = north;
        north.south = south;
    }
}
