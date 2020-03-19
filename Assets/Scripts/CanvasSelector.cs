using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using OpenTDB;

public class CanvasSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown categoriesDPD;
    [SerializeField] private TMP_Dropdown difficultiesDPD;
    [SerializeField] private Button exitBTN;
    [SerializeField] private Button playBTN;

    private Category[] _categories;
    private Difficulty[] _difficulties;

    [Inject]
    private GameManager _gameManager;

    private void Start()
    {
        _categories = _gameManager.Categories;
        categoriesDPD.ClearOptions();
        categoriesDPD.AddOptions(_categories.Select(c => c.Name).ToList());

        _difficulties = _gameManager.Difficulties;
        difficultiesDPD.ClearOptions();
        difficultiesDPD.AddOptions(_difficulties.Select(c => c.Name).ToList());

        exitBTN.onClick.AddListener(_gameManager.Quit);
        playBTN.onClick.AddListener(Play);
    }

    private void Play()
    {
        var category = _categories[categoriesDPD.value];
        var difficulty = _difficulties[difficultiesDPD.value];
        _gameManager.Play(category, difficulty);
    }

}
