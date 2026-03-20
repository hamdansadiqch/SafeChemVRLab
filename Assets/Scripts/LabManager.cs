using UnityEngine;

public class LabManager : MonoBehaviour
{
    // Singleton pattern so other scripts can easily talk to this one
    public static LabManager Instance;

    [Header("Safety State")]
    public bool isWearingGloves = false;
    public bool isWearingGlasses = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // A quick helper function to check if the user is fully protected
    public bool IsFullyProtected()
    {
        return isWearingGloves && isWearingGlasses;
    }
}