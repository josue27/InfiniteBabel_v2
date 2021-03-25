using System;
using System.Collections;
 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using EasyMobile;
using BayatGames.SaveGameFree;

//TODO: Desuscribir a todos cuando el juego pierda o ver como se hace
namespace Brinco
{

    public class Master_Level : MonoBehaviour
    {

        public static Master_Level _masterBrinco;

        // Start is called before the first frame update
        public Button boton_jugar;
        public GameObject panel_reinicio;



        public EstadoJuego estadoJuego;
        [Header("Ritmo Niveles")]
        public float velocidadObstaculosInicial = 1.0f;
        public float velocidadObstaculos;
        public float porcentajeAumento = 1.10f;

        //Despues de cuantos obstaculos pasamos a la siguiente velocidad
        //Debera ser  un aleatorio entre un minimo y maximo
        public int pasarASigVelocidadEn;
        public int pasarSigVelocidad_Min = 10;
        public int pasarSigVelocidad_Max = 12;

        //Debe ir en descalada para que sean mas pronta las velocidades
        public float rateSpawnMin, rateSpawnMax;
        [SerializeField]
        private int enNivel = 0;
        public Niveles[] nivelesDificultad;

        [Header("Score")]
        public int obstaculosCruzados;
        

        [Header("Monedas")]
        public int monedasTomadas;
        public int costoCafeRevivir = 30;

        [Space(10)]
        [Header("BandaTransportadora")]
        public Animator[] bandasTransportadoras;

        [Space(10)]
        [Header("PantallaReinicio")]
        public GameObject pantallNegro;


        [Space(10)]
        [Header("Tutorial")]
        public bool tutorialCompletado;
        public GameObject panelTutorial;//params:tutorialRegreso;

        public BrincoAcumulado jugador;

        [SerializeField]
        private GameObject versionText;


        [SerializeField] private float tiempoReinicio = 4;
        [SerializeField] private float reiniciandoEn = 3;
        [SerializeField] private bool countDownReinicio;
        [SerializeField] private GameObject panelCountDown;
        [SerializeField] private TMP_Text textoCountDown;
        private void Awake()
        {

        Debug.unityLogger.logEnabled = false;

        }
        void Start()
        {
            Debug.Log("App Ver:" + Application.version);
            versionText.SetActive(Debug.isDebugBuild);
            versionText.GetComponent<TMP_Text>().SetText("v"+Application.version);

            _masterBrinco = this;
            Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
            Eventos_Dispatcher.eventos.JugadorPerdio += PerdioJuego;
            Eventos_Dispatcher.MonedaTomada += MonedaTomada;
            Eventos_Dispatcher.CruceObstaculo += ObstaculoCruzado;
            
            velocidadObstaculos = velocidadObstaculosInicial;
            pasarASigVelocidadEn = RandomSigCambio();
            estadoJuego = EstadoJuego.inicio;

            PantallaNegra("out");


            tutorialCompletado = SaveGame.Load<bool>("tutorialCompletado");
          // tutorialCompletado = false;
          // boton_inicio. += InicioJuego_UI;
        }

        public void SetDificultad()
        {
            velocidadObstaculos = velocidadObstaculosInicial;

        }


        ///<sumary>
        ///Llamado por EventDispatcher.InicioJuego
        ///</sumary>
        private void InicioJuego()
        {
            print("Se inicio Juego");

            if (!tutorialCompletado)
            {
                //  StartCoroutine(InicioTutorial_Rutina());
                panelTutorial.SetActive(true);
                estadoJuego = EstadoJuego.tutorial;
            }
            else
            {
                estadoJuego = EstadoJuego.jugando;
                StartCoroutine(InicioJuego_Rutina());
            }

            SpawnEscenario.instancia.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
                                                          nivelesDificultad[enNivel].rateSpawn,
                                                          nivelesDificultad[enNivel].probabilidadMoneda,
                                                          nivelesDificultad[enNivel].spawnearTNT,
                                                          nivelesDificultad[enNivel].spawnearCintasTransportadorasRotas,
                                                          nivelesDificultad[enNivel].minSigObstaculo,
                                                          nivelesDificultad[enNivel].maxSigObstaculo);


        }
        private void PerdioJuego()
        {
            estadoJuego = EstadoJuego.perdio;
       
            panel_reinicio.SetActive(true);

            print("GAME OVER");
        }


