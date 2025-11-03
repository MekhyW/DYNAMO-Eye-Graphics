using UnityEngine;
using System.Collections;

public class ActivateAllDisplays : MonoBehaviour
{
    public Camera[] cameras;
    private float aspectRatio = 5/3f;

    void Start()
    {
        Application.targetFrameRate = 60;
        Debug.Log("displays connected: " + Display.displays.Length);
        for (int i = 0; i < Display.displays.Length; i++) {
            Display.displays[i].Activate();
            float inputAspectRatio = (float)Display.displays[i].systemWidth / Display.displays[i].systemHeight;
            if (inputAspectRatio < aspectRatio) {
                Matrix4x4 projectionMatrix = cameras[i].projectionMatrix;
                float scale = inputAspectRatio / aspectRatio;
                projectionMatrix.m00 *= 0.62f * scale; //magic number due to unknown distortion caused by adapter
                projectionMatrix.m02 += 0.04f;         //magic number due to unknown distortion caused by adapter
                cameras[i].projectionMatrix = projectionMatrix;
            }
        }
    }
}