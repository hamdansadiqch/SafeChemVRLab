using System.Collections;
using UnityEngine;


public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager Instance; 

    [Header("Experiment States")]
    public bool isGreenExperimentActive = false;
    public bool isBlueExperimentActive = false;
    public int experimentsCompleted = 0; 

    [Header("Teleport Pads")]
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.BaseTeleportationInteractable greenStationPad; 
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.BaseTeleportationInteractable blueStationPad;

    [Header("Assistant Audio")]
    public AudioSource assistantVoice;

    [Header("Green Station Audios")]
    public AudioClip welcomeAudio;
    public AudioClip instructionsAudio;

    [Header("Blue Station Audios")]
    public AudioClip welcomeAudio2;      
    public AudioClip instructionsAudio2; 

    [Header("Success Audios")]
    public AudioClip firstSuccessAudio;
    public AudioClip bothSuccessAudio;
    
    // --- NEW: The Final Goodbye ---
    public AudioClip thankYouAndByeAudio;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void StartGreenExperiment()
    {
        if (isBlueExperimentActive) return; 

        isGreenExperimentActive = true;
        Debug.Log("Green Experiment Started!");

        if (blueStationPad != null) blueStationPad.enabled = false;
        
        StartCoroutine(PlayIntroAudio(welcomeAudio, instructionsAudio));
    }

    public void StartBlueExperiment()
    {
        if (isGreenExperimentActive) return;

        isBlueExperimentActive = true;
        Debug.Log("Blue Experiment Started!");

        if (greenStationPad != null) greenStationPad.enabled = false;

        StartCoroutine(PlayIntroAudio(welcomeAudio2, instructionsAudio2));
    }

    IEnumerator PlayIntroAudio(AudioClip welcomeClip, AudioClip instructionsClip)
    {
        if (assistantVoice == null) yield break;

        if (welcomeClip != null)
        {
            assistantVoice.clip = welcomeClip;
            assistantVoice.Play();
            yield return new WaitForSeconds(welcomeClip.length);
        }

        if (instructionsClip != null)
        {
            assistantVoice.clip = instructionsClip;
            assistantVoice.Play();
        }
    }

    // --- UPDATED: Now handles chaining the final goodbye! ---
    public void ReportExperimentComplete()
    {
        experimentsCompleted++; 

        if (experimentsCompleted == 1)
        {
            if (assistantVoice != null && firstSuccessAudio != null)
            {
                assistantVoice.clip = firstSuccessAudio;
                assistantVoice.Play();
            }
            ResetLab(); 
        }
        else if (experimentsCompleted >= 2)
        {
            // If both are done, trigger the grand finale sequence!
            StartCoroutine(PlayFinalVictorySequence());
        }
    }

    // --- NEW: Plays Both Success, waits, then plays Goodbye ---
    IEnumerator PlayFinalVictorySequence()
    {
        ResetLab(); 

        if (assistantVoice != null)
        {
            if (bothSuccessAudio != null)
            {
                assistantVoice.clip = bothSuccessAudio;
                assistantVoice.Play();
                yield return new WaitForSeconds(bothSuccessAudio.length);
            }

            if (thankYouAndByeAudio != null)
            {
                assistantVoice.clip = thankYouAndByeAudio;
                assistantVoice.Play();
            }
        }
    }

    public void ResetLab()
    {
        isGreenExperimentActive = false;
        isBlueExperimentActive = false;

        if (greenStationPad != null) greenStationPad.enabled = true;
        if (blueStationPad != null) blueStationPad.enabled = true;
    }
}