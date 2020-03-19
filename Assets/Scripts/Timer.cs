using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public delegate void OnFinishDelegate();
    public delegate void OnUpdateDelegate(float time);
    public event OnFinishDelegate OnFinish;
    public event OnUpdateDelegate OnUpdate;

    private float _countdown;
    private bool _isPlaying;
    private float _time;

    public float Countdown => _time - _countdown;
    public bool IsPlaying => _isPlaying;
    public float Progress => _countdown / _time;

    void Update()
    {
        if (_isPlaying)
        {
            _countdown += Time.deltaTime;
            if(_countdown >= _time)
            {
                _isPlaying = false;
                OnFinish?.Invoke();
            }
            OnUpdate?.Invoke(_countdown);
        }
    }

    public void Play(float time)
    {
        _time = time;
        _countdown = 0;
        _isPlaying = true;
    }

    public void Stop()
    {
        _isPlaying = false;
    }
}
