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

    private void Awake()
    {
        // Game Board의 크기를 초기화
        board.Initialize(boardSize);
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
}
