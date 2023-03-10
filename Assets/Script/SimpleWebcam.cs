using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWebcam : MonoBehaviour
{

    //IEnumerator Start()
    //{
    //    findWebCams();

    //    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
    //    if (Application.HasUserAuthorization(UserAuthorization.WebCam))
    //    {
    //        Debug.Log("webcam found");
    //    }
    //    else
    //    {
    //        Debug.Log("webcam not found");
    //    }

    //    findMicrophones();

    //    yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
    //    if (Application.HasUserAuthorization(UserAuthorization.Microphone))
    //    {
    //        Debug.Log("Microphone found");
    //    }
    //    else
    //    {
    //        Debug.Log("Microphone not found");
    //    }
    //}

    //void findWebCams()
    //{
    //    foreach (var device in WebCamTexture.devices)
    //    {
    //        Debug.Log("Name: " + device.name);
    //    }
    //}

    //void findMicrophones()
    //{
    //    foreach (var device in Microphone.devices)
    //    {
    //        Debug.Log("Name: " + device);
    //    }
    //}

    void Start()
    {
        WebCamTexture webcamTexture = new WebCamTexture();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

}
