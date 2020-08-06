using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using BayatGames.SaveGameFree;
using TMPro;
using Brinco;
public class Score_Control : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject panelScore;
    public GameObject panelGlobal,panelLocal;
    public Transform panelScore_posAbierta;
    public Transform panelScore_posCerrada;
    public Transform panelScore_posGlobal;

    public GameObject botonAbrirGlobal;
    public GameObject botonCerrarGlobal;
    public float velocidadApertura = 1.0f;
    public bool panelScoreAbierto;
    [Header("Scores")]
    [SerializeField]
    private int highscoreLocal;
    private int scoreRonda;

    private string nombreSlotHighscore = "highscore";

    [Header("UIX Juego")]
    public Animator monitorAnimator;
    public TMP_Text highScoreLocal_text;
    void Start()
    {
       // AbrirPanelScore();
       Eventos_Dispatcher.CruceObstaculo += ObstaculoCruzado;
       Eventos_Dispatcher.MonedaTomada += MonedaTomada;
       Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
        panelScore.transform.position = panelScore_posCerrada.position;
        if(SaveGame.Exists(nombreSlotHighscore)){
            CargarScoreLocal();
            AbrirPanelScore();
        }else{
            Debug.Log("No hay scores, debe ser primeriso");
        }
    }

    [Button("Abrir Cerrar Panel Score")]
    public void AbrirPanelScore()
    {
        highScoreLocal_text.SetText(highscoreLocal.ToString());

        if(!panelScoreAbierto)
         {
             LeanTween.moveY(panelScore,panelScore_posAbierta.position.y,velocidadApertura).setEaseOutBounce();
             panelScoreAbierto = true;
           

         }else{
             LeanTween.moveY(panelScore,panelScore_posCerrada.position.y,velocidadApertura).setEaseInBounce();
             panelScoreAbierto = false;
           
         }
         
    }
    public void AbrirPanelScore_Global(bool _abrir)
    {
        if(_abrir)
        {
         LeanTween.moveY(panelScore,panelScore_posGlobal.position.y,velocidadApertura).setEaseOutBounce();
         botonAbrirGlobal.gameObject.SetActive(false);
         botonCerrarGlobal.gameObject.SetActive(true);
           panelGlobal.SetActive(true);
             panelLocal.SetActive(false);
        }
        else if(!_abrir)
        {
         LeanTween.moveY(panelScore,panelScore_posAbierta.position.y,velocidadApertura).setEaseOutBounce();
          botonAbrirGlobal.gameObject.SetActive(true);
         botonCerrarGlobal.gameObject.SetActive(false);
           panelGlobal.SetActive(false);
             panelLocal.SetActive(true);
         }

        
    }
    private void InicioJuego()
    {
        if(panelScoreAbierto)
        {
            AbrirPanelScore();
        }
    }
    /// <summary>
    /// Activado por EventDispatcher cuando el jugador pierde
    /// </summary>
    private void FinJuego()
    {
        CompararScore();
    }
    private void CompararScore()
    {
        //en teoria highscoreLocal ya deberia estar cargado
        if(highscoreLocal > 0  )
        {   
            if(scoreRonda > highscoreLocal)
            {
                SaveGame.Save<int>(nombreSlotHighscore,scoreRonda);
                Debug.Log("Highscore :"+highscoreLocal+" superado guardano nuevo: "+scoreRonda);
            }
        }else{

            SaveGame.Save<int>(nombreSlotHighscore,scoreRonda);
            Debug.Log("Primer highscore salvado:" + scoreRonda);
        }
        //para que se actualice el Highscore durante la partida
        CargarScoreLocal();
    }
    public void ToggleScoreGlobal()
    {

    }

    public void ObstaculoCruzado()
    {
        if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio)
         return;
         
        MonitorAnimar("obstaculo");
        scoreRonda++;
    }
    public void MonedaTomada()
    {
        if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio)
         return;
        MonitorAnimar("moneda");
    }
    /// <summary>
    /// Activa la animacion de sprite del monitor de score del juego
    /// </summary>
    /// <param name="trigger">moneda,obstaculo</param>
    public void MonitorAnimar(string trigger)
    {
        monitorAnimator.SetTrigger(trigger);
    }
    public void MonitorAnimar(string nombreParametro,bool estado){

    }
    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
         Eventos_Dispatcher.CruceObstaculo -= ObstaculoCruzado;
         Eventos_Dispatcher.MonedaTomada -= MonedaTomada;
          Eventos_Dispatcher.eventos.JugadorPerdio -= FinJuego;
        Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
    }

    public void CargarScoreLocal()
    {
        if(SaveGame.Exists(nombreSlotHighscore))
             highscoreLocal = SaveGame.Load<int>(nombreSlotHighscore);
    }
}
