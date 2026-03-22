using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReactionQuizUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject quizPanel;
    public TMP_Text questionText;
    public TMP_Text resultText;
    public CabbageBeakerReaction beaker;

    private ChemicalType currentCorrectAnswer;
    private bool quizOpen = false;

    private void Start()
    {
        if (quizPanel != null){
            Debug.Log("Calling quizPanel");
            quizPanel.SetActive(false);
        }

        if (resultText != null){
            Debug.Log("Calling resultText");
            resultText.text = "";
        }
    }

    public void ShowQuestion(ChemicalType correctChemical)
    {
        currentCorrectAnswer = correctChemical;
        quizOpen = true;

        if (quizPanel != null)
            quizPanel.SetActive(true);

        if (questionText != null)
            questionText.text = "Which chemical was just added to the cabbage extract?";

        if (resultText != null)
            resultText.text = "";
    }

    public void SelectAnswer(int selectedChemicalIndex)
    {
        ChemicalType selected = (ChemicalType)selectedChemicalIndex;

        if (selected == currentCorrectAnswer)
        {
            resultText.text = "Correct!";
        }
        else
        {
            resultText.text = "Wrong!";
        }

        // 👉 Reset after 2 seconds
        Invoke("FinishQuiz", 2f);
    }

    void FinishQuiz()
    {
        CloseQuiz();

        if (beaker != null)
            beaker.ResetToNeutral();
    }

    public void CloseQuiz()
    {
        quizOpen = false;

        if (quizPanel != null)
            quizPanel.SetActive(false);
    }
}