using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneIndex
{
    Loading = 0,
    Lobby = 1,
    Gameplay = 2,
    LosePopup = 3,
}

public static class SceneLoader
{
    public static async UniTask LoadSceneAsync(SceneIndex sceneIndex, LoadSceneMode mode)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)sceneIndex, mode);
        while (!operation.isDone)
        {
            await UniTask.Yield();
        }
    }

    public static async UniTask UnloadSceneAsync(SceneIndex sceneIndex)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync((int)sceneIndex);
        while (!operation.isDone)
        {
            await UniTask.Yield();
        }
    }
}
