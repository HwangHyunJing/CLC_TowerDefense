﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileContent : MonoBehaviour
{
    GameTileContentFactory originFactory;

    [SerializeField]
    GameTileContentType type = default;

    public GameTileContentType Type => type;

    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(OriginFactory == null, "Refined origin factory!");
            originFactory = value;
        }
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }


}
