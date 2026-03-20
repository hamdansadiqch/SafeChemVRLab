using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class EquipOnGrab : MonoBehaviour
{
    [Header("Equipment Settings")]
    public string equipmentName; // e.g., "Safety Glasses" or "Gloves"

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        // Grab the XR component attached to this object
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Subscribe to the grab event
        grabInteractable.selectEntered.AddListener(OnEquip);
    }

    private void OnEquip(SelectEnterEventArgs args)
    {
        Debug.Log($"{equipmentName} successfully equipped!");

        // Tell the UI Manager to show the message on screen!
        if (EquipmentNotification.Instance != null)
        {
            EquipmentNotification.Instance.ShowMessage($"{equipmentName} Equipped!");
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Always clean up listeners to prevent memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnEquip);
        }
    }
}