using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject
{
    Scene contentScene;

    [SerializeField]
    GameTileContent destinationPrefab = default;
    [SerializeField]
    GameTileContent emptyPrefab = default;

    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(content.gameObject);
    }

    GameTileContent Get(GameTileContent prefab)
    {
        GameTileContent instance = Instantiate(prefab);
        instance.OriginFactory = this;
        MoveToFactoryScene(instance.gameObject);
        return instance;
    }

    // 받은 type 인자에 따라 맞는 Get을 호출
    public GameTileContent Get (GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination: return Get(destinationPrefab);
            case GameTileContentType.Empty: return Get(emptyPrefab);
        }

        // 존재하지 않는 형이 있다면 메시지 호출 후 리턴
        Debug.Assert(false, "Unsupported type: " + type);
        return null;
    }

    // 씬으로 옮긴다곤 하는데.. 뭔 소리야
    void MoveToFactoryScene (GameObject o)
    {
        if(!contentScene.isLoaded)
        {
            if(Application.isEditor)
            {
                contentScene = SceneManager.GetSceneByName(name);
                if(!contentScene.isLoaded)
                {
                    contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                contentScene = SceneManager.CreateScene(name);
            }
        }
        SceneManager.MoveGameObjectToScene(o, contentScene);
    }
}
