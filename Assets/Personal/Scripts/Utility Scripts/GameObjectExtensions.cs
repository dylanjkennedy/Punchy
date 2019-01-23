using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameObjectExtensions
{
    public static void DoDestroyOnLoad(this GameObject obj)
    {
        obj.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
    }
}