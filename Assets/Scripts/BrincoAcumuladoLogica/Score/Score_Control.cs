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
using System.IO;

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


    private Saved_Data datosDescargados;

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

    #region Save Settings
    private SavedGame juegoSalvadoNube;

    private string saveFilePath;

    private Saved_Data juegoSalvadoLocal;
    public static string juegoSalvadoLocal_ID = "SaveLocal";
    #endregion

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

        if (SaveGame.Exists(juegoSalvadoLocal_ID))
        {
            CargarJuegoLocal();
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
        // SaveGame.Save<int>(nombreSlotHighscore, nuevoScore);

        GuardarJuego_Local();
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
  
   void CargarJuegoLocal()
    {
        juegoSalvadoLocal = SaveGame.Load<Saved_Data>(juegoSalvadoLocal_ID);
    }

    public void CargarScoreUsuario()
    {
        // if(SaveGame.Exists(nombreSlotHighscore))
        //      highscoreLocal = SaveGame.Load<int>(nombreSlotHighscore);


        GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_Obstaculos, OnLocalUserScoreLoaded);
    }

    /// <summary>
    /// Carga el Score en la Nube del usuario de acuerdo al tablero de calificaciones y si no puede conectarse 
    /// carga el score local
    /// </summary>
    /// <param name="leaderboardname"></param>
    /// <param name="scoreCargado"></param>
    void OnLocalUserScoreLoaded(string leaderboardname, IScore scoreCargado)
    {
        if (scoreCargado != null)
        {
            debug_text.text = $"Score Google Play:{scoreCargado.value}";
            Debug.Log($"Score Google Play:{scoreCargado.value}");
            HighscoreUsuario = int.Parse(scoreCargado.formattedValue);

            //Verificamos si el Score de la Nube es diferente al Local
          
            if (juegoSalvadoLocal != null)
            {
               
                if (juegoSalvadoLocal.score > HighscoreUsuario)
                {
                    HighscoreUsuario = juegoSalvadoLocal.score;

                }else
                {
                    //Si el guardado local no supera al de GoogleServices entonces asignamos el de la nube al local
                    //quiere decir que hubo un conflicto en el guardado
                    //SaveGame.Save<int>(nombreSlotHighscore,HighscoreUsuario);
                    GuardarJuego_Local();
                }
            }


            
        }
        else
        {
            debug_text.text = $"Problema cargando el score local de GooglePlay cargando score Local";
            if (juegoSalvadoLocal != null)
                HighscoreUsuario = juegoSalvadoLocal.score;


            debug_text.text = $"Score Local cargado{HighscoreUsuario}";
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


    #region Skin Personaje Seleccionado 
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
            //Master_Level._masterBrinco.Reiniciar_Callback();
        }
    }

    public void Guardar_MonedasYPersonajes()
    {
        if(RuntimeManager.IsInitialized())
        {
            OpenSavedGame("guardar");

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
                GameServices.SavedGames.OpenWithAutomaticConflictResolution("SaveMonedas_01",GuardarPartidaEnCloud);
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
            Debug.Log("Se recupero juego guardado Nube, leyendo...!");
            juegoSalvadoNube = savedGame;        // keep a reference for later operations   
            ReadSavedGame(juegoSalvadoNube);
        }
        else
        {
            Debug.Log("Error al cargar juego guardado Nube: " + error);

            if (SaveGame.Exists(juegoSalvadoLocal_ID))
            {
                juegoSalvadoLocal = SaveGame.Load<Saved_Data>(juegoSalvadoLocal_ID);
                Debug.Log("Se cargo juego local");
                if (juegoSalvadoLocal != null)
                {
                    MonedasTotales = juegoSalvadoLocal.monedas;
                    GetComponent<SeleccionPersonaje>().VerificarPersonajesComprados(juegoSalvadoLocal.personajes);

                }
            }
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
                        Debug.Log("Juego Salvado de Nube leido con exito!");
                        // Here you can process the data as you wish.
                        if (data.Length > 0)
                        {
                            // Data processing
                            //Debug.Log("Cloud save-monedas:" + data);

                            Saved_Data juegoRecuperado = ByteArra_Deserealizar(data);

                            //Guardamos los datos descargados para futura comparacion
                            datosDescargados = juegoRecuperado;
                            //Resolver conflicto de monedas
                            if (juegoSalvadoLocal != null)
                            {
                                if(juegoSalvadoLocal.monedas > juegoRecuperado.monedas)
                                {
                                    MonedasTotales = juegoSalvadoLocal.monedas;
                                    Debug.Log($"Conflicto Cloud y Local, las monedas en local({juegoSalvadoLocal.monedas}) es > a las de nube({juegoRecuperado.monedas}) se tomara en cuenta monedas Locales");
                                }
                                else
                                {
                                    MonedasTotales = juegoRecuperado.monedas;
                                    Debug.Log($"Conflicto Cloud y Local, las monedas en local({juegoSalvadoLocal.monedas}) es < a las de nube({juegoRecuperado.monedas}) se tomaran en cuenta monedas Nube");
                                }
                            }
                            else
                            {
                                MonedasTotales = juegoRecuperado.monedas;
                            }
                            //Enviar monitos comprados a SeleccionPersonaje
                            GetComponent<SeleccionPersonaje>().VerificarPersonajesComprados(juegoRecuperado.personajes);
                        }
                        else
                        {
                            Debug.Log("EL  juego guardado Nube se recupero pero esta vacio!");
                        }

                    }
                    else
                    {
                        Debug.Log("Error al leer el archivo guardado de NUBE: " + error);
                    }
                }


            );
        }
        else
        {
            // The saved game is not open. You can optionally open it here and repeat the process.
            Debug.Log("No se pudo abrir el archivo");
            //Cargar con data Local
            if(juegoSalvadoLocal != null)
            {
                MonedasTotales = juegoSalvadoLocal.monedas;
                GetComponent<SeleccionPersonaje>().VerificarPersonajesComprados(juegoSalvadoLocal.personajes);

            }

        }
    }

  

    /// <summary>
    /// Callback despues de abrir el juego guardado en nube si se encuentra el archivo,
    /// Despues llama a EscribirJuegoSalvado, pasandole el juegosalvado encontrado
    /// </summary>
    /// <param name="savedGame">objeto tipo SavedGame</param>
    /// <param name="error">objeto tipo string que recibe errores</param>
    void GuardarPartidaEnCloud(SavedGame savedGame, string error)
    {
        //Aqui declaramo un objeto , al crearlo este se llena automaticamente con los datos de la partida
        Saved_Data saveJuego = NuevoSavedGame();

        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Juego Salvado preparado para guardar  en la nube...");

            juegoSalvadoNube = savedGame;  
            


            byte[] datos = SavedGameDataToByteArray(saveJuego);
        

            EscribirJuegoSalvado(juegoSalvadoNube,datos);
            
            GuardarJuego_Local();
        }
        else
        {

            GuardarJuego_Local();

            Debug.Log("Error al carga slot salvado de juego: " + error);
            //Master_Level._masterBrinco.Reiniciar_Callback();
        }

    }
    

    /// <summary>
    /// Llamado cuando para guardar el juego en un archivo local
    /// </summary>
    void GuardarJuego_Local()
    {
        
        Saved_Data saveJuego = NuevoSavedGame();


        SaveGame.Save<Saved_Data>(juegoSalvadoLocal_ID, saveJuego);
        Debug.Log("Se guardo juego local exitosamente");

        //string saveJuego_json = JsonUtility.ToJson(saveJuego);

        //if (FileManager.WriteToFile(saveLocal_ID, saveJuego.ToJson()))
        //{
        //    Debug.Log("Se guardo juego local exitosamente");
        //}

    }
   
    
    /// <summary>
    /// Escribe los datos a guardar
    /// </summary>
    /// <param name="savedGame">la instancia tipo SavedGame</param>
    /// <param name="data">los datos ya serializados a byteArray</param>
    void EscribirJuegoSalvado(SavedGame savedGame, byte[] data)
    {
        if (savedGame.IsOpen && GameServices.IsInitialized())
        {
            // The saved game is open and ready for writing
            GameServices.SavedGames.WriteSavedGameData(
                savedGame,
                data,
                (SavedGame updatedSavedGame, string error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        Debug.Log($"Se guardo juego en Nube con exito[{MonedasTotales} monedas] ");
                        
                       

                    }
                    else
                    {
                        Debug.Log("NO se pudieron guardar las monedas en la nube" + error);
                        

                    }
                }

            );

            
        }
        else
        {
            // The saved game is not open. You can optionally open it here and repeat the process.
            Debug.Log("El juego no se pudo abrir y/o no esta inicializado el servicio de nube.");
            

        }


       // Master_Level._masterBrinco.Reiniciar_Callback();

    }

    /// <summary>
    /// Convierte a Json y luego  seraliza a bytes un objeto de clase Score_Save
    /// </summary>
    /// <param name="dataObj"> variable de tipo Score_Save</param>
    /// <returns></returns>
  
    
    
    byte[] SavedGameDataToByteArray(Saved_Data dataObj)
    {
        if (dataObj != null)
        {
            // Convert to json string
            string jsonStr = JsonUtility.ToJson(dataObj);
            Debug.Log(jsonStr);
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
    private Saved_Data ByteArra_Deserealizar(byte[] data)
    {
        if(data != null)
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log("Informaicon deserealizada de nube" + jsonStr);
            Saved_Data saveRecuperado = JsonUtility.FromJson<Saved_Data>(jsonStr);

            return saveRecuperado;
        }
        return null;
    }
    
    
    /// <summary>
    /// Funcion que regresa un tipo Score_Save ya con los datos ingresados;
    /// </summary>
    /// <returns></returns>
    private Saved_Data NuevoSavedGame()
    {

        Saved_Data save = new Saved_Data();

        save.monedas = MonedasTotales;
        save.score = scoreRonda > HighscoreUsuario ? scoreRonda : HighscoreUsuario;


        //esto en teoria deberia evitar el error de que suba un personaje como no comprado que en la nube
        //si este comprado
        GetComponent<SeleccionPersonaje>().VerificarPersonajesComprados(datosDescargados.personajes);

        List<PersonajeSalvado> personajesAGuardar = new List<PersonajeSalvado>();

        foreach(PersonajesEnJuego personaje in SeleccionPersonaje._seleccionPersonaje.personajes)
        {
            PersonajeSalvado _personaje = new PersonajeSalvado
            {
                comprado = personaje.comprado,
                nombre = personaje.personaje.nombre
            };
            personajesAGuardar.Add(_personaje);
            Debug.Log($"Personaje:{personaje.personaje.nombre} comprado:{personaje.comprado} y agregado para guardar ");
        }

        save.personajes = personajesAGuardar;

        return save;

    }
    #endregion

    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            Guardar_MonedasYPersonajes();

        }
    }
    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            Guardar_MonedasYPersonajes();

        }
    }
}
/*
 opcion 1 subimos la que tiene mas monedas y/o score
 opcion 2 obtener las 2 y comparar
 
 */