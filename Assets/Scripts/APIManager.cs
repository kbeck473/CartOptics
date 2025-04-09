using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // ✅ Required for UI Text
using TMPro;
 
public class APIManager : MonoBehaviour
{
    [SerializeField] private string gasURL;
    // [SerializeField] private string prompt;
 
    private string detectedObject;
    private string fullPrompt;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI itemText;
 
    private void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SendDataToGAS());
        }
        */
    }
    public void startGASCoroutine()
    {
        StartCoroutine(SendDataToGAS());
    }
 
    private IEnumerator SendDataToGAS()
    {
        /*
        string detectedObject = RunYOLO8n.latestDetectedLabel; // Stores detected object name
        if (string.IsNullOrEmpty(detectedObject))
        {
            Debug.LogWarning("Detected object is null or empty. Skipping request.");
            yield break; // Stop execution if no object is detected
        }
        */
        string fullPrompt = $"Please provide a concise overview of an orange using the following template:   Fruit: [Fruit Name] Flavor: [Brief description of the taste profile] Nutritional Benefits: [Key nutrients and health benefits] Origin: [Where it's commonly grown or originally from] Common Uses: [Typical culinary or other uses] Allergy Info: [things to watch out for]  Keep the summary minimal and user-friendly. Also do not use bold font at all, meaning no ** around the headers like fruit. ALso please add a blank line of space in between each section such as fruit and flavor etc.";
        WWWForm form = new WWWForm();
        form.AddField("parameter", fullPrompt); // changed to detectedObject
        UnityWebRequest www = UnityWebRequest.Post(gasURL, form);
 
        yield return www.SendWebRequest();
        string response = "";
 
        if(www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
            infoText.text = response;
            StartCoroutine(TypewriterEffect(response)); // ✅ Use typewriter animation
 
        }
        else
        {
            response = "There was an error:";
            infoText.text = response;
            StartCoroutine(TypewriterEffect(response)); // ✅ Use typewriter animation
 
        }
 
        Debug.Log(response);
    }
 
    private IEnumerator TypewriterEffect(string newText)
    {
        infoText.text = ""; // Start with an empty text field
        infoText.maxVisibleCharacters = 0; // Hide all characters initially
 
        for (int i = 0; i <= newText.Length; i++)
        {
            infoText.text = newText; // Set the full text
            infoText.maxVisibleCharacters = i; // Gradually reveal characters
            yield return new WaitForSeconds(0.0001f); // Adjust speed here (0.05s per character)
        }
    }
 
}