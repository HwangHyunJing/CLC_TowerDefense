using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    [SerializeField]
    Texture2D gridTexture = default;

    // 생성한 타일들을 관리
    GameTile[] tiles;

    Vector2Int size;

    // 너비 우선 탐색을 위한 큐
    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    GameTileContentFactory contentFactory;

    // 각 타일의 경로를 보이게 할 것인지 여부
    bool showPaths;

    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (GameTile tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    // 각 타일의 그리드를 보이게 할 것인지 여부
    bool showGrid;
    public bool ShowGrid
    {
        get => showGrid;

        set
        {
            showGrid = value;
            Material m = ground.GetComponent<MeshRenderer>().material;
            if(showGrid)
            {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_MainTex", size );
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }

    public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
    {
        this.size = size;
        this.contentFactory = contentFactory;

        ground.localScale = new Vector3(size.x, size.y, 1f);

        // 여기서 offset은 game board의 중간이고,
        // 6.1: 1.4 기준 size의 값은 11이다
        Vector2 offset = new Vector2((size.x - 1) * .5f, (size.y - 1) * .5f);

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

                // 
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }

        // 하나의 타일을 초기화시킴 (Find Paths는 Toggle 메서드 안에서 기능하므로 생략)
        ToggleDestination(tiles[tiles.Length / 2]);
    }

    bool FindPaths()
    {
        // 각 타일의 모든 경로를 초기화
        foreach(GameTile tile in tiles)
        {
            // 해당 타일이 destination 이라면 우선 queue에 넣음
            if(tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        // destination이 없다면 search Fonriter가 비어있을 것이므로
        if(searchFrontier.Count == 0)
        {
            return false;
        }

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

        // 모든 타일을 검사, 만약 고립된 경로의 타일이 존재한다면 false로 스탑
        foreach(GameTile tile in tiles)
        {
            if(!tile.HasPath)
            {
                return false;
            }
        }

        if(showPaths)
        {
            // path 생성이 끝난 뒤, 화살표를 통해 경로 가시화
            foreach (GameTile tile in tiles)
            {
                tile.ShowPath();
            }
        }

        return true;
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

    // destination의 true/false 상태를 바꾸는 메서드
    public void ToggleDestination(GameTile tile)
    {
        // destination 타입인 경우 empty를 부여
        if(tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            // 이후 다시 경로를 설정해준다

            if(!FindPaths())
            {
                tile.Content =
                    contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    // wall의 true/false 상태를 바꾸는 메서드
    public void ToggleWall (GameTile tile)
    {
        if(tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if(!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    
}
