using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public ReactionQuizUI quizUI;
    public ChemicalType optionType;

    public void OnClickOption()
    {
        if (quizUI != null)
        {
            quizUI.SelectAnswer((int)optionType);
        }
    }
}