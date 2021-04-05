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

namespace Brinco
{
    public class Score_Control : MonoBehaviour
    {
        public static Score_Control instancia;

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
        [SerializeField]
        private int scoreRonda;

        public int ScoreRonda { get => scoreRonda; 
            set { 
                scoreRonda = value;
                scoreFinalRonda_text.text = scoreRonda.ToString();
                scoreRonda_text.text = ScoreRonda.ToString();
            }
        }

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
            set
            {
                monedasTotales = value;
                monedasTotales_txt.text = monedasTotales.ToString("0000");
                monedasPartida = monedasTotales;
                //monedasPartidas_txt.text = monedasTotales.ToString("0000");
            }
        }

        public int MonedasPartida
        {
            get => monedasPartida;
            private set
            {
                monedasPartida = value;
                monedasPartidas_txt.text = monedasPartida.ToString("0000");
                //monedasTotales_txt.text = monedasTotales.ToString("0000");

            }
        }

        [Header("UIX Juego")]
        public Animator monitorAnimator;

        public TMP_Text highScoreUsuario_text;
        public TMP_Text monedasUsuario_text;
        public TMP_Text scoreBest_text;
        public TMP_Text scoreFinalRonda_text;
        public TMP_Text scoreRonda_text;

        public TMP_Text debug_text;

        public GameObject nuevoHighScore_letrero;
        public GameObject monitorHighScore_inicial;
        public GameObject highScore_tabla;
        public GameObject[] scoreSlot;

        #region Save Settings
        private SavedGame juegoSalvadoNube;

        private string saveFilePath;

        private Saved_Data juegoSalvadoLocal;
        public static string juegoSalvadoLocal_ID = "SaveLocal";
        #endregion

