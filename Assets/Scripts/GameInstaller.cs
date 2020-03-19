using OpenTDB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TrivialAPI triviaAPI;

    public override void InstallBindings()
    {
        Container.BindInstance(canvasManager);
        Container.BindInstance(gameManager);
        Container.BindInstance(triviaAPI);
    }
}
