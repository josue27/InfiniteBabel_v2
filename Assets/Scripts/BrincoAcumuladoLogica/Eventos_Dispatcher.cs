using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Brinco;
[System.Serializable]
public  class Eventos_Dispatcher : MonoBehaviour
{
    public static Eventos_Dispatcher eventos;
    public  Action JugadorPerdio;
    public  Action InicioJuego;

    public static Action MonedaTomada;

    public static Action CruceObstaculo;

    public static Action<float> CambioVelocidad;
    
    public Action<string> GuardarPersonaje;
   void Awake()
   {
       eventos = this;
   }
   public void InicioJuego_llamada() => InicioJuego?.Invoke();
    public void JugadorPerdioLlamada()
    {
        print("Llamando Perdio");
        JugadorPerdio();
    }
    public void MonedaTomada_Call() => MonedaTomada?.Invoke();

    public void CruceObstaculo_Call() => CruceObstaculo?.Invoke();

    public void CambioVelocidad_Call(float nuevaVelocidad) => CambioVelocidad?.Invoke(nuevaVelocidad);

    public void GuardarPersonaje_Call(string nombrePersonaje) => GuardarPersonaje?.Invoke(nombrePersonaje);
}
