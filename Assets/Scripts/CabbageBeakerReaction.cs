using UnityEngine;

public class CabbageBeakerReaction : MonoBehaviour
{
    [Header("Liquid Renderer")]
    public Renderer cabbageLiquidRenderer;

    [Tooltip("Use _BaseColor for URP Lit, _Color for Standard shader")]
    public string colorProperty = "_BaseColor";

    [Header("Effects")]
    public ParticleSystem fumesVFX;
    public ParticleSystem bubblesVFX;
    public AudioSource audioSource;
    public AudioClip acidClip;
    public AudioClip baseClip;
    public AudioClip neutralClip;

    [Header("Quiz UI")]
    public ReactionQuizUI quizUI;

    [Header("Settings")]
    public float touchCooldown = 0.8f;
    private float nextTouchTime = 0f;

    // Colors for cabbage indicator
    private readonly Color neutralPurple = new Color(0.60f, 0.20f, 0.70f, 0.90f);
    private readonly Color acidPink = new Color(0.90f, 0.20f, 0.50f, 0.90f);
    private readonly Color acidRed = new Color(0.85f, 0.10f, 0.20f, 0.90f);
    private readonly Color baseBlue = new Color(0.20f, 0.35f, 0.90f, 0.90f);
    private readonly Color baseGreen = new Color(0.20f, 0.75f, 0.35f, 0.90f);

    private void Start()
    {
        SetLiquidColor(neutralPurple);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextTouchTime) return;

        ChemicalSource chem = other.GetComponentInParent<ChemicalSource>();
        if (chem == null) return;

        if (chem.oneShot && chem.hasBeenUsed) return;

        RunReaction(chem);
        chem.hasBeenUsed = true;
        nextTouchTime = Time.time + touchCooldown;
    }

    private void RunReaction(ChemicalSource chem)
    {
        switch (chem.chemicalType)
        {
            case ChemicalType.Vinegar:
                SetLiquidColor(acidPink);
                PlayReactionFX(isAcid: true);
                PlaySound(acidClip);
                break;

            case ChemicalType.ClearSoda:
                SetLiquidColor(acidRed);
                PlayReactionFX(isAcid: true);
                PlaySound(acidClip);
                break;

            case ChemicalType.BakingSoda:
                SetLiquidColor(baseBlue);
                PlayReactionFX(isBase: true);
                PlaySound(baseClip);
                break;

            case ChemicalType.SaltWater:
                SetLiquidColor(neutralPurple);
                PlayReactionFX(isNeutral: true);
                PlaySound(neutralClip);
                break;
        }

        if (quizUI != null)
        {
            quizUI.ShowQuestion(chem.chemicalType);
        }
    }

    private void SetLiquidColor(Color newColor)
    {
        if (cabbageLiquidRenderer == null) return;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        cabbageLiquidRenderer.GetPropertyBlock(block);
        block.SetColor(colorProperty, newColor);
        cabbageLiquidRenderer.SetPropertyBlock(block);
    }

    private void PlayReactionFX(bool isAcid = false, bool isBase = false, bool isNeutral = false)
    {
        if (fumesVFX != null)
            fumesVFX.Play();

        if (bubblesVFX != null && (isAcid || isBase))
            bubblesVFX.Play();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}