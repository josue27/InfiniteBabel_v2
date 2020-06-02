using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Brinco;
[Serializable]
public  class Eventos_Dispatcher : MonoBehaviour
{
    public static Action jugadorPerdio;
    public static Action inicioJuego;

    
   
    public void JugadorPerdioLlamada()
    {
        print("Llamando Perdio");
        jugadorPerdio();
    }
    
}
