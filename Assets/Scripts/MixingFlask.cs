using UnityEngine;

public class MixingFlask : MonoBehaviour
{
    [Header("Visual Feedback")]
    [Tooltip("Drag the 3D object representing the liquid inside the flask here.")]
    public MeshRenderer liquidRenderer; 
    
    [Tooltip("The color the liquid should turn when neutralized.")]
    public Color neutralizedColor = new Color(0.2f, 0.8f, 0.2f, 0.8f); // A transparent light green

    private Material liquidMaterial;
    private bool containsAcid = false;
    private bool containsBase = false;

    void Start()
    {
        // Grab the material instance. 
        // Using .material instead of .sharedMaterial ensures we only change the color 
        // of THIS specific flask's liquid, without altering your original project asset.
        if (liquidRenderer != null)
        {
            liquidMaterial = liquidRenderer.material;
        }
        else
        {
            Debug.LogWarning("MixingFlask: Liquid Renderer is not assigned in the Inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check what is being poured in using Unity Tags
        if (other.CompareTag("Acid"))
        {
            AddChemical("Acid");
        }
        else if (other.CompareTag("Base"))
        {
            AddChemical("Base");
        }
    }

    private void AddChemical(string chemicalAdded)
    {
        // CONDITION 1: Safety Check!
        if (!LabManager.Instance.IsFullyProtected())
        {
            TriggerFailure("CHEMICAL BURN! You handled dangerous chemicals without full safety gear!");
            return;
        }

        // CONDITION 2: The Reaction Logic
        if (chemicalAdded == "Acid")
        {
            if (containsAcid) 
            {
                TriggerFailure("Reaction failed! You added too much acid and ruined the mixture.");
                return;
            }
            containsAcid = true;
        }
        else if (chemicalAdded == "Base")
        {
            if (containsBase)
            {
                TriggerFailure("Reaction failed! You added too much base.");
                return;
            }
            containsBase = true;
        }

        // Check for Neutralization Success
        if (containsAcid && containsBase)
        {
            TriggerSuccess("Success! The solution is neutralized.");
        }
        else
        {
            EquipmentNotification.Instance.ShowMessage($"{chemicalAdded} added safely. Awaiting next chemical...");
        }
    }

    private void TriggerFailure(string reason)
    {
        EquipmentNotification.Instance.ShowMessage($"<color=red>FAIL:</color> {reason}");
        
        // Reset the flask so they can try again
        containsAcid = false;
        containsBase = false;
    }

    private void TriggerSuccess(string message)
    {
        EquipmentNotification.Instance.ShowMessage($"<color=green>SUCCESS:</color> {message}");
        
        // Change the liquid color!
        if (liquidMaterial != null)
        {
            // If using the Universal Render Pipeline (URP), the color property is usually "_BaseColor"
            liquidMaterial.SetColor("_BaseColor", neutralizedColor);
            
            // If using the standard built-in shader, it's "_Color". We set both just to be safe!
            liquidMaterial.SetColor("_Color", neutralizedColor);
        }
    }
}