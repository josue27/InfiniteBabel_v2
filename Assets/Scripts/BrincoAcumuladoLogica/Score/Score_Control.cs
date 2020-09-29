using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using BayatGames.SaveGameFree;
using TMPro;
using Brinco;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using EasyMobile;
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
    private string nombreSlotPersonajeUsado = "nombrePersonaje" ;

    [Header("UIX Juego")]
    public Animator monitorAnimator;
    public TMP_Text highScoreLocal_text;

    public TMP_Text debug_text;


     private void Awake()
    {
        if(!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
    }
    void Start()
    {
       // AbrirPanelScore();
       Eventos_Dispatcher.CruceObstaculo += ObstaculoCruzado;
       Eventos_Dispatcher.MonedaTomada += MonedaTomada;
       Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
       Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
       Eventos_Dispatcher.eventos.GuardarPersonaje += GuardarPersonajeSeleccionado;

        panelScore.transform.position = panelScore_posCerrada.position;

        if(SaveGame.Exists(nombreSlotHighscore)){
            CargarScoreLocal();
            AbrirPanelScore();
            CargarPersonajeGuardado();
        }else{
            Debug.Log("No hay scores, debe ser primeriso");
        }
    }

    [Button("Abrir Cerrar Panel Score")]
    public void AbrirPanelScore()
    {
       // highScoreLocal_text.SetText(highscoreLocal.ToString());

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
        //en teoria highscoreLocal ya deberia estar cargado y solamente checamos que sea mas de 0 para que
        //valga la pena
        if(highscoreLocal > 0  )
        {   
            if(scoreRonda > highscoreLocal)
            {
                SaveGame.Save<int>(nombreSlotHighscore,scoreRonda);
                DesbloquearLogro();

                SubirScoreGooglePlay(scoreRonda);
                Debug.Log("Highscore :"+highscoreLocal+" superado guardano nuevo: "+scoreRonda);
            }
        }else{//debe significar que no habia score y debe ser su primer juego

            SaveGame.Save<int>(nombreSlotHighscore,scoreRonda);
            DesbloquearLogro();

            SubirScoreGooglePlay(scoreRonda);
            Debug.Log("Primer highscore salvado:" + scoreRonda);
        }
        //para que se actualice el Highscore durante la partida
        CargarScoreLocal();
    }
    public void ToggleScoreGlobal()
    {

    }
    public void MostrarGoogleAchievemnts() 
    {
        // if (PlayGamesPlatform.Instance.localUser.authenticated) {
        //     PlayGamesPlatform.Instance.ShowAchievementsUI();
        // }
        // else {
        //   Debug.Log("Cannot show Achievements, not logged in");
        //   debug_text.text = "No se pueden mostrar los logros";
        // }

        if(GameServices.IsInitialized())
        {

            GameServices.ShowAchievementsUI();
        }else
        {
            #if UNITY_ANDROID
            GameServices.Init();    // start a new initialization process
            #elif UNITY_IOS
            Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
            #endif
        }
    }
    public void MostrarGoogleLeaderBoard()
    {
        // if(PlayGamesPlatform.Instance.localUser.authenticated)
        // {
        //     PlayGamesPlatform.Instance.ShowLeaderboardUI();
        // }else{
        //     debug_text.SetText("No esta conectado o no inicio sesion");
        // }
        if(GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }else{
            debug_text.text = "Fallo al mostar Leaderborad";
        }
    }

    public void SubirScoreGooglePlay(int nuevoScore)
    {
         // Submit leaderboard scores, if authenticated
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                // Note: make sure to add 'using GooglePlayGames'
                PlayGamesPlatform.Instance.ReportScore(nuevoScore,
                    GPGSIds.leaderboard_the_best_runner,
                    (bool success) =>
                    {
                        Debug.Log("(Google) Leaderboard update success: " + success);
                        debug_text.text = "(Google) Leaderboard update success: " + success;
                    });

				//WriteUpdatedScore();
				
            }

            if(GameServices.IsInitialized())
            {
                GameServices.ReportScore(nuevoScore,EM_GPGSIds.leaderboard_the_best_runner,(bool exito)=>{
                    debug_text.text ="Se subio el score exitosamente";
                });
            }

    }

    public void DesbloquearLogro()
    {
        // if(Social.localUser.authenticated)
        // {
        //     PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_welcome_to_the_late_shift,100.0f,
        //     (bool success)=>{
        //         debug_text.text = "Logro desbloqueado:"+success;
        //     });
        // }
        if(PlayGamesPlatform.Instance.localUser.authenticated)
        {
             PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_welcome_to_the_late_shift,100.0f,
            (bool success)=>{
                debug_text.text = "Logro desbloqueado:"+success;
            });
        }
        if(GameServices.IsInitialized())
        {
            GameServices.UnlockAchievement(EM_GPGSIds.achievement_welcome_to_the_late_shift,
            (bool exito)=>
            {
                debug_text.text = "GS:"+exito;
            }
            );
        }
    }
    public void GameServicesMensajes(bool escito)
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
        // if(SaveGame.Exists(nombreSlotHighscore))
        //      highscoreLocal = SaveGame.Load<int>(nombreSlotHighscore);


        GameServices.LoadLocalUserScore(EM_GPGSIds.leaderboard_the_best_runner,OnLocalUserScoreLoaded);
    }
    void OnLocalUserScoreLoaded(string leaderboardname,IScore scoreCargado)
    {
        if(scoreCargado != null)
        {
            debug_text.text=$"{scoreCargado.value}";
             highScoreLocal_text.SetText(highscoreLocal.ToString());


        }else
        {
        debug_text.text=$"Problema con el escore";

        }
    }
    /// <summary>
    /// Se encarga de buscar el slot del personaje guardado, 
    /// si existe le manda el nombre del Personaje para buscarlo
    /// </summary>
    public void CargarPersonajeGuardado()
    {
        if(SaveGame.Exists(nombreSlotPersonajeUsado))
        {
            string p = SaveGame.Load<string>(nombreSlotPersonajeUsado);
            SeleccionPersonaje._seleccionPersonaje?.BuscarPersonaje(p);
        }
        Debug.Log("Se mando cambio de personaje guardado");
    }

    private void GuardarPersonajeSeleccionado(string _nombrePersonaje)
    {
        SaveGame.Save(nombreSlotPersonajeUsado,_nombrePersonaje);
        Debug.Log("Personaje: "+_nombrePersonaje+" guardado para siguiente partida");
    }
}
