using UnityEngine;

public enum ChemicalType
{
    Vinegar,
    SaltWater,
    BakingSoda,
    ClearSoda
}

public class ChemicalSource : MonoBehaviour
{
    public ChemicalType chemicalType;

    [Header("Optional")]
    public bool oneShot = true;

    [HideInInspector] public bool hasBeenUsed = false;
}