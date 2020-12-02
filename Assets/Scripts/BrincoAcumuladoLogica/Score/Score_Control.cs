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
using System;


public class Score_Control : MonoBehaviour
{
    public static Score_Control instancia;
    // Start is called before the first frame update
    public GameObject panelScore;
    public GameObject panelGlobal, panelLocal;
    public Transform panelScore_posAbierta;
    public Transform panelScore_posCerrada;
    public Transform panelScore_posGlobal;

    public GameObject botonAbrirGlobal;
    public GameObject botonCerrarGlobal;
    public float velocidadApertura = 1.0f;
    public bool panelScoreAbierto;

    [Header("Scores")]
    [SerializeField]
    private int highScoreUsuario;
    private int scoreRonda;
    public int HighscoreUsuario { get => highScoreUsuario; set => highScoreUsuario = value; }
    public int ScoreRonda { get => scoreRonda; set => scoreRonda = value; }

    private string nombreSlotHighscore = "highscore";
    private string nombreSlotPersonajeUsado = "nombrePersonaje";

    //Monedas
    private int monedasTotales;
    private int monedasPartida = 0;
   

    [Header("UIX Monedas")]
    public TMP_Text monedasPartidas_txt;
    public TMP_Text monedasTotales_txt;

    public int MonedasTotales
    {
        get => monedasTotales;
        private set
        {
            monedasTotales = value;
            monedasTotales_txt.text = monedasTotales.ToString("0000");
        }
    }

    public int MonedasPartida {
        get => monedasPartida;
        private set {
            monedasPartida = value;
            monedasPartidas_txt.text = MonedasPartida.ToString("0000");
        }
    }

    [Header("UIX Juego")]
    public Animator monitorAnimator;

    public TMP_Text highScoreUsuario_text;
    public TMP_Text monedasUsuario_text;

    public TMP_Text debug_text;
    public GameObject monitorHighScore_inicial;
    public GameObject highScore_tabla;
    public GameObject[] scoreSlot;


    private SavedGame juegoSalvado;

