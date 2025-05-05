using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LoadSceneTests
{
    private bool _sceneLoaded;

    [UnityTest]
    public IEnumerator LoadNewScene_LoadsCorrectScene()
    {
        _sceneLoaded = false;

        // Register scene loaded callback
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Create a GameObject with LoadScene component
        var go = new GameObject("SceneLoader");
        var loadScene = go.AddComponent<LoadScene>();

        // Call the method to load the scene
        loadScene.LoadNewScene();

        // Wait up to 5 seconds for scene load
        float timeout = 5f;
        while (!_sceneLoaded && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        // Cleanup
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Assert.IsTrue(_sceneLoaded, "Scene was not loaded successfully.");
        Assert.AreEqual("camera working", SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "camera working")
        {
            _sceneLoaded = true;
        }
    }
}
