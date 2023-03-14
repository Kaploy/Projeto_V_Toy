using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LookAtCamera : MonoBehaviour
{
    public Camera newCamera;
    // Start is called before the first frame update
    void Start()
    {
        newCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + newCamera.transform.rotation * Vector3.forward, newCamera.transform.rotation * Vector3.up);
    }
}
