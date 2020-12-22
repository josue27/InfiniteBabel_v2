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

    public int HighscoreUsuario {
        get => highScoreUsuario;
        set {
            highScoreUsuario = value;
            highScoreUsuario_text.text = highScoreUsuario.ToString();
        }
    }
    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }

        saveFilePath = Application.persistentDataPath + "/save.data";
    }
    void Start()
    {
        instancia = this;
        //CargarScoreGlobal();
        // AbrirPanelScore();
        Eventos_Dispatcher.CruceObstaculo += ObstaculoCruzado;
        Eventos_Dispatcher.MonedaTomada += MonedaTomada;
        Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
        Eventos_Dispatcher.JuegoCargado += CargarScores;
        panelScore.transform.position = panelScore_posCerrada.position;


        //if (SaveGame.Exists(juegoSalvadoLocal_ID))
        //{
        //   // CargarJuegoLocal();
        //    CargarScoreUsuario();
        //    CargarMonedas();
        //   // CargarPersonajeGuardado();
        //   // AbrirPanelScore();//Pasamos esta funcion a CargarScoreUsuario() para que muestre score despues de calcular


        //}
        //else
        //{
        //    Debug.Log("No hay scores, debe ser primeriso");
        //}

        // AbrirPanelScore("mostrar");
        CargarScores();

    }

    public void CargarScores()
    {
        AbrirPanelScore("mostrar");

        CargarScoreUsuario();
        CargarMonedas();

    }

    void OnDestroy()
    {
        Eventos_Dispatcher.CruceObstaculo -= ObstaculoCruzado;
        Eventos_Dispatcher.MonedaTomada -= MonedaTomada;
        Eventos_Dispatcher.eventos.JugadorPerdio -= FinJuego;
        Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
        Eventos_Dispatcher.JuegoCargado -= CargarScores;

    }

    private void InicioJuego()
    {

        AbrirPanelScore("ocultar");

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

                Save_Control.instancia.SubirScoreGooglePlay(scoreRonda);
                Debug.Log("Highscore :" + highScoreUsuario + " superado guardano nuevo: " + scoreRonda);
            }
            DesbloquearLogro();

        } else {//debe significar que no habia score y debe ser su primer juego

            ///Desbloqueamos logro de primera vez
            DesbloquearLogro();

            Save_Control.instancia.SubirScoreGooglePlay(scoreRonda);
            Debug.Log("Primer highscore salvado:" + scoreRonda);
        }
        //para que se actualice el Highscore durante la partida
        // CargarScoreUsuario();

        if (scoreRonda >= 5)
        {
            Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_rookie_moves);
        } else if (scoreRonda >= 10)
        {
            Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_looking_that_promotion);
        } else if (scoreRonda >= 30)
        {
            Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_senior_moves);
        }
    }


    [Button("Abrir Cerrar Panel Score")]
    public void AbrirPanelScore()
    {
        //Abirr monitor de score inicial
        LeanTween.moveLocalY(monitorHighScore_inicial, 0f, 0.5f).setEaseInOutSine();       //highScoreLocal_text.SetText(highscoreLocal.ToString());

        //No se esta usando este panel
        //if (!panelScoreAbierto)
        //{
        //    LeanTween.moveY(panelScore, panelScore_posAbierta.position.y, velocidadApertura).setEaseOutBounce();
        //    panelScoreAbierto = true;


        //}
        //else
        //{
        //    LeanTween.moveY(panelScore, panelScore_posCerrada.position.y, velocidadApertura).setEaseInBounce();
        //    panelScoreAbierto = false;

        //}

    }


    /// <summary>
    /// Abre o cierra el panel ocn el score mas alto del jugador
    /// </summary>
    /// <param name="estado">mostrar,ocultar</param>
    public void AbrirPanelScore(string estado)
    {
        if (estado == "mostrar")
        {
            LeanTween.moveLocalY(monitorHighScore_inicial, 150f, 0.5f).setEaseInOutSine();
        }
        else if (estado == "ocultar")
        {
            LeanTween.moveLocalY(monitorHighScore_inicial, 600f, 0.5f).setEaseInOutSine();

        }
        else
            Debug.Log("Error no se encontro el estado");
    }

    //public void AbrirPanelScore_Global(bool _abrir)
    //{

    //    highScore_tabla.gameObject.SetActive(_abrir);
    //  //  if (_abrir) CargarScoreGlobal();

    //}


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



    
    public void DesbloquearLogro()
    {

        Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_welcome_to_the_late_shift);


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
        // AbrirPanelScore();

        //DEPRECATED
        //GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_Obstaculos, OnLocalUserScoreLoaded);

        HighscoreUsuario = Save_Control.instancia.HighScoreUsuario;


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
        return;
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


        MonedasTotales = Save_Control.instancia.JuegoSalvadoNube.monedas;
        // OpenSavedGame("leer");
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
        int totalPartida = Mathf.Abs(MonedasPartida - MonedasTotales);
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

    #endregion
    public int ScoreFinal()
    {
        return scoreRonda > HighscoreUsuario ? scoreRonda : HighscoreUsuario;
    }
    public void GuardarMonedas()
    {

        //DEPRECATED
        //if (RuntimeManager.IsInitialized())
        //{
        //    OpenSavedGame("guardar");
        //}
        //else
        //{
        //    //Master_Level._masterBrinco.Reiniciar_Callback();
        //}
    }

    public void Guardar_MonedasYPersonajes()
    {
        if (RuntimeManager.IsInitialized())
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
                GameServices.SavedGames.OpenWithAutomaticConflictResolution("SaveMonedas_01", GuardarPartidaEnCloud);
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
            //INHABILITADO POR ERRORES
            //if (SaveGame.Exists(juegoSalvadoLocal_ID))
            //{
            //    juegoSalvadoLocal = SaveGame.Load<Saved_Data>(juegoSalvadoLocal_ID);
            //    Debug.Log("Se cargo juego local");
            //    if (juegoSalvadoLocal != null)
            //    {
            //        MonedasTotales = juegoSalvadoLocal.monedas;
            //        ///Inhabilitamos porque no sabemos si esto afecta al guardado en nube
            //        //GetComponent<SeleccionPersonaje>().VerificarPersonajesComprados(juegoSalvadoLocal.personajes);

            //    }
            //}
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

                            Saved_Data juegoRecuperado = ConvertidorData.ByteArra_Deserealizar(data);

                            //Guardamos los datos descargados para futura comparacion
                            datosDescargados = juegoRecuperado;
                            //INHABILITADO POR ERRORES
                            //Resolver conflicto de monedas

                            //if (juegoSalvadoLocal != null)
                            //{
                            //    if(juegoSalvadoLocal.monedas > juegoRecuperado.monedas)
                            //    {
                            //        MonedasTotales = juegoSalvadoLocal.monedas;
                            //        Debug.Log($"Conflicto Cloud y Local, las monedas en local({juegoSalvadoLocal.monedas}) es > a las de nube({juegoRecuperado.monedas}) se tomara en cuenta monedas Locales");
                            //    }
                            //    else
                            //    {
                            //        MonedasTotales = juegoRecuperado.monedas;
                            //        Debug.Log($"Conflicto Cloud y Local, las monedas en local({juegoSalvadoLocal.monedas}) es < a las de nube({juegoRecuperado.monedas}) se tomaran en cuenta monedas Nube");
                            //    }
                            //}
                            //else
                            //{
                            //    MonedasTotales = juegoRecuperado.monedas;
                            //}

                            MonedasTotales = juegoRecuperado.monedas;

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
            //INHABILITADO POR ERRORES
            //if(juegoSalvadoLocal != null)
            //{
            //    MonedasTotales = juegoSalvadoLocal.monedas;
            //  //  GetComponent<SeleccionPersonaje>().VerificarPersonajesComprados(juegoSalvadoLocal.personajes);
            // //   GetComponent<Ad_Control>().RemoverAds(juegoSalvadoLocal.removeAds);
            //}

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



            byte[] datos = ConvertidorData.SavedGameDataToByteArray(saveJuego);


            EscribirJuegoSalvado(juegoSalvadoNube, datos);

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
    /// Funcion que regresa un tipo Score_Save ya con los datos ingresados;
    /// </summary>
    /// <returns></returns>
    private Saved_Data NuevoSavedGame()
    {

        Saved_Data save = new Saved_Data();

        save.monedas = MonedasTotales;
        save.score = scoreRonda > HighscoreUsuario ? scoreRonda : HighscoreUsuario;

        save.removeAds = GetComponent<Ad_Control>().removerAdIntermedio;

        //esto en teoria deberia evitar el error de que suba un personaje como no comprado que en la nube
        //si este comprado
        if (datosDescargados != null)
            GetComponent<SeleccionPersonaje>().VerificarPersonajesComprados(datosDescargados.personajes);

        List<PersonajeSalvado> personajesAGuardar = new List<PersonajeSalvado>();

        foreach (PersonajesEnJuego personaje in SeleccionPersonaje._seleccionPersonaje.personajes)
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
    

}


