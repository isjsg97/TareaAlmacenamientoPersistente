using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardDataSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI correctsTXT;
    [SerializeField] private TextMeshProUGUI dateTXT;
    [SerializeField] private TextMeshProUGUI incorrectsTXT;
    [SerializeField] private TextMeshProUGUI pointsTXT;
    [SerializeField] private TextMeshProUGUI positionTXT;
    [SerializeField] private TextMeshProUGUI totalTXT;
    

    public void SetData(int position, LeaderboardData data)
    {
        correctsTXT.text = data.CorrectQuestions.ToString();
        dateTXT.text = data.Date.ToLocalTime().ToString("M/d/yyyy");
        incorrectsTXT.text = (data.TotalQuestions - data.CorrectQuestions).ToString();
        pointsTXT.text = data.Points.ToString();
        positionTXT.text = position.ToString();
        totalTXT.text = data.TotalQuestions.ToString();
    }
}
