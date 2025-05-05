using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using TMPro;

public class RunYOLO8nTests
{
    private GameObject testObj;
    private RunYOLO8n yoloScript;
    private GameObject labelObj;
    private DummyAPIManager dummyAPI;

    [SetUp]
    public void Setup()
    {
        testObj = new GameObject("YOLOTestObject");
        yoloScript = testObj.AddComponent<RunYOLO8n>();

        // Setup TMP label
        labelObj = new GameObject("Label", typeof(TextMeshProUGUI));
        yoloScript.objectLabel = labelObj.GetComponent<TextMeshProUGUI>();

        // Assign dummy API manager
        dummyAPI = testObj.AddComponent<DummyAPIManager>();
        yoloScript.apimanager = dummyAPI;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(testObj);
        Object.DestroyImmediate(labelObj);
    }

    [Test]
    public void UpdateLabel_AppendsCorrectly()
    {
        yoloScript.UpdateLabel("apple");

        Assert.AreEqual("apple", yoloScript.objectLabel.text);
        Assert.Contains("apple", dummyAPI.detectedLabels);
    }

    [UnityTest]
    public IEnumerator CheckConsecutiveLabels_TriggersAfter3Same()
    {
        yoloScript.UpdateLabel("pizza");
        yoloScript.UpdateLabel("pizza");
        yoloScript.UpdateLabel("pizza");

        yield return null;

        yoloScript.CheckConsecutiveLabels();

        Assert.AreEqual(1, dummyAPI.calledCount, "API should be triggered once after 3 identical labels.");
    }

    [Test]
    public void CheckConsecutiveLabels_DoesNotTriggerIfDifferent()
    {
        yoloScript.UpdateLabel("pizza");
        yoloScript.UpdateLabel("apple");
        yoloScript.UpdateLabel("donut");

        yoloScript.CheckConsecutiveLabels();

        Assert.AreEqual(0, dummyAPI.calledCount, "API should not be triggered if labels differ.");
    }

    [Test]
    public void AllowedItems_ContainsExpectedClass()
    {
        var contains = yoloScript.allowedItems.Contains("banana");
        Assert.IsTrue(contains, "Expected 'banana' to be in allowedItems.");
    }
}

// Dummy API manager to track method calls
public class DummyAPIManager : MonoBehaviour
{
    public int calledCount = 0;
    public List<string> detectedLabels = new();

    public void startGASCoroutine()
    {
        calledCount++;
    }

    public void SetDetectedObject(string label)
    {
        detectedLabels.Add(label);
    }
}
