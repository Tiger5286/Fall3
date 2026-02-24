using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 30.0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
    }
}
