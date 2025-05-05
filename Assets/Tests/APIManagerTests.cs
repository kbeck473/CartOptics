using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class APIManagerTests
{
    private GameObject testGO;
    private APIManager apiManager;
    private TextMeshProUGUI infoTextMock;
    private TextMeshProUGUI itemTextMock;

    [SetUp]
    public void Setup()
    {
        testGO = new GameObject("APITestObject");
        apiManager = testGO.AddComponent<APIManager>();

        // Create and assign mock TMP objects
        var infoGO = new GameObject("InfoText", typeof(TextMeshProUGUI));
        var itemGO = new GameObject("ItemText", typeof(TextMeshProUGUI));
        infoTextMock = infoGO.GetComponent<TextMeshProUGUI>();
        itemTextMock = itemGO.GetComponent<TextMeshProUGUI>();

        apiManager.infoText = infoTextMock;
        apiManager.itemText = itemTextMock;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(testGO);
        Object.DestroyImmediate(infoTextMock.gameObject);
        Object.DestroyImmediate(itemTextMock.gameObject);
    }

    [Test]
    public void SetDetectedObject_UpdatesItemText()
    {
        apiManager.SetDetectedObject("banana");
        Assert.AreEqual("banana", apiManager.itemText.text);
    }

    [UnityTest]
    public IEnumerator StartGASCoroutine_EmptyItemText_DoesNotSendRequest()
    {
        itemTextMock.text = "";

        yield return apiManager.StartCoroutine("SendDataToGAS");

        Assert.IsTrue(string.IsNullOrEmpty(infoTextMock.text));
    }

    [UnityTest]
    public IEnumerator TypewriterEffect_DisplaysFullText()
    {
        string sampleResponse = "Test object description";
        yield return apiManager.StartCoroutine("TypewriterEffect", sampleResponse);

        Assert.AreEqual(sampleResponse, infoTextMock.text);
    }
}
