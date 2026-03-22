using UnityEngine;

public class MixingFlask : MonoBehaviour
{
    [Header("Visual Feedback")]
    public MeshRenderer liquidRenderer; 
    
    [Tooltip("Color when the flask is mostly Acid.")]
    public Color acidColor = new Color(0.8f, 0.1f, 0.1f, 0.8f); // Red
    
    [Tooltip("Color when the flask is mostly Base.")]
    public Color baseColor = new Color(0.1f, 0.1f, 0.8f, 0.8f); // Blue
    
    [Tooltip("The perfect neutralization color (e.g., Pink).")]
    public Color neutralizedColor = new Color(0.8f, 0.4f, 0.8f, 0.8f); // Pink

    [Header("Mixture Settings")]
    [Tooltip("How much liquid is added per trigger/drop.")]
    public float fillAmountPerDrop = 5f; 
    
    [Tooltip("The ideal percentage of acid needed to hit the perfect color (0.5 = 50/50 mix).")]
    [Range(0.1f, 0.9f)]
    public float targetAcidRatio = 0.5f; 
    
    [Tooltip("How forgiving the game is. 0.05 means they can be off by 5% and still win.")]
    public float tolerance = 0.05f;

    private Material liquidMaterial;
    
    private float currentAcid = 0f;
    private float currentBase = 0f;
    private bool experimentEnded = false; // Locks the flask once they win or fail

    void Start()
    {
        if (liquidRenderer != null) liquidMaterial = liquidRenderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (experimentEnded) return; // Don't allow more mixing if they already won/failed

        if (other.CompareTag("Acid"))
        {
            AddChemical("Acid", fillAmountPerDrop);
        }
        else if (other.CompareTag("Base"))
        {
            AddChemical("Base", fillAmountPerDrop);
        }
    }

    private void AddChemical(string chemicalAdded, float amount)
    {
        // 1. Record what the ratio was BEFORE adding the new drop
        float previousTotal = currentAcid + currentBase;
        float previousRatio = previousTotal > 0 ? currentAcid / previousTotal : (chemicalAdded == "Acid" ? 1f : 0f);

        // 2. Add the new chemical
        if (chemicalAdded == "Acid") currentAcid += amount;
        else if (chemicalAdded == "Base") currentBase += amount;

        // 3. Calculate the NEW ratio
        float newTotal = currentAcid + currentBase;
        float newRatio = currentAcid / newTotal;

        // 4. Update the color immediately so the player can see the change
        UpdateVisualColor(newRatio);

        // We only check for success/failure if they actually have BOTH chemicals in the flask
        if (currentAcid > 0 && currentBase > 0)
        {
            CheckReactionState(previousRatio, newRatio);
        }
    }

    private void CheckReactionState(float previousRatio, float newRatio)
    {
        // CONDITION A: Did they hit the perfect color zone?
        if (Mathf.Abs(newRatio - targetAcidRatio) <= tolerance)
        {
            // NEW CHECK: They hit the color, but did they wear safety gear?
            if (!LabManager.Instance.IsFullyProtected())
            {
                TriggerFailure("Experiment failed! You hit perfect neutralization, but you aren't wearing your safety gear!");
                return;
            }

            TriggerSuccess("Perfect! You matched the neutralization color.");
            return;
        }

        // CONDITION B: OVERSHOOT CHECK! 
        // If they were above the target, and jumped completely below it (or vice versa), they added too much!
        bool wasMostlyAcid = previousRatio > (targetAcidRatio + tolerance);
        bool isNowMostlyBase = newRatio < (targetAcidRatio - tolerance);

        bool wasMostlyBase = previousRatio < (targetAcidRatio - tolerance);
        bool isNowMostlyAcid = newRatio > (targetAcidRatio + tolerance);

        if ((wasMostlyAcid && isNowMostlyBase) || (wasMostlyBase && isNowMostlyAcid))
        {
            TriggerFailure("Experiment failed! You added too much and completely ruined the neutralization color.");
        }
    }

    private void UpdateVisualColor(float currentRatio)
    {
        if (liquidMaterial == null) return;
        Color currentColor;

        // A custom 3-way color blend ensuring it hits exactly your neutralizedColor when the ratio is perfect
        if (currentRatio > targetAcidRatio)
        {
            // Mostly Acid - Blends from Pink towards Red
            float t = (currentRatio - targetAcidRatio) / (1f - targetAcidRatio);
            currentColor = Color.Lerp(neutralizedColor, acidColor, t);
        }
        else if (currentRatio < targetAcidRatio)
        {
            // Mostly Base - Blends from Blue towards Pink
            float t = currentRatio / targetAcidRatio;
            currentColor = Color.Lerp(baseColor, neutralizedColor, t);
        }
        else
        {
            // Perfect match
            currentColor = neutralizedColor;
        }

        liquidMaterial.SetColor("_BaseColor", currentColor);
        liquidMaterial.SetColor("_Color", currentColor);
    }

    private void TriggerFailure(string reason)
    {
        experimentEnded = true;
        EquipmentNotification.Instance.ShowMessage($"<color=red>FAIL:</color> {reason}");
        
        // Give them a moment to see their failure, then you could reset the flask here
        Invoke(nameof(ResetFlask), 3f); 
    }

    private void TriggerSuccess(string message)
    {
        experimentEnded = true;
        EquipmentNotification.Instance.ShowMessage($"<color=green>SUCCESS:</color> {message}");
        
        // Ensure it visually snaps to the exact perfect pink
        liquidMaterial.SetColor("_BaseColor", neutralizedColor);
        liquidMaterial.SetColor("_Color", neutralizedColor);
    }

    private void ResetFlask()
    {
        currentAcid = 0f;
        currentBase = 0f;
        experimentEnded = false;
        
        // Reset color to transparent/empty
        Color clearColor = new Color(1, 1, 1, 0);
        liquidMaterial.SetColor("_BaseColor", clearColor);
        liquidMaterial.SetColor("_Color", clearColor);
    }

    // The Beakers will call this if they fall over
    public void ForceFailExperiment(string reason)
    {
        if (experimentEnded) return; // Don't fail if they already won
        
        experimentEnded = true;
        EquipmentNotification.Instance.ShowMessage($"<color=red>CATASTROPHE:</color> {reason}");
        
        // Reset the flask after 3 seconds
        Invoke(nameof(ResetFlask), 3f); 
    }
}