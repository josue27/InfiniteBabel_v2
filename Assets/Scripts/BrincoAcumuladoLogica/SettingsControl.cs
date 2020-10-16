using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brinco;

public class SettingsControl : MonoBehaviour
{
    public GameObject panelSettings;

    public GameObject panelBotonesSuperiores;

    [SerializeField] Vector3 posPanelBotonesSuperiores_Activados;
    [SerializeField] Vector3 posPanelBotonesSuperiores_Desactivados;
    private void Awake()
    {
        // posPanelBotonesSuperiores_Activados = panelBotonesSuperiores.transform.position;
         panelBotonesSuperiores.transform.localPosition = posPanelBotonesSuperiores_Desactivados;
         IntroJuego();
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
    }
   

    void IntroJuego()
    {
        LeanTween.moveLocalY(panelBotonesSuperiores, posPanelBotonesSuperiores_Activados.y, 0.5f).setEaseInOutSine();
        Debug.Log("SE MOVIERON BOTONES");
    }
    void InicioJuego()
    {
        LeanTween.moveLocalY(panelBotonesSuperiores, posPanelBotonesSuperiores_Desactivados.y, 0.5f).setEaseInOutSine();

    }
    public void Toggle_PanelSettings()
    {
        if (!panelSettings) return;

        panelSettings.SetActive(!panelSettings.activeInHierarchy);
    }
        
     
}
