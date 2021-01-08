using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brinco;

public class SettingsControl : MonoBehaviour
{
    public GameObject panelSettings;

    public GameObject panelBotonesSuperiores;
    public GameObject panelIdiomas;
    public GameObject panelCreditos;

    [SerializeField] Vector3 posPanelBotonesSuperiores_Activados;
    [SerializeField] Vector3 posPanelBotonesSuperiores_Desactivados;

    private void Awake()
    {
        // posPanelBotonesSuperiores_Activados = panelBotonesSuperiores.transform.position;
         panelBotonesSuperiores.transform.localPosition = posPanelBotonesSuperiores_Desactivados;
         IntroJuego();
         Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
    }
    private void Start()
    {
        Eventos_Dispatcher.Reinicio += Reinicio;
    }
    private void OnDisable()
    {
        Eventos_Dispatcher.Reinicio -= Reinicio;

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
    
    public void Toggle_PanelIdiomas()
    {
        if (!panelIdiomas)
        {
            Debug.Log("No hay panel idiomas");
            return;
        }

        panelIdiomas.SetActive(!panelIdiomas.activeInHierarchy);
    }
     
    public void Toggle_PanelCreditos()
    {
        if(!panelCreditos)
        {
            Debug.Log("No hay panel creditos");
            return;

        }
        panelCreditos.SetActive(!panelCreditos.activeInHierarchy);
    }

    void Reinicio()
    {
        LeanTween.moveLocalY(panelBotonesSuperiores, posPanelBotonesSuperiores_Activados.y, 0.5f).setEaseInOutSine();
    }
}
