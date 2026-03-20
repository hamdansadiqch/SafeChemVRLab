using UnityEngine;
using TMPro; // Needed for TextMeshPro
using System.Collections;

public class EquipmentNotification : MonoBehaviour
{
    // This makes it easy for any script to find this manager
    public static EquipmentNotification Instance; 

    [Header("UI References")]
    public TextMeshProUGUI notificationText;

    void Awake()
    {
        // Set up the Instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Make sure the text is empty when the game starts
        if (notificationText != null)
        {
            notificationText.text = "";
        }
    }

    // Other scripts will call this function
    public void ShowMessage(string message)
    {
        // Stop any previous timers so messages don't overlap weirdly
        StopAllCoroutines(); 
        StartCoroutine(HideMessageAfterDelay(message, 3f)); // Shows for 3 seconds
    }

    private IEnumerator HideMessageAfterDelay(string message, float delay)
    {
        notificationText.text = message; // Show the text
        yield return new WaitForSeconds(delay); // Wait for the delay
        notificationText.text = ""; // Clear the text
    }
}