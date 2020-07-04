using System;
using System.Collections;
 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//TODO: Desuscribir a todos cuando el juego pierda o ver como se hace
namespace Brinco
{
    
    public class Master_Level : MonoBehaviour
    {
        
        public static Master_Level _masterBrinco;
        
        // Start is called before the first frame update
        public Button boton_inicio;
        public Button boton_reinicio;
        public Button boton_brinco;

        [Header("Pasing Niveles")]
        public float velocidadObstaculosInicial = 1.0f;
        public float sigVelocidad ;
        public float porcentajeAumento = 1.10f;

        //Despues de cuantos obstaculos pasamos a la siguiente velocidad
        //Debera ser  un aleatorio entre un minimo y maximo
        public int pasarASigVelocidadEn;
        public int pasarSigVelocidad_Min = 10;
        public int pasarSigVelocidad_Max = 12;

        //Debe ir en descalada para que sean mas pronta las velocidades
        public float rateSpawnMin, rateSpawnMax;

        [Header("Score")]
        public int obstaculosCruzados;
        public TMP_Text score_tex;

        [Header("Monedas")]
        public int monedasTomadas;
        public TMP_Text monedas_txt;

        void Start()
        {
            _masterBrinco = this;
            Eventos_Dispatcher.inicioJuego += InicioJuego;
            Eventos_Dispatcher.jugadorPerdio += PerdioJuego;
            Eventos_Dispatcher.MonedaTomada += MonedaTomada;
            Eventos_Dispatcher.CruceObstaculo += ObstaculoCruzado;
            
            sigVelocidad = velocidadObstaculosInicial;
            pasarASigVelocidadEn = RandomSigCambio();
          //  boton_inicio. += InicioJuego_UI;
        }


        private void InicioJuego()
        {
            print("Se inicio Juego");
            StartCoroutine(InicioJuego_Rutina());
        }
         private void PerdioJuego()
        {
            print("GAME OVER");
            boton_reinicio.gameObject.SetActive(true);
        }

        public void InicioJuego_UI()
        {
            Eventos_Dispatcher.eventos.InicioJuego_llamada();

        }
        
        public void Reiniciar()
        {
            Eventos_Dispatcher.inicioJuego -= InicioJuego;
            Eventos_Dispatcher.jugadorPerdio -= PerdioJuego;
            SceneManager.LoadScene(0);
        }
        private void OnDestroy()
        {
            Eventos_Dispatcher.inicioJuego -= InicioJuego;
            Eventos_Dispatcher.jugadorPerdio -= PerdioJuego;
        }

        IEnumerator InicioJuego_Rutina()
        {
            boton_inicio.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            boton_brinco.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            SpawnObstaculos_Apertura.spawner.ActivarObstaculo();
        }

        public void ObstaculoPasado(){

        }

        private void MonedaTomada()
        {

            monedasTomadas++;
            monedas_txt.SetText("x"+monedasTomadas.ToString());

        }

        public void ObstaculoCruzado()
        {

            obstaculosCruzados++;

            if(obstaculosCruzados == pasarASigVelocidadEn)
            {
                pasarASigVelocidadEn = RandomSigCambio();
                CambiarVelocidad();

            }

            score_tex.SetText(obstaculosCruzados.ToString());

        }

        public void CambiarVelocidad()
        {
            Debug.Log("Cambiando Velocidad...");

            Eventos_Dispatcher.eventos.CambioVelocidad_Call(SigVelocidad());
        }

        int RandomSigCambio()
        {
            int r = UnityEngine.Random.Range(pasarSigVelocidad_Min,pasarSigVelocidad_Max);

            return r;
        }

        float SigVelocidad()
        {
            float r = sigVelocidad * 1.10f;
            return r;
        }
    }
    
}

