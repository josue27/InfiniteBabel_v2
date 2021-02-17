using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class Sharing_Control : MonoBehaviour
{
    public string mensaje = "Check this out https://lateshiftgame.com/";
    private const string nombreArchivo = "LS_score_screenshot";
    public void CompartirScore()
    {
        StartCoroutine(SaveScreenshot());
        Debug.Log("Compartiendo imagen...");
    }


    IEnumerator SaveScreenshot()
    {
        // Wait until the end of frame
        yield return new WaitForEndOfFrame();

        // The SaveScreenshot() method returns the path of the saved image
        // The provided file name will be added a ".png" extension automatically
        string path = Sharing.SaveScreenshot(nombreArchivo);
        Debug.Log("Imagen tomada y guardada");
        yield return new WaitForSeconds(0.1f);
        string pathFinal = System.IO.Path.Combine(Application.persistentDataPath, nombreArchivo+".png");

        Sharing.ShareImage(path, mensaje);
        Debug.Log("Imagen compartiendose");
    }
}
