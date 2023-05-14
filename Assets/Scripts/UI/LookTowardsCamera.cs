using UnityEngine;

/// <summary>
/// This class is responsible of aligning the gameObject to the main camera (considering rotation or not)
/// </summary>
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
