using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {

    private class LoadingMonoBehavour : MonoBehaviour { }
    public enum Scene {
        SampleScene,
        Loading,
        MainMenu
    }

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;
    public static void Load(Scene scene){
        // Set the loader callback action to load the target scene
        onLoaderCallback = () => {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehavour>().StartCoroutine(LoadSceneAsync(scene));
        };

        // Optionally, you can manage singleton destruction here if needed
        if (GameManager.Instance != null)
        {
            GameObject.Destroy(GameManager.Instance.gameObject);
        }

        //Load the loading scene
        SceneManager.LoadScene(Scene.Loading.ToString());

    }

    private static IEnumerator LoadSceneAsync(Scene scene) {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOperation.isDone) {
            yield return null;
        }
    }

    public static float GetLoadingProgress() {
        if(loadingAsyncOperation != null) {
            return loadingAsyncOperation.progress;
        }
        return 1f;
    }

    public static void LoaderCallback() {
        // Triggered after the first Update which lets the sceen refresh
        // Execute the loader callback action which will load the target scene
        if(onLoaderCallback != null) {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
