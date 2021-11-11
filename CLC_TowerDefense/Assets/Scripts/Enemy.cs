using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress;

    // 자체적인 rotation을 위한 값
    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Refined origin factory!");
            originFactory = value;
        }
    }

    public void SpawnOn(GameTile tile)
    {
        // enemy 생성 시, 이동해야 하는 경로를 지정
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        

        progress = 0f;
        PrepareIntro();
    }

    void PrepareIntro()
    {
        // 출발지 > 다음 위치로 가는 Exit 받기
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;

        // 이동하는 방향 받기
        direction = tileFrom.PathDirection;

        // 방향 전환 여부에 대한 값 초기화
        directionChange = DirectionChange.None;
        // 처음에는 From과 To 사이의 각 차이가 없다 (초기 이동 방향에 일치)
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        // Enemy 자체의 rotation도 direction에 맞춰주기
        transform.localRotation = direction.GetRotation();
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime;
        while(progress >= 1f)
        {
            // 다음 타일로 행선지 업데이트
            tileFrom = tileTo;
            tileTo = tileTo.NextTileOnPath;
            // destination에 도달한 경우는 일단 파괴(정확히는 recycle)
            if(tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            progress -= 1f;
            PrepareNextState();
        }

        transform.localPosition =
            Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        if(directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(
                directionAngleFrom, directionAngleTo, progress
            );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    void PrepareNextState()
    {
        positionFrom = positionTo;
        positionTo = tileFrom.ExitPoint;

        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;

        // direction Change에 따른 방향값 제시
        switch(directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
    }
    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
    }
    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180f;
    }
}
