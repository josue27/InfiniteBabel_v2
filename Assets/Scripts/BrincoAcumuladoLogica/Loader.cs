using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EasyMobile;
#if EM_UIAP
using UnityEngine.Purchasing;
#endif
public class Loader : MonoBehaviour
{
    public string scenaJuego = "brincoAcumulado_03_Perspective";
    // Start is called before the first frame update
    public Image fillImagen;
    public Slider sliderPorcentaje;
    public GameObject mobileConsole;
    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
        if(!InAppPurchasing.IsInitialized())
        {
            InAppPurchasing.InitializePurchasing();
        }
// #if DEVELOPMENT_BUILD
//         Debug.Log("DEVELOPMENT_BUILD");
//         mobileConsole.SetActive(true);
// #endif

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
        while (!loader.isDone)
        {
            //sliderPorcentaje.value = loader.progress;
            fillImagen.fillAmount = loader.progress;
            //Debug.Log($"Cargando: {loader.progress}");
            yield return null;
        }
        //loader.allowSceneActivation = true;
    }
    
}
