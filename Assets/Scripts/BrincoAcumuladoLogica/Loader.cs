using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EasyMobile;

public class Loader : MonoBehaviour
{
    public string scenaJuego = "brincoAcumulado_03_Perspective";
    // Start is called before the first frame update
    public Image fillImagen;

    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
        
    }
    void Start()
    {
        StartCoroutine(CargaAsync());   
    }
    IEnumerator CargaAsync()
    {
        yield return new WaitForSeconds(2.0f);
        AsyncOperation loader = SceneManager.LoadSceneAsync(scenaJuego);
       // loader.allowSceneActivation = false;
        while (loader.isDone)
        {
            fillImagen.fillAmount = loader.progress / 100.0f;
            Debug.Log($"Cargando: {loader.progress / 100.0f}");
            yield return null;
        }
        //loader.allowSceneActivation = true;
    }
    
}
