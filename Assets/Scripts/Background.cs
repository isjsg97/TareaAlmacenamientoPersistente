using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [SerializeField] private Image backgroundIMG;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 changeDirectionTime;

    private Material _material;
    private int _direction;
    private float _angle;

    void Start()
    {
        _material = backgroundIMG.material;
        _angle = Random.Range(0, 360);

        _material.SetColor("_Color", color1);
        _material.SetColor("_Color2", color2);
        UpdateMaterial();

        StartCoroutine(ChangeDirectionCor());
    }

    
    void Update()
    {
        _angle += speed * _direction * Time.deltaTime;
        UpdateMaterial();
    }

    IEnumerator ChangeDirectionCor()
    {
        while (true)
        {
            _direction = Random.value < 0.5 ? -1 : 1;
            yield return new WaitForSeconds(Random.Range(changeDirectionTime.x, changeDirectionTime.y));
        }
    }

    void UpdateMaterial()
    {
        _material.SetFloat("_Angle", _angle);
    }
}