        /// <summary>
        /// Llamado con el boton play de parte del jugador
        /// </summary>
        public void InicioJuego_UI()
        {
            Eventos_Dispatcher.eventos.InicioJuego_llamada();
            LeanTween.moveLocalY(boton_jugar.gameObject, -800.0f, 0.5f).setEaseInOutSine();

            VibracionesControl.instancia.Vibrar(TipoVibracion.Exito);
        }
        
        /// <summary>
        /// Llamado por el boton Reiniciar de UI
        /// </summary>
        public void Reiniciar()
        {
            if (estadoJuego == EstadoJuego.reiniciando)
                return;

            estadoJuego = EstadoJuego.reiniciando;
            //Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
            //Eventos_Dispatcher.eventos.JugadorPerdio -= PerdioJuego;
            StartCoroutine(SecuenciaReinicio());
            //Score_Control.instancia.GuardarMonedas();
            //PantallaNegra("in");
            
        }
        /// <summary>
        /// Llamado por Score_Control Si se guardaron las monedas exitosamente o no
        /// </summary>
        public void Reiniciar_Callback()
        {
            Debug.Log("Reiniciando...");
            SceneManager.LoadScene(1);

        }
        IEnumerator SecuenciaReinicio()
        {
            Save_Control.instancia.GuardarJuego();
            PantallaNegra("in");

            yield return new WaitForSeconds(1.3f);

            #region Nuevo sistema Reinicio

            panel_reinicio.SetActive(false);
            enNivel = 0;
            obstaculosCruzados = 0;
            boton_jugar.gameObject.SetActive(true);
            Eventos_Dispatcher.eventos.Reinico_Call();
            PantallaNegra("out");
            LeanTween.moveLocalY(boton_jugar.gameObject, -425.0f, 0.5f).setEaseInOutSine();
            estadoJuego = EstadoJuego.inicio;

            #endregion



            //HARDCORD SOLUTION
            //SceneManager.LoadScene(1);
        }


        /// <summary>
        /// Llamado por UI por el usuario para que pueda revivir a cambio de una cantidad 
        /// de cafe, se evalua si tiene el cafe suficiente de lo contrario no lo revive
        /// </summary>
        public void RevivirConCafe()
        {
            if (GetComponent<Score_Control>().MonedasTotales < costoCafeRevivir)
            {
                Sonido_Control.sonidos.ReproducirSonido_UI("errorBoton");
                VibracionesControl.instancia.Vibrar(TipoVibracion.Error);
                Debug.Log("Sin cafe suficiente");
                return;
            }
            else
            {
                Revivir();
                GetComponent<Score_Control>().RestarMonedas(costoCafeRevivir);
            }
        }
        /// <summary>
        /// Llamado para iniciar la secuencia de Revivir
        /// </summary>
        public void Revivir()
        { 
            //Checar si tiene suficientes monedas
           
            StartCoroutine(Revivir_Secuencia());
        }

        private IEnumerator Revivir_Secuencia()
        {

            jugador.PrepararParaRevivir();
            panelCountDown.SetActive(true);
            textoCountDown.text = "3";
            LeanTween.value(4, 0, tiempoReinicio).setOnUpdate((float t)=> {

                int tiempo = Mathf.FloorToInt(t);
                Debug.Log("Tiempo:"+tiempo);
                textoCountDown.text =  tiempo <= 0? "GO": tiempo.ToString();


            });
            panel_reinicio.SetActive(false);

            yield return new WaitForSeconds(3);
            panelCountDown.SetActive(false);

            estadoJuego = EstadoJuego.jugando;
            panel_reinicio.SetActive(false);

          
            Eventos_Dispatcher.eventos.Revivir_Call();
            Debug.Log("Reviviendo");
        }
        private void OnDestroy()
        {
            Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
            Eventos_Dispatcher.eventos.JugadorPerdio -= PerdioJuego;
            Eventos_Dispatcher.MonedaTomada -= MonedaTomada;
            Eventos_Dispatcher.CruceObstaculo -= ObstaculoCruzado;
        }

