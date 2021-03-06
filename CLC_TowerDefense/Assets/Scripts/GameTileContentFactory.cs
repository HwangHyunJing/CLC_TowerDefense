using UnityEngine;
// using UnityEngine.SceneManagement;

// GameObjectFactory의 상속을 받음

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    // Scene contentScene;

    [SerializeField]
    GameTileContent destinationPrefab = default;
    [SerializeField]
    GameTileContent emptyPrefab = default;
    [SerializeField]
    GameTileContent wallPrefab = default;
    [SerializeField]
    GameTileContent spawnPointPrefab = default;

    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
        // Debug.Log("name of the object: " + content.gameObject.name);

        // 중복되는 위치에 동일 오브젝트를 생성할 경우, 신규 오브젝트를 제거
        Destroy(content.gameObject);
    }

    // prefab을 기반으로 생성한 물체를 scene에 넣음
    GameTileContent Get(GameTileContent prefab)
    {
        // 본인이 직접 만들지 않고 부모 호출
        GameTileContent instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        // MoveToFactoryScene(instance.gameObject);
        return instance;
    }

    // 받은 type 인자에 따라 맞는 Get을 호출
    public GameTileContent Get (GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination: return Get(destinationPrefab);
            case GameTileContentType.Empty: return Get(emptyPrefab);
            case GameTileContentType.Wall: return Get(wallPrefab);
            case GameTileContentType.SpawnPoint: return Get(spawnPointPrefab);
        }

        // 존재하지 않는 형이 있다면 메시지 호출 후 리턴
        Debug.Assert(false, "Unsupported type: " + type);
        return null;
    }

    // 부모에서 처리
    // void MoveToFactoryScene (GameObject o)

}
