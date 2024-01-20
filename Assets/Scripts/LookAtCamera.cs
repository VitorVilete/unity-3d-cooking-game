using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }

    [SerializeField] private Mode mode;
    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                // Older versions of Unity did not cache the main camera
                // You can use this on current versions of Unity
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                // Gets the direction pointing from the camera
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                // Instead of looking at the camera, look at the opposite direction
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
            default:
                break;
        }
    }
}