        #region Tutorial
        IEnumerator InicioTutorial_Rutina()
        {
            boton_jugar.gameObject.SetActive(false);
            jugador.spriteAnim.CambioAnimacion("corriendo",true);
            yield return new WaitForSeconds(1.5f);
            panelTutorial.SetActive(true);
            jugador.puedeBrincar = true;

           

        }

        /// <summary>
        /// Llamado para pasar a la siguiente instruccion, por el momento por BrincoAcumulado.cs
        /// </summary>
        /// <param name="nombre">brinco,regreso</param>
        public void InstruccionesCompletada(string nombre)
        {
            if(nombre =="brinco")
            {

                panelTutorial.GetComponent<Animator>().SetBool("tutorialRegreso", true);
                LeanTween.value(1.0f, 0.5f, 0.5f).setOnUpdate((float val)=>{
                    Time.timeScale = val;
                });

            }else if(nombre == "regreso")
            {
                panelTutorial.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                estadoJuego = EstadoJuego.jugando;
                StartCoroutine(InicioJuego_Rutina());
                tutorialCompletado = true;
                SaveGame.Save("tutorialCompletado", tutorialCompletado);
            }
        }

        public void CerrarInsturcciones()
        {
            panelTutorial.gameObject.SetActive(false);
            
            estadoJuego = EstadoJuego.jugando;
            StartCoroutine(InicioJuego_Rutina());
            tutorialCompletado = true;
            SaveGame.Save("tutorialCompletado", tutorialCompletado);
        }
        #endregion

        /// <summary>
        /// Secuencia de como inicia el juego despues de presionar PLAY
        /// </summary>
        /// <returns></returns>
        IEnumerator InicioJuego_Rutina()
        {
            yield return new WaitForSeconds(1.0f);
            boton_jugar.gameObject.SetActive(false);
            jugador.puedeBrincar = true;

          

            SpawnEscenario.instancia.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
                                                           nivelesDificultad[enNivel].rateSpawn,
                                                           nivelesDificultad[enNivel].probabilidadMoneda,
                                                           nivelesDificultad[enNivel].spawnearTNT,
                                                           nivelesDificultad[enNivel].spawnearCintasTransportadorasRotas,
                                                           nivelesDificultad[enNivel].minSigObstaculo,
                                                           nivelesDificultad[enNivel].maxSigObstaculo);
           
