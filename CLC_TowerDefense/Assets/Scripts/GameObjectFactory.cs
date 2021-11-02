using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameObjectFactory : ScriptableObject
{
    Scene scene;

    // 상속을 위한 protected, template 형 메서드
    protected T CreateGameObjectInstance<T> (T prefab) where T : MonoBehaviour
    {
        // 받은 prefab를 scene으로 옮기는 코드
        if(!scene.isLoaded)
        {
            if(Application.isEditor)
            {
                scene = SceneManager.GetSceneByName(name);
                if(!scene.isLoaded)
                {
                    scene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                scene = SceneManager.CreateScene(name);
            }
        }

        // 
        T instance = Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
        return instance;
    }
}
