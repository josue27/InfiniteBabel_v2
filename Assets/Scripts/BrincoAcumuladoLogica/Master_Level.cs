using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//TODO: Desuscribir a todos cuando el juego pierda o ver como se hace
namespace Brinco
{
    
    public class Master_Level : MonoBehaviour
    {
        
        public static Master_Level _masterBrinco;
        
        // Start is called before the first frame update
        public Button boton_inicio;
        public Button boton_reinicio;
        void Start()
        {
            _masterBrinco = this;
            Eventos_Dispatcher.inicioJuego += InicioJuego;
            Eventos_Dispatcher.jugadorPerdio += PerdioJuego;
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
            Eventos_Dispatcher.inicioJuego();

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
            yield return new WaitForSeconds(0.5f);
            boton_inicio.gameObject.SetActive(false);
        }
    }
    
}

