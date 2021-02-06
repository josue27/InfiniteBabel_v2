using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementoEscenario_Control : MonoBehaviour
{
  
    public ObstaculoControl obstaculo;
    public GameObject letreroWarning;
    public void Mover(Vector3 aPosicion,float duracion)
    {

        Eventos_Dispatcher.eventos.InicioJuego += ContinuarMovimiento;
        LeanTween.move(this.gameObject,aPosicion,duracion).setOnComplete(()=>
        {
              //  Debug.Log(this.gameObject.name+" termino recorrido" );
                Reiniciar();

         });
    }

    private void OnEnable()
    {
      
        Eventos_Dispatcher.Reinicio += ReiniciarJuego;
    }
    private void OnDisable()
    {
        Eventos_Dispatcher.Reinicio -= ReiniciarJuego;
        Eventos_Dispatcher.eventos.InicioJuego -= ContinuarMovimiento;


    }
    /// <summary>
    /// Se utiliza para parar el movimiento tween del objeto, sobre todo cuando el jugador pierde
    /// esta funcion es llamada desde SpawnEscenario.cs
    /// </summary>
    public void Parar()
    {
        //LeanTween.cancel(this.gameObject);
        LeanTween.pause(this.gameObject);
    }
   
    /// <summary>
    /// Llamada por mover para dar a entender que ya se cumplio totalmente su moviemineto y debe volver al Que
    /// </summary>
    public void Reiniciar()
    {
        if(this.transform.CompareTag("piso"))
        {
          transform.GetChild(0).gameObject.SetActive(true);
          transform.GetChild(2).gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }

    public void ReiniciarJuego()
    {
        if (this.transform.CompareTag("piso"))
        {
            obstaculo.gameObject.SetActive(false);

        }

    }

    public void ContinuarMovimiento()
    {
        if(this.gameObject.activeInHierarchy)
            LeanTween.resume(gameObject);

    }

    public void ActivarObstaculo(int _cantidadCajas,bool _spawnTNT, bool _moneda)
    {
        obstaculo.gameObject.SetActive(true);
        obstaculo.SetObstaculo(_cantidadCajas,0,_spawnTNT);
        if (_moneda)
            obstaculo.ActivarMoneda();

       
    }
}
