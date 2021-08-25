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

    // 너비 우선 탐색을 위한 큐
    Queue<GameTile> searchFrontier = new Queue<GameTile>();

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
            for (int x=0; x < size.x; x++, i++)
            {

                // 가장 최근에 생성된 타일
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

                // 일종의 체크보드 패턴을 생성 (직접 그려보면 쉽다)

                // x가 짝수인 경우(비트 and 연산) IsAlternative를 true로 설정
                tile.IsAlternative = (x & 1) == 0;

                // true/false 반전
                if((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }
            }
        }

        // 모든 파일이 경로를 찾도록 하는 메서드
        FindPaths();
    }

    void FindPaths()
    {
        // 각 타일의 모든 경로를 초기화
        foreach(GameTile tile in tiles)
        {
            tile.ClearPath();
        }

        // 특정 타일을 destination으로 삼음
        tiles[tiles.Length / 2].BecomeDestination();
        // destination 정보를 큐에 입력
        searchFrontier.Enqueue(tiles[tiles.Length / 2]);

        // destination을 기반으로 단일 경로 만들기
        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            // dqueue를 통해 잔여 데이터가 떨어지는 경우 종료
            if (tile != null)
            {
                // 다채로운 패턴을 위해 짝수 번째와 홀수 번째 타일의 서치 서순이 다름
                if(tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathWest());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                }
            }
        }

        // path 생성이 끝난 뒤, 화살표를 통해 경로 가시화
        foreach(GameTile tile in tiles)
        {
            tile.ShowPath();
        }
    }

    // 타일의 가리킴에 대한 메서드
    public GameTile GetTile (Ray ray)
    {
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            // return null;
            int x = (int)(hit.point.x + size.x * .5f);
            int y = (int)(hit.point.z + size.y * .5f);
            
            if(x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
        }

        // 아무것도 감지되지 않은 경우
        return null;
    }
}