        public int HighscoreUsuario
        {
            get => highScoreUsuario;
            set
            {
                highScoreUsuario = value;
                highScoreUsuario_text.text = highScoreUsuario.ToString();
                scoreBest_text.text = highScoreUsuario.ToString();
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
            Eventos_Dispatcher.Reinicio += Reinicio;
            panelScore.transform.position = panelScore_posCerrada.position;
            Eventos_Dispatcher.eventos.Revivir += Revivir;

            nuevoHighScore_letrero.SetActive(false);

          

        }


        /// <summary>
        /// Llamado por el momento para abrir el panel score cuando ya se tiene cargado el score y monedas
        /// </summary>
        public void CargarScores()
        {

            //CargarScoreUsuario();
            //CargarMonedas();
            if(Master_Level._masterBrinco.estadoJuego == EstadoJuego.inicio)
                AbrirPanelScore("mostrar");

        }

        void OnDestroy()
        {
            Eventos_Dispatcher.CruceObstaculo -= ObstaculoCruzado;
            Eventos_Dispatcher.MonedaTomada -= MonedaTomada;
            Eventos_Dispatcher.eventos.JugadorPerdio -= FinJuego;
            Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
            Eventos_Dispatcher.JuegoCargado -= CargarScores;
            Eventos_Dispatcher.eventos.Revivir -= Revivir;

        }

        private void InicioJuego()
        {

            AbrirPanelScore("ocultar");
            ScoreRonda = 0;
            
            MonedasPartida = MonedasTotales;
        }

        /// <summary>
        /// Activado por EventDispatcher cuando el jugador pierde
        /// </summary>
        private void FinJuego()
        {
            CompararScore();

            scoreFinalRonda_text.text = ScoreRonda.ToString();

            //Save_Control.instancia.SubirScoreCafe(MonedasTotales);
            //IngresarMonedasPartida();
        }



        private void CompararScore()
        {
            //en teoria highscoreLocal ya deberia estar cargado y solamente checamos que sea mas de 0 para que
            //valga la pena
            if (highScoreUsuario > 0)
            {
                if (ScoreRonda > highScoreUsuario)//Si se llega a superar el score guardado
                {
                    Debug.Log("Highscore :" + highScoreUsuario + " superado guardano nuevo: " + ScoreRonda);

                    
                    Save_Control.instancia.ReportarNuevoHighScore(ScoreRonda);
                    //Asignar como highscore actual
                    //HighscoreUsuario = ScoreRonda;
                   

                    nuevoHighScore_letrero.SetActive(true);

                }
                DesbloquearLogroPrimeraCarrera();

            }
            else
            {//debe significar que no habia score y debe ser su primer juego

                ///Desbloqueamos logro de primera vez
                DesbloquearLogroPrimeraCarrera();

                //Save_Control.instancia.SubirScoreGooglePlay(ScoreRonda);
                Save_Control.instancia.ReportarNuevoHighScore(ScoreRonda);
               
                nuevoHighScore_letrero.SetActive(true);

                Debug.Log("Primer highscore salvado:" + ScoreRonda);
            }
            //para que se actualice el Highscore durante la partida
            // CargarScoreUsuario();

            if (ScoreRonda >= 5)
            {
               
                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_Rookie_moves);
            }
            else if (ScoreRonda >= 10)
            {
               
                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_Looking_Promotion);
            }
            else if (ScoreRonda >= 30)
            {
                
                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_Looking_Promotion);
            }
        }


        [Button("Abrir Cerrar Panel Score")]
        public void AbrirPanelScore()
        {
            //Abirr monitor de score inicial
            LeanTween.moveLocalY(monitorHighScore_inicial, 0f, 0.5f).setEaseInOutSine();       //highScoreLocal_text.SetText(highscoreLocal.ToString());

            

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

       

        /// <summary>
        /// Llamado por UI por el usuario, muestra el tablero nativo de Score de Google
        /// </summary>
        public void MostrarGoogleLeaderBoard()
        {


            if (GameServices.IsInitialized())
            {
                GameServices.ShowLeaderboardUI();
            }
            else
            {
#if UNITY_ANDROID
                GameServices.Init();    // start a new initialization process
#elif UNITY_IOS
    Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
            }
        }


        public void DesbloquearLogroPrimeraCarrera()
        {
            Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_FirstRun);
        }



        /// <summary>
        /// Lleva la cuenta de los obstaculos cruzados, ojo Master_Level.cs tambien lleva la cuenta pero
        /// para aumentar la dificultad, este scoreRonda es el que se guarda y publica
        /// </summary>
        public void ObstaculoCruzado()
        {
            if (Master_Level._masterBrinco.estadoJuego != EstadoJuego.jugando)
                return;

            MonitorAnimar("obstaculo");
            ScoreRonda++;

        }

        /// <summary>
        /// Activa la animacion de sprite del monitor de score del juego
        /// </summary>
        /// <param name="trigger">moneda,obstaculo</param>
        public void MonitorAnimar(string trigger)
        {
            monitorAnimator.SetTrigger(trigger);
        }


        /// <summary>
        /// Busca en Save_Control el valor del HighScoreUsuario Actual
        /// </summary>
        public void CargarScoreUsuario()
        {
            // if(SaveGame.Exists(nombreSlotHighscore))
            //      highscoreLocal = SaveGame.Load<int>(nombreSlotHighscore);
            // AbrirPanelScore();

            HighscoreUsuario = Save_Control.instancia.HighScoreUsuario;


        }


        /// <summary>
        /// Funcion de debug para rellenar el tablero de Highscores
        /// </summary>
        [Button]
        void Rellenar()
        {
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
            //MonedasPartida++;
            MonedasTotales++;
            MonedasPartida = MonedasTotales;
        }

    
       
        public void SumarMonedas(int cantidad)
        {
            MonedasTotales += cantidad;
            MonedasPartida = MonedasTotales;
            LerpInt(MonedasPartida, MonedasTotales, 1.0f);

        }
        public void SumarMonedas(int cantidad,bool guardar)
        {
            SumarMonedas(cantidad);
            if(guardar)
                Save_Control.instancia.GuardarJuego();
        }
        public void RestarMonedas(int cantidad)
        {
            MonedasTotales -= cantidad;
            MonedasPartida = MonedasTotales;//ojo solo sirve para display el que se guarda sigue siendo MonedasTotales
            //MonedasPartida = MonedasTotales;
            LerpInt(MonedasPartida, MonedasTotales, 1.0f);

        }

        [EasyButtons.Button]
        void PruebaScore()
        {
            StartCoroutine(LerpInt(MonedasPartida, MonedasPartida + 10, 1));
        }

        IEnumerator LerpInt(float valorInit,float valorFinal,float duracion)
        {
            float tiempoPasado = 0;
            while (tiempoPasado < duracion)
            {
                MonedasPartida = (int)Mathf.Lerp(valorInit, valorFinal, tiempoPasado / duracion);
                tiempoPasado += Time.deltaTime;
                yield return null;
            }
            MonedasPartida = (int)valorFinal;
        }
        #endregion



        /// <summary>
        /// Obtienes el score final haciendo una comparacion entre el score ronda y el HighScore
        /// OJO:Por el momento no se utiliza
        /// </summary>
        /// <returns></returns>
        public int ScoreFinal()
        {
            return ScoreRonda > HighscoreUsuario ? ScoreRonda: HighscoreUsuario;
        }
    

        void Reinicio()
        {
            ScoreRonda = 0;  
            nuevoHighScore_letrero.SetActive(false);
            CargarScoreUsuario();
            AbrirPanelScore("mostrar");
        }

        void Revivir()
        {

        }

    }


}