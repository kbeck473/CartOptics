using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMesh Pro namespace

public class ShoppingCart : MonoBehaviour
{
    // Reference to the TextMesh Pro UI element that holds the new item’s name.
    public TMP_Text itemLabel;

    // Reference to the TextMesh Pro UI element that displays the shopping cart items.
    public TMP_Text cartText;

    // Dictionary to keep track of items and their counts.
    private Dictionary<string, int> cartItems = new Dictionary<string, int>();

    // Called on start to initialize the cart display.
    void Start()
    {
        UpdateCartDisplay();
    }

    // Called when the "Add Item" button is pressed.
    public void OnAddItemButtonPressed()
    {
        string newItem = itemLabel.text.Trim();

        // Only proceed if the item label is not empty.
        if (string.IsNullOrEmpty(newItem))
            return;

        // If the item is already in the cart, increment the count.
        if (cartItems.ContainsKey(newItem))
        {
            cartItems[newItem]++;
        }
        else
        {
            // If it's a new item, add it with a count of 1.
            cartItems[newItem] = 1;
        }

        UpdateCartDisplay();
    }

    // Called when the "Remove/Decrement Item" button is pressed.
    public void OnRemoveItemButtonPressed()
    {
        string itemToRemove = itemLabel.text.Trim();

        // Only proceed if the item label is not empty.
        if (string.IsNullOrEmpty(itemToRemove))
            return;

        // If the item is in the cart, decrement its count.
        if (cartItems.ContainsKey(itemToRemove))
        {
            cartItems[itemToRemove]--;

            // If the count drops to zero, remove the item entirely.
            if (cartItems[itemToRemove] <= 0)
            {
                cartItems.Remove(itemToRemove);
            }
        }
        else
        {
            Debug.Log("Item not found in cart.");
        }

        UpdateCartDisplay();
    }

    // Update the cart text field to show all items and their quantities,
    // or display "No Items In Cart" if the cart is empty.
    private void UpdateCartDisplay()
    {
        List<string> itemsDisplay = new List<string>();

        foreach (var kvp in cartItems)
        {
            // Append count if more than one exists for the item.
            if (kvp.Value > 1)
            {
                itemsDisplay.Add($"{kvp.Key} {kvp.Value}x");
            }
            else
            {
                itemsDisplay.Add(kvp.Key);
            }
        }

        // If the itemsDisplay list is empty, set the default message.
        if (itemsDisplay.Count == 0)
        {
            cartText.text = "No Items In Cart";
        }
        else
        {
            // Otherwise, join the items with new lines.
            cartText.text = string.Join("\n", itemsDisplay);
        }
    }
}
