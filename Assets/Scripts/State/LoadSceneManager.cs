using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager 
{
    private static Scene _unLoadScene;
    public static async void LoadScene(string sceneName)
    {

        // アンロードするシーンを保存（現在のシーン）
        _unLoadScene = SceneManager.GetActiveScene();

        // シーンの読み込み
        await LoadNewSceneAsync(sceneName);
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Battle"))
        {
            BattleManager.Instance.BattleDataSetup();
        }
        // 読み込みが完了したら、古いシーンを削除
        await UnLoadSceneAsync();
        // データのロード
        //GameManager.Instance.SceneData.LoadSceneData();
        
    }

    private static Task LoadNewSceneAsync(string sceneName)
    {
        var tcs = new TaskCompletionSource<object>();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOperation.completed += operation => { tcs.SetResult(null); };

        return tcs.Task.ContinueWith(task =>
        {
            // シーンを読み込む
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private static async Task UnLoadSceneAsync()
    {
        var tcs = new TaskCompletionSource<object>();

        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_unLoadScene.buildIndex);
        asyncOperation.completed += operation => { tcs.SetResult(null); };

        await tcs.Task;
    }
}
