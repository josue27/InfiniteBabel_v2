﻿using System.Collections;
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
                monedasPartidas_txt.text = monedasTotales.ToString("0000");
            }
        }

        public int MonedasPartida
        {
            get => monedasPartida;
            private set
            {
                monedasPartida = value;
                //monedasPartidas_txt.text = MonedasPartida.ToString("0000");
                monedasTotales_txt.text = monedasTotales.ToString("0000");

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

            nuevoHighScore_letrero.SetActive(false);

            //DEPRECATED(07/01/2021):la clase SaveScore se encarga ahora de cargar y salvar el juego
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
           // CargarScores();

        }

        public void CargarScores()
        {

            CargarScoreUsuario();
           // CargarMonedas();
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

                    Save_Control.instancia.SubirScoreGooglePlay(ScoreRonda);
                    Debug.Log("Highscore :" + highScoreUsuario + " superado guardano nuevo: " + ScoreRonda);
                    HighscoreUsuario = ScoreRonda;
                    nuevoHighScore_letrero.SetActive(true);
                }
                DesbloquearLogro();

            }
            else
            {//debe significar que no habia score y debe ser su primer juego

                ///Desbloqueamos logro de primera vez
                DesbloquearLogro();

                Save_Control.instancia.SubirScoreGooglePlay(ScoreRonda);
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
            }
            else
            {
                debug_text.text = "Fallo al mostar Leaderborad";
            }
        }




        public void DesbloquearLogro()
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
        }

        /// <summary>
        /// Inicia el comando para cargar el score en la nube y buscar cuantas monedas tiene el jugador
        /// 
        /// </summary>
        public void CargarMonedas()
        {

            try
            {
                MonedasTotales = Save_Control.instancia.JuegoSalvadoNube.monedas;
            }catch(Exception e)
            {
                Debug.Log("Error cargando monedas: "+e);
            }
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
            
        }
        public void RestarMonedas(int cantidad)
        {
            MonedasTotales -= cantidad;
        }

        #endregion
      
        

        /// <summary>
        /// Obtienes el score final haciendo una comparacion entre el score ronda y el HighScore
        /// </summary>
        /// <returns></returns>
        public int ScoreFinal()
        {
            return ScoreRonda > HighscoreUsuario ? ScoreRonda: HighscoreUsuario;
        }
    

        void Reinicio()
        {
            ScoreRonda = 0;
            scoreFinalRonda_text.text = ScoreRonda.ToString();
            nuevoHighScore_letrero.SetActive(false);

            AbrirPanelScore("mostrar");
        }

    }


}