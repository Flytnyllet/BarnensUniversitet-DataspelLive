using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform camera;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + camera.forward);
    }
}
