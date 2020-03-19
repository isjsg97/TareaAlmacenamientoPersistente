using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLoading : MonoBehaviour
{
    [SerializeField] private RawImage iconIMG;
    [SerializeField] private float speed;

    private void OnEnable()
    {
        iconIMG.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }

    private void Update()
    {
        iconIMG.transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
