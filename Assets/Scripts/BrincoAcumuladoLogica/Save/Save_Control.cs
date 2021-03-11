using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using EasyMobile;
using BayatGames.SaveGameFree;
using Brinco;
    public class Save_Control : MonoBehaviour
    {
        public static Save_Control instancia;
        
        //Juego Nube
        private SavedGame juegoNube;//este nos lo pide EasyMobile
        private Saved_Data juegoSalvadoNube;//este lo usamos para referencia local
        //Juego Local
        Saved_Data juegoSalvadoLocal;
        public static string juegoSalvadoLocal_ID = "SaveLocal";

        private string nombreSlotPersonajeUsado = "nombrePersonaje";
        //HighScore
        private int highScoreUsuario;
        public int HighScoreUsuario { get => highScoreUsuario;  private set => highScoreUsuario = value; }
        public Saved_Data JuegoSalvadoNube { get => juegoSalvadoNube; set => juegoSalvadoNube = value; }

        [SerializeField] int reinicios = 0;

        void Start()
        {
            instancia = this;

           // CargarJuegoLocal();
            CargarScoreUsuario();
            CargarSavedGame();
            CargarPersonajeGuardado();


            Eventos_Dispatcher.eventos.GuardarPersonaje += GuardarPersonajeSeleccionado;

            Eventos_Dispatcher.Reinicio += Reinicio;

        }

        private void CargarJuegoLocal()
        {
            juegoSalvadoLocal = SaveGame.Load<Saved_Data>(juegoSalvadoLocal_ID);
        }
        private void UsuarioLogeoExitoso()
        {
            CargarScoreUsuario();
            CargarSavedGame();
            GameServices.UserLoginSucceeded -= UsuarioLogeoExitoso;

        }


        #region ScoreJugador
        public void CargarScoreUsuario()
        {
            if(GameServices.IsInitialized())
            {
                GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_Obstaculos, OnLocalUserScoreLoaded);

            }else if(!GameServices.IsInitialized())
            {
                GameServices.Init();
                GameServices.UserLoginSucceeded += UsuarioLogeoExitoso;
            }


        }
        
        void OnLocalUserScoreLoaded(string leaderboardname, IScore scoreCargado)
        {
            if (scoreCargado != null)
            {
                //debug_text.text = $"Score Google Play:{scoreCargado.value}";
                Debug.Log($"Score Google Play:{scoreCargado.value}");
                HighScoreUsuario = int.Parse(scoreCargado.formattedValue);
                Score_Control.instancia.HighscoreUsuario = HighScoreUsuario;

            }
            else
            {
                Debug.Log($"Problema cargando el score local de GooglePlay cargando score Local o no tienes ningun score guardado");
                //if (juegoSalvadoLocal != null)
                //    HighscoreUsuario = juegoSalvadoLocal.score;
                NativeUI.Alert("Error connection", "We couldn´t connect with the cloud");
            //debug_text.text = $"Score Local cargado{HighscoreUsuario}";
                Score_Control.instancia.HighscoreUsuario = HighScoreUsuario;

                GameServices.Init();

                //if (Master_Level._masterBrinco.estadoJuego != EstadoJuego.jugando)
                //    CargarScoreUsuario();
            }

        }

        /// <summary>
        /// Maneja la subida de un HighScore logrado, llamado por CompararScore(),
        /// tambien salva el score localmente por si las dudas
        /// Se supones que debe subirlo a GameCenter o GooglePlay conforme corresponda
        /// </summary>
        /// <param name="nuevoScore"></param>
        public void SubirScoreGooglePlay(int nuevoScore)
        {

            //Salvamos primer el score localmente por si no hay internet
            // SaveGame.Save<int>(nombreSlotHighscore, nuevoScore);

            //GuardarJuego_Local();
            if (GameServices.IsInitialized())
            {
                //GameServices.ReportScore(nuevoScore, EM_GPGSIds.leaderboard_the_best_runner, (bool exito) => {
                //   Debug.Log("Se subio el score exitosamente");
                //});

                GameServices.ReportScore(nuevoScore, EM_GameServicesConstants.Leaderboard_Obstaculos, (bool exito) => {
                    Debug.Log("Se subio el score exitosamente");
                });
            }
            else
            {
#if UNITY_ANDROID
                GameServices.Init();    // start a new initialization process

#elif UNITY_IOS
                Debug.Log("Cannot show  Upload score");
                NativeUI.Alert("Error GS","Couldn´t save score");
#endif
            }


        }
        

        public void SubirScoreCafe(int nuevoScoreCafe)
        {
            if (GameServices.IsInitialized())
            {
                //GameServices.ReportScore(nuevoScore, EM_GPGSIds.leaderboard_the_best_runner, (bool exito) => {
                //   Debug.Log("Se subio el score exitosamente");
                //});

                GameServices.ReportScore(nuevoScoreCafe, EM_GameServicesConstants.Leaderboard_Cafes, (bool exito) => {
                    Debug.Log("Se subio el score exitosamente");
                });
            }
            else
            {
    #if UNITY_ANDROID
                GameServices.Init();    // start a new initialization process

    #elif UNITY_IOS
                Debug.Log("Cannot show  Upload score");
    #endif
            }
        }

        /// <summary>
        /// Asigna el nuevo Highscore a la variable local para su futuro guardado asi como
        /// repota la funcion para subir el score a google play
        /// </summary>
        /// <param name="nuevoScore"></param>
        public void ReportarNuevoHighScore(int nuevoScore)
        {
            //Asignar a save control como actual para que cuando demos reset game y este script se lo pida a Save_Control lo obtenga actualizado            HighScoreUsuario = nuevoScore;
            Debug.Log("Save_Control: Nuevo score reportado localmente: " + nuevoScore);
            HighScoreUsuario = nuevoScore;
            SubirScoreGooglePlay(nuevoScore);
        }

        #endregion


        #region CargarJuego
        void CargarSavedGame()
        {
            GameServices.SavedGames.OpenWithAutomaticConflictResolution("SaveMonedas_01", OpenSavedGameCallback);
        }

        void OpenSavedGameCallback(SavedGame savedGame, string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                Debug.Log("Se recupero juego guardado Nube, leyendo...!");
                juegoNube = savedGame;        // keep a reference for later operations   
                ReadSavedGame(juegoNube);
            }
            else
            {
                Debug.Log("Error al cargar juego guardado Nube: " + error);
                //Llamamos al evento para que pueda continuar el juego
                Eventos_Dispatcher.eventos.JuegoCargado_Call();

            }
        }

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
                                //Guardamos los datos descargados para futura comparacion
                                juegoSalvadoNube = ConvertidorData.ByteArra_Deserealizar(data);
                                
                                Score_Control.instancia.MonedasTotales = JuegoSalvadoNube.monedas;


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

            }

            Eventos_Dispatcher.eventos.JuegoCargado_Call();

        }

        #endregion


        #region GuardarJuego
        public void GuardarJuego()
        {
            GameServices.SavedGames.OpenWithAutomaticConflictResolution("SaveMonedas_01", GuardarPartidaEnCloud);
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

                juegoNube = savedGame;



                byte[] datos = ConvertidorData.SavedGameDataToByteArray(saveJuego);


                EscribirJuegoSalvado(juegoNube, datos);

                //GuardarJuego_Local();
            }
            else
            {

               // GuardarJuego_Local();

                Debug.Log("Error al carga slot salvado de juego: " + error);
                //Master_Level._masterBrinco.Reiniciar_Callback();
            }

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
                            //Debug.Log($"Se guardo juego en Nube con exito[{MonedasTotales} monedas] ");



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

            Debug.Log("Guardando " + Score_Control.instancia.MonedasTotales + " monedas");
            save.monedas = Score_Control.instancia.MonedasTotales; 
            save.score = HighScoreUsuario;
            //DEPRECATED: No deberiamos tener que pedirselo ya que Score_Control lo actualizara de haber un nuevo highscore
            //save.score = Score_Control.instancia.ScoreFinal();

            save.removeAds = GetComponent<Ad_Control>().removerAdIntermedio;

         

            List<PersonajeSalvado> personajesAGuardar = new List<PersonajeSalvado>();

            foreach (PersonajesEnJuego personaje in SeleccionPersonaje._seleccionPersonaje.personajes)
            {
                PersonajeSalvado _personaje = new PersonajeSalvado
                {
                    comprado = personaje.comprado,
                    nombre = personaje.personaje.nombre
                };
                personajesAGuardar.Add(_personaje);
               // Debug.Log($"Personaje:{personaje.personaje.nombre} comprado:{personaje.comprado} y agregado para guardar ");
            }

            save.personajes = personajesAGuardar;

            return save;

        }

        #endregion



        #region PersonajeSeleccionadoLocal

        /// <summary>
        /// Carga de el numero de personaje que el jugdor uso la ultima vez, es 
        /// local porque no es relevante que se guarde en la nube
        /// </summary>
        public void CargarPersonajeGuardado()
        {
            if (SaveGame.Exists(nombreSlotPersonajeUsado))
            {
                string p = SaveGame.Load<string>(nombreSlotPersonajeUsado);
                SeleccionPersonaje._seleccionPersonaje.SetPersonaje(p);
                //Debug.Log("Se mando cambio de personaje guardado");
            }
            else
            {
                Debug.Log("No se encontro Skin Personaje guardado");
            }

        }

        /// <summary>
        /// Guarda el personaje que el Usuario selecciono de manera local,llakado por Event
        /// </summary>
        /// <param name="_nombrePersonaje"></param>
        private void GuardarPersonajeSeleccionado(string _nombrePersonaje)
        {
            SaveGame.Save(nombreSlotPersonajeUsado, _nombrePersonaje);
            Debug.Log("Personaje: " + _nombrePersonaje + " guardado para siguiente partida");
        }

        #endregion


        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                GuardarJuego();

            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GuardarJuego();

            }
        }
      

        void Reinicio()
        {

            //No deberiamos pedir esto ya que se supone ya esta cargado, al volverlo a pedir se sobreescribe lo que lleva de la partida
            //CargarScoreUsuario();
            // CargarSavedGame();
            if (GameServices.IsInitialized() == false)
            {
                GameServices.Init();
            }
            // HighScoreUsuario = Score_Control.instancia.HighscoreUsuario;
            reinicios++;
            if(reinicios == 3)
            {
                Debug.Log("Tercer reinicio guardado automatico...");
                GuardarJuego();
                reinicios = 0;
            }
            
        }
    }





