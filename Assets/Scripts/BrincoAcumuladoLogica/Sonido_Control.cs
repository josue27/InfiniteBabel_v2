using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brinco;
public class Sonido_Control : MonoBehaviour
{
    public static Sonido_Control sonidos;
    public AudioSource ui_AudioSource;
    public AudioSource musicaBackground_AudioSource;
    public AudioSource moneda_sfx;

    public AudioSource obstaculoPasado_sfx;

    public List<ClipSonido> clipsSonidos = new List<ClipSonido>();

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        sonidos = this;
        Eventos_Dispatcher.MonedaTomada +=ReproducirMoneda;
        Eventos_Dispatcher.CruceObstaculo += ReproducirObstaculoPasado;
    }
    public void ReproducirSonido_UI(string nombrePista)
    {
        foreach(ClipSonido clipSonido in clipsSonidos)
        {
            if(clipSonido.nombre.Equals(nombrePista))
            {
                ui_AudioSource.clip = clipSonido.clip;
                ui_AudioSource.Play();
                break;
            }
        }
    }
    private void ReproducirMoneda()
    {
        if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio)
         return;
        moneda_sfx.Play();
    }
    private void ReproducirObstaculoPasado()
    {
        if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio)
         return;
        if(obstaculoPasado_sfx)
         obstaculoPasado_sfx.Play();
    }
    //cgs-07 brinco
    //cgs-30 paso de obstaculo?
    //cgs-45 moneda?
    //cgs-47 brinco regreso
    //cgs-48 caida perdio
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        Eventos_Dispatcher.MonedaTomada -=ReproducirMoneda;
        
    }
}
[System.Serializable]
public class ClipSonido
{
    public string nombre;//conviene que tenga nombre de la ui como "botonPlay" o nombre de la Accion
    public AudioClip clip;
}