           if(SpawnObstaculos_Apertura.spawner)
           {
            yield return new WaitForSeconds(0.5f);
           // EncenderBandaTransportadora(true);
            SpawnObstaculos_Apertura.spawner.EmpezoJuego();
         
            yield return new WaitForSeconds(1.5f);
            SpawnObstaculos_Apertura.spawner.ActivarObstaculo();
           }
        }

        

        private void MonedaTomada()
        {
            if(estadoJuego == EstadoJuego.perdio)
                return;
            monedasTomadas++;
           // monedas_txt.SetText(monedasTomadas.ToString("000"));

        }
         ///<sumary>
         ///Llamado por EventDispatcher desde jugador cuando este curza un obstaculo
         //lleva el conteo de cuantos obstaculos se han cruzados y llama al cambio de velocidad 
        ///</sumary>    
        public void ObstaculoCruzado()
        {   
             if(estadoJuego != EstadoJuego.jugando)
                return;

            obstaculosCruzados++;
            // if(enNivel+1 >= nivelesDificultad.Length){

            //       Debug.Log("Ya no hay mas niveles");
            //       //Volvemos a llamas SetDificultad para que se siga multiplicando la velocidad
            //      SpawnObstaculos_Apertura.spawner.SetDificultad(SigVelocidad(), nivelesDificultad[enNivel].cantidadEspacios);
            //     return;

            // }else 
            if(obstaculosCruzados == nivelesDificultad[enNivel].obstaculFinal)
            {
               // pasarASigVelocidadEn = RandomSigCambio();
                //CambiarVelocidad();
                if(enNivel + 1 >= nivelesDificultad.Length)
                {
                    Debug.Log("Ya no hay mas niveles");
                    
                }else{
                 
                    enNivel++;

                }
                //  SpawnObstaculos_Apertura.spawner.SetDificultad(SigVelocidad(), nivelesDificultad[enNivel].cantidadEspacios);
                
                //DEPRECATED Marzo 23
                // SpawnObstaculos_Apertura.spawner.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
                                                            //    nivelesDificultad[enNivel].rateSpawn,
                                                            //    nivelesDificultad[enNivel].probabilidadMoneda,
                                                            //    nivelesDificultad[enNivel].spawnearTNT);

                SpawnEscenario.instancia.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
                                                           nivelesDificultad[enNivel].rateSpawn,
                                                           nivelesDificultad[enNivel].probabilidadMoneda,
                                                           nivelesDificultad[enNivel].spawnearTNT,
                                                           nivelesDificultad[enNivel].spawnearCintasTransportadorasRotas,
                                                           nivelesDificultad[enNivel].minSigObstaculo,
                                                           nivelesDificultad[enNivel].maxSigObstaculo);
                //SpawnObstaculos_Apertura.spawner.gameObject.GetComponent<SpawnEscenario>().spawnearCintasRotas = nivelesDificultad[enNivel].spawnearCintasTransportadorasRotas;
                
                Debug.Log("Se aumento la dificultad nivel: "+ enNivel);
            }
            //DEPRECATED(08/01/2021)
            //score_tex.SetText(obstaculosCruzados.ToString());

        }
         ///<sumary>
         ///Se llama cuando el jugdor pasa el obstaculo n = pasarASigVelocidadEn
        ///</sumary>
        public void CambiarVelocidad()
        {
            Debug.Log("Cambiando Velocidad...");

            //Eventos_Dispatcher.eventos.CambioVelocidad_Call(SigVelocidad());
        }

        ///<sumary>
        ///Elige entre 2 numeros enteros en que obstaculo el jugador pasa a la siguienteVelocidad y se vuelve a sumar
        ///</sumary>
        int RandomSigCambio()
        {
            int r = UnityEngine.Random.Range(pasarSigVelocidad_Min,pasarSigVelocidad_Max);

            return r;
        }

        float SigVelocidad()
        {
            float r = velocidadObstaculos * nivelesDificultad[enNivel].multiplicadorVelocidad;
            return r;
        }

        public void EncenderBandaTransportadora(bool _encender)
        {
            if (bandasTransportadoras.Length <= 0)
                return;
            for (int i = 0; i < bandasTransportadoras.Length; i++)
            {
                bandasTransportadoras[i].SetBool("corriendo", _encender);
            }
        }
        

        /// <summary>
        /// Anima la pantalla para sacar o entrar, utilizada principalmente
        /// cuando reiniciamos el juego
        /// </summary>
        /// <param name="inout">se espera string: in o out</param>
        void PantallaNegra(string inout)
        {
            if(pantallNegro == null)
            {
                Debug.Log("No se encontro pantalla negro");
                return;
            }
            if (!pantallNegro.activeInHierarchy)
                pantallNegro.SetActive(true);

            if(inout == "in")
            {
                pantallNegro.transform.position = new Vector3(2400f,1400f,0f);

                LeanTween.moveLocalX(pantallNegro, 0f, 0.5f).setEaseInOutSine();

            }else if(inout =="out")
            {
                //pantallNegro.transform.position = new Vector3(900f,450f,0f);
                LeanTween.moveLocalX(pantallNegro, -900f, 0.5f).setEaseInOutSine();

            }
        }
    }
    
}

[System.Serializable]
public class Niveles
{
    public int obstaculFinal;//obstaculo donde termina el  y empieza el siguiente, 1 -> infinito
    public int cantidadEspacios;
    public float multiplicadorVelocidad;//por cuanto hacemos el incremente de velocidad, tomando en cuenta la velocidad anterior
    public float rateSpawnMin;
    public float rateSpawnMax;

    public float rateSpawn;

    public int minSigObstaculo;
    public int maxSigObstaculo;

    [Tooltip("Probabilidad que aparezca una moneda(int) r > 10-probabilidadMoneda")]
    public int probabilidadMoneda;
    [Tooltip("Se spawnearan obstaculos extras en las orillsa(niveles mas dificiles")]
    public bool obstaculoOrilla;
    [Tooltip("Se spawnean cintaTransportadora rota")]
    public bool spawnearCintasTransportadorasRotas;
    [Tooltip("Espawnear TNT?")]
    public bool spawnearTNT;
}

public enum EstadoJuego
{
    jugando,
    perdio,
    inicio,
    reiniciando,
    tutorial
}