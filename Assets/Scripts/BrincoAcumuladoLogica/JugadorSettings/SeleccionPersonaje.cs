using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeleccionPersonaje : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("UIX")]
    public GameObject botonesSeleccion;
    void Start()
    {
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
    }

    private void InicioJuego()
    {
        botonesSeleccion.SetActive(false);
    }
}
