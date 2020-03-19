using OpenTDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Timer))]
public class GameManager : MonoBehaviour
{
    private const string FILE_LEADERBOARD = "Leaderboard.dat";
    private const int COUNT_QUESTION_LEADERBOARD = 5;

    [SerializeField] private int pointsQuestion = 100;
    [SerializeField] private int questionCount = 10;
    [SerializeField] private float roundDuration = 10;
    [SerializeField] private float roundResultDuration = 3;

    private CanvasGameplay _canvasGameplay;
    private CanvasResult _canvasResult;
    private Category _category;
    private int _correctQuestions;
    private Difficulty _difficulty;
    private int _points;
    private Question[] _questions;
    private LeaderboardData _result;
    private List<LeaderboardData>_resultAll;
    private Timer _timer;

    [Inject]
    private CanvasManager _canvasManager;
    [Inject]
    private TrivialAPI _triviaAPI;

    public Category[] Categories { get; private set; }
    public Difficulty[] Difficulties { get; private set; }

    private void Awake()
    {
        _canvasGameplay = _canvasManager.CanvasGameplay;
        _canvasResult = _canvasManager.CanvasResult;
        _timer = GetComponent<Timer>();
    }

    void Start()
    {
        LoadMenu();
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadMenuCor());
    }

    public void Play(Category category, Difficulty difficulty)
    {
        _category = category;
        _difficulty = difficulty;
        _canvasManager.ShowLoadingScreen(false);
        StartCoroutine(GameplayCor(category, difficulty));
    }

    public void PlayAgain()
    {
        Play(_category, _difficulty);
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator GameplayCor(Category category, Difficulty difficulty)
    {
        yield return StartCoroutine(InitializeCor(category ,difficulty));
        
        for (int i = 0; i < _questions.Length; i++)
        {
            yield return StartCoroutine(RoundCor(i));
        }

        LoadSaveResult();
        ShowResult();
    }

    IEnumerator InitializeCor(Category category, Difficulty difficulty)
    {
        _canvasManager.ShowLoadingScreen(true);
        Result<Question[]> result = _triviaAPI.GetQuestions(category, difficulty, questionCount);
        yield return new WaitUntil(() => result.IsCompleted);
        _questions = result.Data;
        _canvasManager.ShowLoadingScreen(false);
        _canvasManager.ShowGameplay();
        
        _correctQuestions = 0;
        _points = 0;
        _result = null;
    }

    IEnumerator LoadMenuCor()
    {
        _canvasManager.ShowLoadingScreen(true);
        Result<Category[]> resultCategories = _triviaAPI.GetCategories();
        yield return new WaitUntil(() => resultCategories.IsCompleted);
        Categories = resultCategories.Data;

        Result<Difficulty[]> resultDifficulties = _triviaAPI.GetDifficulties();
        yield return new WaitUntil(() => resultDifficulties.IsCompleted);
        Difficulties = resultDifficulties.Data;

        _canvasManager.ShowLoadingScreen(false);
        _canvasManager.ShowSelector();
    }

    IEnumerator RoundCor(int index)
    {
        var question = _questions[index];
        PrepareRoundCanvas(question, index);

        var outTime = false;
        Timer.OnFinishDelegate onFinish = () => outTime = true;
        Timer.OnUpdateDelegate onUpdate = (t) => _canvasGameplay.SetTime(roundDuration - t);
        _timer.OnFinish += onFinish;
        _timer.OnUpdate += onUpdate;
        _timer.Play(roundDuration);

        yield return new WaitUntil(() => _canvasGameplay.ChoosedAnswer != null || outTime);

        _timer.Stop();
        _timer.OnFinish -= onFinish;
        _timer.OnUpdate -= onUpdate;

        CheckResponse(question.CorrectAnswer);
        ShowRoundResult(question.CorrectAnswer, index);
        yield return new WaitForSeconds(roundResultDuration);
    }

    private void CheckResponse(Answer solution)
    {
        var choosedAnswer = _canvasGameplay.ChoosedAnswer;

        if (choosedAnswer != null && choosedAnswer == solution)
        {
            _points += Mathf.CeilToInt((1 - _timer.Progress) * pointsQuestion);
            _correctQuestions++;
        }
    }

    private Answer[] GetAnswers(Question question)
    {
        var answers = new List<Answer>();
        answers.Add(question.CorrectAnswer);
        answers.AddRange(question.IncorrectAnswers);
        if (question.Type == Question.Types.Boolean)
            answers.Sort((a1, a2) => -a1.Sentence.CompareTo(a2.Sentence));
        else
            answers.Sort((a1, a2) => Random.value > 0.5f ? 1 : -1);
        return answers.ToArray();
    }

    private void LoadSaveResult()
    {
        if (!FileSystem.LoadEncrypt(FILE_LEADERBOARD, out _resultAll))
            _resultAll = new List<LeaderboardData>();

        _result = new LeaderboardData()
        {
            CorrectQuestions = _correctQuestions,
            Date = DateTime.UtcNow,
            Points = _points,
            TotalQuestions = _questions.Length
        };

        _resultAll.Add(_result);

        _resultAll.Sort((r1, r2) =>
        {
            var comparation = r2.Points.CompareTo(r1.Points);
            if (comparation == 0) comparation = r1.Date.CompareTo(r2.Date);
            return comparation;
        });
        for(int i = _resultAll.Count - 1; i >= COUNT_QUESTION_LEADERBOARD; i--)
        {
            _resultAll.RemoveAt(i);
        }

        FileSystem.SaveEncrypt(_resultAll, FILE_LEADERBOARD);
    }

    private void PrepareRoundCanvas(Question question, int index)
    {
        var answers = GetAnswers(question);
        _canvasGameplay.SetQuestion(question, index + 1);
        _canvasGameplay.SetScore(index, _points, _questions.Length, _correctQuestions);
        _canvasGameplay.SetAnswers(answers);
    }

    private void ShowResult()
    {
        _canvasResult.SetData(_result, _resultAll.ToArray());
        _canvasManager.ShowResult();
    }

    private void ShowRoundResult(Answer solution, int index)
    {
        _canvasGameplay.SetScore(index + 1, _points, _questions.Length, _correctQuestions);
        _canvasGameplay.ShowSolution(solution);
    }
}
