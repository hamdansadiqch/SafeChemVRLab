using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReactionQuizUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject quizPanel;
    public TMP_Text questionText;
    public TMP_Text resultText;

    private ChemicalType currentCorrectAnswer;
    private bool quizOpen = false;

    private void Start()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (resultText != null)
            resultText.text = "";
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
        if (!quizOpen) return;

        ChemicalType selected = (ChemicalType)selectedChemicalIndex;

        if (selected == currentCorrectAnswer)
        {
            if (resultText != null)
                resultText.text = "Correct!";
        }
        else
        {
            if (resultText != null)
                resultText.text = "Wrong. Correct answer: " + currentCorrectAnswer;
        }
    }

    public void CloseQuiz()
    {
        quizOpen = false;

        if (quizPanel != null)
            quizPanel.SetActive(false);
    }
}