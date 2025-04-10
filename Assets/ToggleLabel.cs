using UnityEngine;
using TMPro; // Import TextMesh Pro namespace

public class ToggleLabel : MonoBehaviour
{
    // Serialized field for the TextMesh Pro UI element that displays the item name.
    // This is private, but since it’s marked with [SerializeField], it will be visible in the Inspector.
    [SerializeField]
    private TMP_Text itemLabel;

    // This boolean helps us track which item is currently displayed.
    private bool isApple = true;

    // Optionally set a default value when the scene starts.
    void Start()
    {
        if (itemLabel != null)
        {
            itemLabel.text = "Apple";
            isApple = true;
        }
        else
        {
            Debug.LogWarning("Item Label has not been assigned in the Inspector!");
        }
    }

    // Call this method (for example, via a button) to toggle the label.
    public void ToggleItemLabel()
    {
        if (itemLabel == null)
        {
            Debug.LogWarning("Item Label is not assigned!");
            return;
        }

        // Toggle between "Apple" and "Orange" based on the isApple flag.
        if (isApple)
        {
            itemLabel.text = "Orange";
        }
        else
        {
            itemLabel.text = "Apple";
        }

        // Toggle the state for the next button press.
        isApple = !isApple;
    }
}
