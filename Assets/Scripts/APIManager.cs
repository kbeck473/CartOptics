using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // ✅ Required for UI Text
using TMPro;
 
public class APIManager : MonoBehaviour
{
    [SerializeField] private string gasURL;
    private string detectedObject;
    private string fullPrompt;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI itemText;

    public void SetDetectedObject(string detectedObject)
    {
        itemText.text = detectedObject;
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
        if (string.IsNullOrEmpty(itemText.text))
        {
            yield break;
        }
        string fullPrompt = $@"You are an AI assistant helping to provide concise overviews of physical, inanimate objects detected by a vision model.
        
        Please provide a concise overview of the following object: [" + itemText.text + @"]

        Use the following template:
        Object: [Object Name]

        Description: [Brief description of the object]

        Origin: [Where it’s commonly used or originally from]

        Common Uses: [Typical uses of the object]

        Pricing: [How much it usually costs in USD]

        If the object is a food item or edible, mention relevant details such as taste, nutritional value, or common allergens in the Description section. Otherwise, exclude any food-related information.

        Do not use bold font or any special formatting. Leave a blank line of space between each section.

        If no object is specified after 'following object:', do not provide a response.";
        

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