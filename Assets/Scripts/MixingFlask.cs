using UnityEngine;

public class MixingFlask : MonoBehaviour
{
    [Header("Visual Feedback")]
    public MeshRenderer liquidRenderer; 
    public Color neutralizedColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);

    [Header("Reset Settings")]
    public Transform[] objectsToReset; 

    // --- NEW: Audio Settings ---
    [Header("Assistant Audio")]
    [Tooltip("Drag the Assistant's Audio Source component here")]
    public AudioSource assistantVoice;
    [Tooltip("Drag the 'added a lot of acid' clip here")]
    public AudioClip tooMuchAcidClip;
    [Tooltip("Drag the 'added a lot of alkali' clip here")]
    public AudioClip tooMuchBaseClip;

    private Material liquidMaterial;
    private Color initialLiquidColor; // --- NEW: To remember the starting color ---

    private bool containsAcid = false;
    private bool containsBase = false;

    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;

    void Start()
    {
        if (objectsToReset != null && objectsToReset.Length > 0)
        {
            initialPositions = new Vector3[objectsToReset.Length];
            initialRotations = new Quaternion[objectsToReset.Length];
            
            for (int i = 0; i < objectsToReset.Length; i++)
            {
                if (objectsToReset[i] != null)
                {
                    initialPositions[i] = objectsToReset[i].position;
                    initialRotations[i] = objectsToReset[i].rotation;
                }
            }
        }

        if (liquidRenderer != null)
        {
            liquidMaterial = liquidRenderer.material;
            
            // --- NEW: Save the original color of the liquid when the game starts ---
            if (liquidMaterial.HasProperty("_BaseColor"))
                initialLiquidColor = liquidMaterial.GetColor("_BaseColor");
            else
                initialLiquidColor = liquidMaterial.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        if (!LabManager.Instance.IsFullyProtected())
        {
            // Null here because we don't have a specific audio for safety gear failure yet!
            TriggerFailure("CHEMICAL BURN! You handled dangerous chemicals without full safety gear!", null);
            return;
        }

        if (chemicalAdded == "Acid")
        {
            if (containsAcid) 
            {
                TriggerFailure("Reaction failed! You added too much acid.", tooMuchAcidClip); // Sends the Acid audio
                return;
            }
            containsAcid = true;
        }
        else if (chemicalAdded == "Base")
        {
            if (containsBase)
            {
                TriggerFailure("Reaction failed! You added too much base.", tooMuchBaseClip); // Sends the Alkali audio
                return;
            }
            containsBase = true;
        }

        if (containsAcid && containsBase)
        {
            TriggerSuccess("Success! The solution is neutralized.");
        }
        else
        {
            EquipmentNotification.Instance.ShowMessage($"{chemicalAdded} added safely. Awaiting next chemical...");
        }
    }

    // --- UPDATED: Now takes an AudioClip and plays it when resetting ---
    private void TriggerFailure(string reason, AudioClip failClip)
    {
        EquipmentNotification.Instance.ShowMessage($"<color=red>FAIL:</color> {reason}");
        
        containsAcid = false;
        containsBase = false;

        // Play the failure audio!
        if (assistantVoice != null && failClip != null)
        {
            assistantVoice.clip = failClip;
            assistantVoice.Play();
        }

        ResetExperimentObjects();
    }

    private void ResetExperimentObjects()
    {
        // --- NEW: Revert the liquid back to its original clear/starting color ---
        if (liquidMaterial != null)
        {
            if (liquidMaterial.HasProperty("_BaseColor"))
                liquidMaterial.SetColor("_BaseColor", initialLiquidColor);
            
            liquidMaterial.color = initialLiquidColor;
        }

        if (objectsToReset == null) return;

        for (int i = 0; i < objectsToReset.Length; i++)
        {
            if (objectsToReset[i] != null)
            {
                objectsToReset[i].position = initialPositions[i];
                objectsToReset[i].rotation = initialRotations[i];

                Rigidbody rb = objectsToReset[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    private void TriggerSuccess(string message)
    {
        EquipmentNotification.Instance.ShowMessage($"<color=green>SUCCESS:</color> {message}");
        
        if (liquidMaterial != null)
        {
            liquidMaterial.SetColor("_BaseColor", neutralizedColor);
            liquidMaterial.SetColor("_Color", neutralizedColor);
        }

        // --- NEW: Tell the global manager we finished so it plays the Congrats audio! ---
        if (ExperimentManager.Instance != null)
        {
            ExperimentManager.Instance.ReportExperimentComplete();
        }
    }
}