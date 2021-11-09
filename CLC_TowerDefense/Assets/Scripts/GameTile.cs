using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow = default;

    // 주변 타일에 대한 정보 저장
    GameTile north, east, south, west, nextOnPath;
    // 최종 목적지 타일까지 겨쳐야 할 수
    int distance;

    // Enemy가 경로 이동을 위해 참조하는 부분
    public GameTile NextTileOnPath => nextOnPath;

    // 특정 방향으로 이웃을 생성하기 위한 public 호출
    public GameTile GrowPathNorth() => GrowPathTo(north);
    public GameTile GrowPathSouth() => GrowPathTo(south);
    public GameTile GrowPathWest() => GrowPathTo(west);
    public GameTile GrowPathEast() => GrowPathTo(east);

    // 만약 distance가 값을 초기화했다면, path가 있다고 간주
    public bool HasPath => distance != int.MaxValue;

    // 각 타일의 회전에 대한 성분
    static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    public bool IsAlternative { get; set; }

    // game tile의 내용에 직접 접근하기 위한 변수
    GameTileContent content;

    public GameTileContent Content
    {
        get => content;
        set
        {
            // content값이 존재하는지 확인
            Debug.Assert(value != null, "Null assigned to content!");

            // 이미 content가 존재한다면, 신규 content 오브젝트 파괴
            if(content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

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

        south.north = north;
        north.south = south;
    }

    // 경로에 대한 초기화
    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    // 특정 타일을 destination으로 만들 때 사용
    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
    }

    // 인자로 받은 이웃을 자신의 직전 타일로 설정
    GameTile  GrowPathTo (GameTile neighbor)
    {
        // 만약 HasPath가 false라면, 에러 출력
        Debug.Assert(HasPath, "No path!");

        // 존재하지 않거나 || 경로를 지닐 수 없다면
        if(neighbor == null || neighbor.HasPath)
        {
            // path에 포함시키지 않는다
            return null;
        }

        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;

        // 이웃 타일이 가용 공간이면 neighbor를, 막힌 wall이라면 null을 리턴
        return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
    }

    // 주어진 경로에 따라 회전을 시키는 메서드
    public void ShowPath()
    {
        // 만약 destination이라면 화살표를 제거
        if(distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == south ? southRotation :
            nextOnPath == east ? eastRotation :
            westRotation;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }
}
