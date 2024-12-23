using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingState : IGameState
{
    private readonly SceneIndex _nextSceneIndex;
    private readonly Slider _loadingSlider;

    public LoadingState(SceneIndex nextSceneName, Slider loadingSlider)
    {
        _nextSceneIndex = nextSceneName;
        _loadingSlider = loadingSlider;
    }

    public async UniTask EnterState()
    {
        await UniTask.Delay(1000);
        await LoadSceneAsync(_nextSceneIndex, LoadSceneMode.Single);
    }

    public UniTask ExitState()
    {
        return UniTask.CompletedTask;
    }

    private async UniTask LoadSceneAsync(SceneIndex sceneIndex, LoadSceneMode mode)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync((int)sceneIndex, mode);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            _loadingSlider.value = progressValue;

            if (operation.progress >= 0.5f)
            {
                await UniTask.Delay(1000);
                operation.allowSceneActivation = true;
            }
        }
    }
}
