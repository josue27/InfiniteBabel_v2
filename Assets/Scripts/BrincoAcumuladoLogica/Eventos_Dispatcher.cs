using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Brinco;
[System.Serializable]
public  class Eventos_Dispatcher : MonoBehaviour
{
    public static Eventos_Dispatcher eventos;
    public static Action jugadorPerdio;
    public static Action inicioJuego;

    public static Action MonedaTomada;

    public static Action CruceObstaculo;

    public static Action<float> CambioVelocidad;
  
   void Awake()
   {
       eventos = this;
   }
   public void InicioJuego_llamada() => inicioJuego?.Invoke();
    public void JugadorPerdioLlamada()
    {
        print("Llamando Perdio");
        jugadorPerdio();
    }
    public void MonedaTomada_Call() => MonedaTomada?.Invoke();

    public void CruceObstaculo_Call() => CruceObstaculo?.Invoke();

    public void CambioVelocidad_Call(float nuevaVelocidad) => CambioVelocidad?.Invoke(nuevaVelocidad);


}
