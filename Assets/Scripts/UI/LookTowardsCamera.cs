using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTowardsCamera : MonoBehaviour
{
    private void Start()
    {
        Camera camera = Camera.main;
        transform.LookAt(2*transform.position - camera.transform.position);
    }

}
