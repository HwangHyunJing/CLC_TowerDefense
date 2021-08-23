using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    // 생성한 타일들을 관리
    GameTile[] tiles;

    Vector2Int size;

    public void Initialize(Vector2Int size)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        // 여기서 offset은 game board의 중간이고,
        // 6.1: 1.4 기준 size의 값은 11이다
        Vector2 offset = new Vector2((size.x - 1) *.5f, (size.y - 1) * .5f);

        tiles = new GameTile[size.x * size.y];

        for(int y=0, i=0; y < size.y; y++)
        {
            for(int x=0; x < size.x; x++, i++)
            {
                GameTile tile = tiles[i] =  Instantiate(tilePrefab);
                
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(
                    x - offset.x, 0f, y - offset.y
                    );

                if(x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
                }
                if(y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
                }
            }
        }

    }

    
}
