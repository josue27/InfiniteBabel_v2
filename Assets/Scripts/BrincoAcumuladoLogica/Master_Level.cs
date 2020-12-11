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
        public Button boton_inicio;
        public GameObject panel_reinicio;
        public TMP_Text scoreFinal;
        public TMP_Text scoreBest;


        public EstadoJuego estadoJuego;
        [Header("Ritmo Niveles")]
        public float velocidadObstaculosInicial = 1.0f;
        public float velocidadObstaculos ;
        public float porcentajeAumento = 1.10f;

        //Despues de cuantos obstaculos pasamos a la siguiente velocidad
        //Debera ser  un aleatorio entre un minimo y maximo
        public int pasarASigVelocidadEn;
        public int pasarSigVelocidad_Min = 10;
        public int pasarSigVelocidad_Max = 12;

        //Debe ir en descalada para que sean mas pronta las velocidades
        public float rateSpawnMin, rateSpawnMax;
        private int enNivel = 0;
        public Niveles[] nivelesDificultad;

        [Header("Score")]
        public int obstaculosCruzados;
        public TMP_Text score_tex;

        [Header("Monedas")]
        public int monedasTomadas;
        public TMP_Text monedas_txt;

        [Space(10)]
        [Header("BandaTransportadora")]
        public Animator[] bandasTransportadoras;

        [Space(10)]
        [Header("PantallaReinicio")]
        public GameObject pantallNegro;

        public bool tutorialCompletado;

       
        public GameObject panelTutorial;//params:tutorialRegreso;

        public BrincoAcumulado jugador;
        
        private void OnValidate()
        {
           // Debug.Log( pantallNegro.transform.position);
        }
        void Start()
        {
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
          //  boton_inicio. += InicioJuego_UI;
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
                StartCoroutine(InicioTutorial_Rutina());
                estadoJuego = EstadoJuego.tutorial;
            }
            else
            {
                estadoJuego = EstadoJuego.jugando;
                StartCoroutine(InicioJuego_Rutina());
            }
            
            
        }
         private void PerdioJuego()
        {
            estadoJuego = EstadoJuego.perdio;

            if (Score_Control.instancia)
            {
                scoreBest.text = $"{Score_Control.instancia.HighscoreUsuario}";
                scoreFinal.text = $"{Score_Control.instancia.ScoreRonda}";
            }
            panel_reinicio.SetActive(true);

            print("GAME OVER");
        }


        /// <summary>
        /// Llamado con el boton play de parte del jugador
        /// </summary>
        public void InicioJuego_UI()
        {
            Eventos_Dispatcher.eventos.InicioJuego_llamada();

        }
        
        public void Reiniciar()
        {
            if (estadoJuego == EstadoJuego.reiniciando)
                return;

            estadoJuego = EstadoJuego.reiniciando;
            Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
            Eventos_Dispatcher.eventos.JugadorPerdio -= PerdioJuego;
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
            Score_Control.instancia.GuardarMonedas();
            PantallaNegra("in");

            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene(1);
        }

        private void OnDestroy()
        {
            Eventos_Dispatcher.eventos.InicioJuego -= InicioJuego;
            Eventos_Dispatcher.eventos.JugadorPerdio -= PerdioJuego;
        }


        IEnumerator InicioTutorial_Rutina()
        {
            boton_inicio.gameObject.SetActive(false);
            jugador.spriteAnim.CambioAnimacion("corriendo",true);
            yield return new WaitForSeconds(1.5f);
            panelTutorial.SetActive(true);
            jugador.puedeBrincar = true;

            SpawnEscenario.instancia.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
                                                           nivelesDificultad[enNivel].rateSpawn,
                                                           nivelesDificultad[enNivel].probabilidadMoneda,
                                                           nivelesDificultad[enNivel].spawnearTNT,
                                                           nivelesDificultad[enNivel].spawnearCintasTransportadorasRotas,
                                                           nivelesDificultad[enNivel].minSigObstaculo,
                                                           nivelesDificultad[enNivel].maxSigObstaculo);

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
            }
        }

        /// <summary>
        /// Secuencia de como inicia el juego despues de presionar PLAY
        /// </summary>
        /// <returns></returns>
        IEnumerator InicioJuego_Rutina()
        {
            yield return new WaitForSeconds(1.0f);
            boton_inicio.gameObject.SetActive(false);
            jugador.puedeBrincar = true;

            // SpawnObstaculos_Apertura.spawner.SetDificultad(this.velocidadObstaculos,nivelesDificultad[enNivel].cantidadEspacios);
            //SpawnObstaculos_Apertura.spawner.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
            //                                               nivelesDificultad[enNivel].rateSpawn,
            //                                               nivelesDificultad[enNivel].probabilidadMoneda);

            SpawnEscenario.instancia.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
                                                           nivelesDificultad[enNivel].rateSpawn,
                                                           nivelesDificultad[enNivel].probabilidadMoneda,
                                                           nivelesDificultad[enNivel].spawnearTNT,
                                                           nivelesDificultad[enNivel].spawnearCintasTransportadorasRotas,
                                                           nivelesDificultad[enNivel].minSigObstaculo,
                                                           nivelesDificultad[enNivel].maxSigObstaculo);
            yield return new WaitForSeconds(0.5f);
           // EncenderBandaTransportadora(true);
            SpawnObstaculos_Apertura.spawner.EmpezoJuego();
         
            yield return new WaitForSeconds(1.5f);
            SpawnObstaculos_Apertura.spawner.ActivarObstaculo();
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
             if(estadoJuego == EstadoJuego.perdio)
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
                SpawnObstaculos_Apertura.spawner.SetDificultad(nivelesDificultad[enNivel].cantidadEspacios,
                                                               nivelesDificultad[enNivel].rateSpawn,
                                                               nivelesDificultad[enNivel].probabilidadMoneda,
                                                               nivelesDificultad[enNivel].spawnearTNT);

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

            score_tex.SetText(obstaculosCruzados.ToString());

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