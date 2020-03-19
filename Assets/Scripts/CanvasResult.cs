using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class CanvasResult : MonoBehaviour
{ 
    [SerializeField] private Transform leaderboardContent;
    [SerializeField] private GameObject leaderboardDataSlotPrefab;
    [SerializeField] private Button menuBTN;
    [SerializeField] private Button playBTN;
    [SerializeField] private TextMeshProUGUI pointsTXT;
    [SerializeField] private Button quitBTN;
    [SerializeField] private TextMeshProUGUI resultQuestionsTXT;

    [Inject]
    private GameManager _gameManager;

    private void Start()
    {
        menuBTN.onClick.AddListener(_gameManager.LoadMenu);
        playBTN.onClick.AddListener(_gameManager.PlayAgain);
        quitBTN.onClick.AddListener(_gameManager.Quit);
    }

    public void SetData(LeaderboardData currentMatch, LeaderboardData[] leaderboard)
    {
        SetLeaderboard(leaderboard);
        SetResult(currentMatch);
    }

    private void SetLeaderboard(LeaderboardData[] leaderboard)
    {
        for (int i = leaderboardContent.childCount - 1; i >= 0; i--)
        {
            Destroy(leaderboardContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < leaderboard.Length; i++)
        {
            var slot = Instantiate(leaderboardDataSlotPrefab, leaderboardContent);
            slot.GetComponent<LeaderboardDataSlot>().SetData(i + 1, leaderboard[i]);
        }
    }

    public void SetResult(LeaderboardData currentMatch)
    {
        pointsTXT.text = currentMatch.Points.ToString();
        var corrects = currentMatch.CorrectQuestions;
        var total = currentMatch.TotalQuestions;
        var incorrects = total - corrects;
        resultQuestionsTXT.text = $"<color=green>{corrects}</color>/<color=red>{incorrects}</color>/{total}";
    }
}