    private string saveFilePath;
    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
        instancia = this;
        saveFilePath = Application.persistentDataPath + "/save.data";
    }
    void Start()
    {
        //CargarScoreGlobal();
        // AbrirPanelScore();
        Eventos_Dispatcher.CruceObstaculo += ObstaculoCruzado;
        Eventos_Dispatcher.MonedaTomada += MonedaTomada;
        Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
        Eventos_Dispatcher.eventos.GuardarPersonaje += GuardarPersonajeSeleccionado;

        panelScore.transform.position = panelScore_posCerrada.position;

        if (SaveGame.Exists(nombreSlotHighscore))
        {
            CargarScoreUsuario();
            CargarMonedas();
            CargarPersonajeGuardado();
           // AbrirPanelScore();//Pasamos esta funcion a CargarScoreUsuario() para que muestre score despues de calcular


        }
        else
        {
            Debug.Log("No hay scores, debe ser primeriso");
        }



    }

    void OnDestroy()
    {
        Eventos_Dispatcher.CruceObstaculo -= ObstaculoCruzado;
        Eventos_Dispatcher.MonedaTomada -= MonedaTomada;
        Eventos_Dispatcher.eventos.JugadorPerdio -= FinJuego;
        Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
    }

    private void InicioJuego()
    {
        // if(panelScoreAbierto)
        // {
        //     AbrirPanelScore();
        // }

        LeanTween.moveLocal(monitorHighScore_inicial, new Vector3(-600.0f, 0f, 0f), 0.5f).setEaseInOutSine();

        MonedasPartida = MonedasTotales;
    }

    /// <summary>
    /// Activado por EventDispatcher cuando el jugador pierde
    /// </summary>
    private void FinJuego()
    {
        CompararScore();
        IngresarMonedasPartida();
    }

    

    private void CompararScore()
    {
        //en teoria highscoreLocal ya deberia estar cargado y solamente checamos que sea mas de 0 para que
        //valga la pena
        if (highScoreUsuario > 0)
        {
            if (scoreRonda > highScoreUsuario)//Si se llega a superar el score guardado
            {

                SubirScoreGooglePlay(scoreRonda);
                Debug.Log("Highscore :" + highScoreUsuario + " superado guardano nuevo: " + scoreRonda);
            }
            DesbloquearLogro();

        } else {//debe significar que no habia score y debe ser su primer juego

            ///Desbloqueamos logro de primera vez
            DesbloquearLogro();

            SubirScoreGooglePlay(scoreRonda);
            Debug.Log("Primer highscore salvado:" + scoreRonda);
        }
        //para que se actualice el Highscore durante la partida
        CargarScoreUsuario();
    }
    

    [Button("Abrir Cerrar Panel Score")]
    public void AbrirPanelScore()
    {
        //Abirr monitor de score inicial
        LeanTween.moveLocal(monitorHighScore_inicial, new Vector3(0f, 0f, 0f), 0.5f).setEaseInOutSine();       // highScoreLocal_text.SetText(highscoreLocal.ToString());

        if (!panelScoreAbierto)
        {
            LeanTween.moveY(panelScore, panelScore_posAbierta.position.y, velocidadApertura).setEaseOutBounce();
            panelScoreAbierto = true;


        }
        else
        {
            LeanTween.moveY(panelScore, panelScore_posCerrada.position.y, velocidadApertura).setEaseInBounce();
            panelScoreAbierto = false;

        }

    }
    public void AbrirPanelScore_Global(bool _abrir)
    {

        highScore_tabla.gameObject.SetActive(_abrir);
        if (_abrir) CargarScoreGlobal();

    }

    /// <summary>
    /// Llamado por UI por el usuario, muestra el tablero de Logros de Google
    /// </summary>
    public void MostrarGoogleAchievemnts()
    {
        // if (PlayGamesPlatform.Instance.localUser.authenticated) {
        //     PlayGamesPlatform.Instance.ShowAchievementsUI();
        // }
        // else {
        //   Debug.Log("Cannot show Achievements, not logged in");
        //   debug_text.text = "No se pueden mostrar los logros";
        // }

        if (GameServices.IsInitialized())
        {

            GameServices.ShowAchievementsUI();
        } else
        {
#if UNITY_ANDROID
            GameServices.Init();    // start a new initialization process
#elif UNITY_IOS
            Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
        }
    }

    /// <summary>
    /// Llamado por UI por el usuario, muestra el tablero nativo de Score de Google
    /// </summary>
    public void MostrarGoogleLeaderBoard()
    {


        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        } else {
            debug_text.text = "Fallo al mostar Leaderborad";
        }
    }

    /// <summary>
    /// Maneja la subida de un HighScore logrado, llamado por CompararScore(),
    /// tambien salva el score localmente por si las dudas
    /// </summary>
    /// <param name="nuevoScore"></param>
    public void SubirScoreGooglePlay(int nuevoScore)
    {

        //Salvamos primer el score localmente por si no hay internet
        SaveGame.Save<int>(nombreSlotHighscore, nuevoScore);

        if (GameServices.IsInitialized())
        {
            GameServices.ReportScore(nuevoScore, EM_GPGSIds.leaderboard_the_best_runner, (bool exito) => {
                debug_text.text = "Se subio el score exitosamente";
            });
        }
        else {
#if UNITY_ANDROID
            GameServices.Init();    // start a new initialization process

#elif UNITY_IOS
            Debug.Log("Cannot show  Upload score);
#endif
        }


    }

    public void DesbloquearLogro()
    {

        if (GameServices.IsInitialized())
        {
            GameServices.UnlockAchievement(EM_GPGSIds.achievement_welcome_to_the_late_shift,
            (bool exito) =>
            {
                debug_text.text = "GS:" + exito;
            }
            );
        }
    }

    public void GameServicesMensajes(bool escito)
    {

    }

    /// <summary>
    /// Lleva la cuenta de los obstaculos cruzados, ojo Master_Level.cs tambien lleva la cuenta pero
    /// para aumentar la dificultad, este scoreRonda es el que se guarda y publica
    /// </summary>
    public void ObstaculoCruzado()
    {
        if (Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio)
            return;

        MonitorAnimar("obstaculo");
        scoreRonda++;
    }
    
    /// <summary>
    /// Activa la animacion de sprite del monitor de score del juego
    /// </summary>
    /// <param name="trigger">moneda,obstaculo</param>
    public void MonitorAnimar(string trigger)
    {
        monitorAnimator.SetTrigger(trigger);
    }
    //public void MonitorAnimar(string nombreParametro, bool estado) {

    //}
  
   

    public void CargarScoreUsuario()
    {
        // if(SaveGame.Exists(nombreSlotHighscore))
        //      highscoreLocal = SaveGame.Load<int>(nombreSlotHighscore);


        GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_Obstaculos, OnLocalUserScoreLoaded);
    }

    void OnLocalUserScoreLoaded(string leaderboardname, IScore scoreCargado)
    {
        if (scoreCargado != null)
        {
            debug_text.text = $"Score Google Play:{scoreCargado.value}";
            HighscoreUsuario = int.Parse(scoreCargado.formattedValue);


            int highLocal;
            if (SaveGame.Exists(nombreSlotHighscore))
            {
                highLocal = SaveGame.Load<int>(nombreSlotHighscore);
                if (highLocal > HighscoreUsuario)
                {
                    HighscoreUsuario = highLocal;
                }else
                {
                    //Si el guardado local no supera al de GoogleServices entonces asignamos el de la nube
                    //quiere decir que hubo un conflicto en el guardado
                    SaveGame.Save<int>(nombreSlotHighscore,HighscoreUsuario);
                }
            }


            
        }
        else
        {
            debug_text.text = $"Problema cargando el score local de GooglePlay cargando score Local";
            if (SaveGame.Exists(nombreSlotHighscore))
                HighscoreUsuario = SaveGame.Load<int>(nombreSlotHighscore);


            debug_text.text = $"Score Local cargado{highScoreUsuario}";
            GameServices.Init();

            //if (Master_Level._masterBrinco.estadoJuego != EstadoJuego.jugando)
            //    CargarScoreUsuario();
        }
        highScoreUsuario_text.SetText(HighscoreUsuario.ToString());

        AbrirPanelScore();

    }


    /// <summary>
    /// Abre el score global de Game service asi como la nueva tabla
    /// </summary>
    public void CargarScoreGlobal()
    {

        GameServices.LoadScores(EM_GameServicesConstants.Leaderboard_Obstaculos, 0, 10, TimeScope.Today, UserScope.Global, OnScoresLoaded);
    }
    // Scores loaded callback
    void OnScoresLoaded(string leaderboardName, IScore[] scores)
    {
        if (scores != null && scores.Length > 0)
        {
            Debug.Log("Scores Globales cargados");
            for (int i = 0; i < scores.Length; i++)
            {
                scoreSlot[i].transform.GetChild(0).GetComponent<TMP_Text>().text = $"{scores[i].rank}";
                scoreSlot[i].transform.GetChild(1).GetComponent<TMP_Text>().text = scores[i].userID;
                scoreSlot[i].transform.GetChild(2).GetComponent<TMP_Text>().text = $"{scores[i].value}";
            }
        } else {
            Debug.Log("Advertencia Hubo un problema al cargar los scores globales");
            for (int i = 0; i < scoreSlot.Length; i++)
            {
                scoreSlot[i].transform.GetChild(0).GetComponent<TMP_Text>().text = $"{i + 1}";
                scoreSlot[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "none...";
                scoreSlot[i].transform.GetChild(2).GetComponent<TMP_Text>().text = $"{000}";
            }
        }
    }

    /// <summary>
    /// Funcion de debug para rellenar el tablero de Highscores
    /// </summary>
    [Button]
    void Rellenar() {
        for (int i = 0; i < scoreSlot.Length; i++)
        {
            scoreSlot[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "none...";
            scoreSlot[i].transform.GetChild(1).GetComponent<TMP_Text>().text = $"{000}";
        }

    }


    #region Skin Personaje
    /// <summary>
    /// Se encarga de buscar el slot del personaje guardado localelmente, 
    /// si existe le manda el nombre del Personaje para buscarlo
    /// </summary>
    public void CargarPersonajeGuardado()
    {
        if (SaveGame.Exists(nombreSlotPersonajeUsado))
        {
            string p = SaveGame.Load<string>(nombreSlotPersonajeUsado);
            SeleccionPersonaje._seleccionPersonaje?.BuscarPersonaje(p);
            Debug.Log("Se mando cambio de personaje guardado");
        }
        else
        {
            Debug.Log("No se encontro Skin Personaje guardado");
        }
        
    }

    /// <summary>
    /// Guarda el personaje que el Usuario selecciono de manera local
    /// </summary>
    /// <param name="_nombrePersonaje"></param>
    private void GuardarPersonajeSeleccionado(string _nombrePersonaje)
    {
        SaveGame.Save(nombreSlotPersonajeUsado, _nombrePersonaje);
        Debug.Log("Personaje: " + _nombrePersonaje + " guardado para siguiente partida");
    }
    #endregion

    #region  Monedas

    /// <summary>
    /// Llamado por EventDispatcher cuando el jugador toma una moneda
    /// </summary>
    public void MonedaTomada()
    {
        if (Master_Level._masterBrinco.estadoJuego == EstadoJuego.perdio)
            return;
        MonitorAnimar("moneda");
        MonedasPartida++;
    }

    /// <summary>
    /// Inicia el comando para cargar el score en la nube y buscar cuantas monedas tiene el jugador
    /// 
    /// </summary>
    public void CargarMonedas()
    {
        OpenSavedGame("leer");
    }

    /// <summary>
    /// Llamado cuando acaba el juego, se encarga de sumar las monedas ganadas
    /// </summary>
    private void IngresarMonedasPartida()
    {
        //Se supone que MonedasPartida es > MonedasTotales etnonces deberia dar positivo , haces esto para dar el efecto de ganancia?
        if (MonedasPartida <= 0)
        {
            Debug.Log("No hay monedas para guardar saltando...");
            return;
        }
        int totalPartida = Mathf.Abs( MonedasPartida - MonedasTotales);
        SumarMonedas(totalPartida);
    
    }

    
    public void SumarMonedas(int cantidad)
    {
        MonedasTotales += cantidad;
        //GuardarMonedas();
    }
    public void RestarMonedas(int cantidad)
    {
        MonedasTotales -= cantidad;
    }

 
    public void GuardarMonedas()
    {
        if (RuntimeManager.IsInitialized())
        {
            OpenSavedGame("guardar");
        }
        else
        {
            Master_Level._masterBrinco.Reiniciar_Callback();
        }
    }



    /// <summary>
    /// Open a saved game with automatic conflict resolution
    /// Tienes que indicar que quieres hacer depues de abrir el archivo
    /// </summary>
    /// <param name="caso">leer, guardar</param>
    void OpenSavedGame(string caso)
    {
        switch (caso)
        {
            case "leer":
                GameServices.SavedGames.OpenWithAutomaticConflictResolution("SaveMonedas_01", OpenSavedGameCallback);
                break;
            case "guardar":
                GameServices.SavedGames.OpenWithAutomaticConflictResolution("SaveMonedas_01",GuardarMonedasCloud);
                break;
            default:
                Debug.Log("Error no se encontro" + caso + " como comando para abrir slot guardado");
                break;
        }
        // Open a saved game named "My_Saved_Game" and resolve conflicts automatically if any.
        

    }

    // Open saved game callback
    void OpenSavedGameCallback(SavedGame savedGame, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Se recupero juego guardado, leyendo...!");
            juegoSalvado = savedGame;        // keep a reference for later operations   
            ReadSavedGame(juegoSalvado);
        }
        else
        {
            Debug.Log("Error al carga salvado de juego: " + error);
        }
    }
    

    // Retrieves the binary data associated with the specified saved game
    void ReadSavedGame(SavedGame savedGame)
    {
        if (savedGame.IsOpen)
        {
            // The saved game is open and ready for reading
            GameServices.SavedGames.ReadSavedGameData
               (
                savedGame,
                (SavedGame game, byte[] data, string error) =>
                {
                    //si error esta vacio o no existe o sea todo fue correcto
                    if (string.IsNullOrEmpty(error))
                    {
                        Debug.Log("Saved game data has been retrieved successfully!");
                        // Here you can process the data as you wish.
                        if (data.Length > 0)
                        {
                            // Data processing
                            //Debug.Log("Cloud save-monedas:" + data);

                            Score_Save juegoRecuperado = ByteArra_Deserealizar(data);
                            MonedasTotales = juegoRecuperado.monedas;
                        }
                        else
                        {
                            Debug.Log("The saved game has no data!");
                        }

                    }
                    else
                    {
                        Debug.Log("Reading saved game data failed with error: " + error);
                    }
                }


            );
        }
        else
        {
            // The saved game is not open. You can optionally open it here and repeat the process.
            Debug.Log("You must open the saved game before reading its data.");
        }
    }

    //TODO-BUG: hay un problema en el byte pues este n puede guardar mas de 255, entonces necesitamos ver como podemos guardar una estructura mas larga

    //SERIALIZAR Y DESERIALIZAR

    /// <summary>
    /// Callback despues de abrir el juego guardado en nube si se encuentra el archivo,
    /// Despues llama a EscribirJuegoSalvado, pasandole el juegosalvado encontrado
    /// </summary>
    /// <param name="savedGame">objeto tipo SavedGame</param>
    /// <param name="error">objeto tipo string que recibe errores</param>
    void GuardarMonedasCloud(SavedGame savedGame, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Juego Salvado preparado para guardar monedas...");
            juegoSalvado = savedGame;// keep a reference for later operations   
            
           
            //Aqui declaramo un objeto , al crearlo este se llena automaticamente con los datos de la partida
            Score_Save saveJuego = NuevoSavedGame();

            byte[] datos = SavedGameDataToByteArray(saveJuego);
        

            EscribirJuegoSalvado(juegoSalvado,datos);
        }
        else
        {
            Debug.Log("Error al carga slot salvado de juego: " + error);
            Master_Level._masterBrinco.Reiniciar_Callback();

        }

    }

    /// <summary>
    /// Escribe los datos a guardar
    /// </summary>
    /// <param name="savedGame">la instancia tipo SavedGame</param>
    /// <param name="data">los datos ya serializados a byteArray</param>
    void EscribirJuegoSalvado(SavedGame savedGame, byte[] data)
    {
        if (savedGame.IsOpen)
        {
            // The saved game is open and ready for writing
            GameServices.SavedGames.WriteSavedGameData(
                savedGame,
                data,
                (SavedGame updatedSavedGame, string error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        Debug.Log($"Se guardaron {MonedasTotales} monedas en la nube exitosamente");
                        Master_Level._masterBrinco.Reiniciar_Callback();

                    }
                    else
                    {
                        Debug.Log("NO se pudieron guardar las monedas en la nube" + error);
                        Master_Level._masterBrinco.Reiniciar_Callback();

                    }
                }

            );
        }
        else
        {
            // The saved game is not open. You can optionally open it here and repeat the process.
            Debug.Log("You must open the saved game before writing to it.");
            Master_Level._masterBrinco.Reiniciar_Callback();

        }


      
    }

    /// <summary>
    /// Convierte en Json y seraliza un objeto de clase Score_Save
    /// </summary>
    /// <param name="dataObj"> variable de tipo Score_Save</param>
    /// <returns></returns>
    byte[] SavedGameDataToByteArray(Score_Save dataObj)
    {
        if (dataObj != null)
        {
            // Convert to json string
            string jsonStr = JsonUtility.ToJson(dataObj);

            // Json string to byte[]
            return System.Text.Encoding.UTF8.GetBytes(jsonStr);
        }

        return null;
    }

    /// <summary>
    /// Deserealiza y convierte los datos a tipo Score_Save
    /// </summary>
    /// <param name="data">byte array que hay que pasar</param>
    /// <returns></returns>
    private Score_Save ByteArra_Deserealizar(byte[] data)
    {
        if(data != null)
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);

            Score_Save saveRecuperado = JsonUtility.FromJson<Score_Save>(jsonStr);

            return saveRecuperado;
        }
        return null;
    }
    /// <summary>
    /// Funcion que regresa un tipo Score_Save ya con los datos ingresados;
    /// </summary>
    /// <returns></returns>
    private Score_Save NuevoSavedGame()
    {

        Score_Save save = new Score_Save();

        save.monedas = MonedasTotales;
        save.score = scoreRonda > highScoreUsuario ? scoreRonda : highScoreUsuario;


        return save;

    }
    #endregion
}
