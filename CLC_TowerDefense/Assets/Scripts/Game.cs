using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // 게임 보드의 전체 크기
    [SerializeField]
    Vector2Int boardSize = new Vector2Int(11, 11);

    // 게임 보드의 정보를 받아옴
    [SerializeField]
    GameBoard board = default;

    // tile의 타입을 정하는 팩토리
    [SerializeField]
    GameTileContentFactory tileContentFactory = default;

    // 플레이어의 입력값을 위한 레이
    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    private void Awake()
    {
        // Game Board의 크기를 초기화
        board.Initialize(boardSize, tileContentFactory);
    }

    private void Update()
    {
        // 주요한 동작(wall 생성)
        if(Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        // 보조 동작(destination 생성)
        else if(Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }
    }

    private void OnValidate()
    {
        if(boardSize.x < 2)
        {
            boardSize.x = 2;
        }
        if(boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    // 빈 타일을 입력하면 wall로 바꾼다
    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if(tile != null)
        {
            board.ToggleWall(tile);
        }
    }

    
    void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if(tile != null)
        {
            board.ToggleDestination(tile);
        }
    }


}
