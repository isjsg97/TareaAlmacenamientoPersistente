using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using OpenTDB;
using System;

public class CanvasGameplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsTXT;
    [SerializeField] private TextMeshProUGUI questionNumberTXT;
    [SerializeField] private TextMeshProUGUI questionTXT;
    [SerializeField] private TextMeshProUGUI resultQuestionsTXT;
    [SerializeField] private TextMeshProUGUI timeTXT;
    [SerializeField] private Button[] optionsBTN;

    [SerializeField] private Color optionCorrectColor;
    [SerializeField] private Color optionIncorrectColor;
    [SerializeField] private Color optionMarkedColor;
    [SerializeField] private Color optionNormalColor;

    private void OnEnable()
    {
        Clear();
    }

    public void Clear()
    {
        ClearQuestion();
        ClearTime();
    }

    #region Answers

    public Answer ChoosedAnswer { get; private set; }
    Answer[] _answers;

    public void SetAnswers(Answer[] answers)
    {
        _answers = answers;
        ChoosedAnswer = null;

        for (int i = 0; i < optionsBTN.Length; i++)
        {
            var option = optionsBTN[i];
            if (i >= answers.Length)
                option.gameObject.SetActive(false);
            else
            {
                ActivateOption(option, true);

                var answer = answers[i];

                option.GetComponentInChildren<TextMeshProUGUI>().text = answer.Sentence;
                option.onClick.AddListener(() =>
                {
                    ChoosedAnswer = answer;
                    var colorBlock = option.colors;
                    colorBlock.normalColor = optionMarkedColor;
                    option.colors = colorBlock;
                    ActivateOptions(false);
                });

                var colors = option.colors;
                colors.normalColor = optionNormalColor;
                colors.disabledColor = optionNormalColor;
                colors.pressedColor = optionMarkedColor;
                option.colors = colors;

                option.gameObject.SetActive(true);
            }
        }
    }

    public void ShowSolution(Answer solution)
    {
        for(int i = 0; i < _answers.Length; i++)
        {
            var option = optionsBTN[i];
            var answer = _answers[i];
            var colors = option.colors;

            if (answer == solution)
                colors.normalColor = colors.disabledColor = optionCorrectColor;
            else if (ChoosedAnswer != null && ChoosedAnswer == answer)
                colors.normalColor = colors.disabledColor = optionIncorrectColor;

            option.colors = colors;
            ActivateOption(option, false);
        }
    }

    private void ActivateOption(Button option, bool activated)
    {
        option.interactable = activated;
        var colors = option.colors;
        colors.disabledColor = colors.normalColor;
        option.colors = colors;
    }

    private void ActivateOptions(bool activated)
    {
        Array.ForEach(optionsBTN, o => ActivateOption(o, activated));
    }

    #endregion


    #region Question

    public void ClearQuestion()
    {
        resultQuestionsTXT.text = "";
        questionNumberTXT.text = "";
        questionTXT.text = "";
    }

    public void SetQuestion(Question question, int index)
    {     
        questionNumberTXT.text = $"Question {index}:";
        questionTXT.text = question.Sentence;
    }

    public void SetScore(int index, int points, int questionsCount, int correctCount)
    {
        resultQuestionsTXT.text = $"<color=green>{correctCount}</color>/<color=red>{index - correctCount}</color>/{questionsCount}";
        pointsTXT.text = $"{points}";
    }

    #endregion

    #region Time

    public void ClearTime()
    {
        timeTXT.text = "-";
    }

    public void SetTime(float time)
    {
        timeTXT.text = $"{Mathf.CeilToInt(time)}";
    }

    #endregion
}
