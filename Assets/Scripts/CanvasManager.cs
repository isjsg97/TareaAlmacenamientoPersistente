using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private CanvasGameplay gameplay;
    [SerializeField] private CanvasLoading loading;
    [SerializeField] private CanvasResult result;
    [SerializeField] private CanvasSelector selector;

    public CanvasGameplay CanvasGameplay => gameplay;
    public CanvasResult CanvasResult => result;

    private void Awake()
    {
        gameplay.gameObject.SetActive(false);
        loading.gameObject.SetActive(false);
        result.gameObject.SetActive(false);
        selector.gameObject.SetActive(false);
    }

    public void ShowGameplay()
    {
        gameplay.gameObject.SetActive(true);
        result.gameObject.SetActive(false);
        selector.gameObject.SetActive(false);
    }

    public void ShowLoadingScreen(bool show)
    {
        loading.gameObject.SetActive(show);
    }

    public void ShowResult()
    {
        gameplay.gameObject.SetActive(false);
        result.gameObject.SetActive(true);
        selector.gameObject.SetActive(false);
    }

    public void ShowSelector()
    {
        gameplay.gameObject.SetActive(false);
        result.gameObject.SetActive(false);
        selector.gameObject.SetActive(true);
    }
}
