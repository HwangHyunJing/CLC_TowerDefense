using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress;

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
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileTo.transform.localPosition;
        progress = 0f;
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
            positionFrom = positionTo;
            positionTo = tileTo.transform.localPosition;
            progress -= 1f;
        }

        transform.localPosition =
            Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        return true;
    }
}
