using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowardsCamera : MonoBehaviour
{
    [SerializeField] bool alignWithCameraRotation = false;
    private void Start()
    {
        Camera camera = Camera.main;
        if (alignWithCameraRotation)
        {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        }
        else
        {
            transform.LookAt(2 * transform.position - camera.transform.position);
        }
    }


}
