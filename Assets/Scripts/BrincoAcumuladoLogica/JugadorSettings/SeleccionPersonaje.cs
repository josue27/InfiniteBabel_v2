using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SeleccionPersonaje : MonoBehaviour
{
    public static SeleccionPersonaje _seleccionPersonaje;
    public Sprite_Animador spriteAnimacion;
    public int enPersonaje;
    public List<PersonajeScriptable> personajes = new List<PersonajeScriptable>();
    [Header("UIX")]
    public GameObject botonesSeleccion;
    public TMP_Text nombrePersonaje_text;
    
     /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _seleccionPersonaje = this;
    }
    void Start()
    {
        Eventos_Dispatcher.eventos.InicioJuego += InicioJuego;
        CambiarPersonaje();
    }

    private void InicioJuego()
    {
        botonesSeleccion.SetActive(false);
        nombrePersonaje_text.gameObject.SetActive(false);
        GuardarPersonajeSeleccionado();
    }
    public void SiguientePersonaje()
    {
        enPersonaje++;
        if(enPersonaje >= personajes.Count)
            enPersonaje = 0;

        CambiarPersonaje();
    }
    public void AnteriorPersonaje()
    {
        enPersonaje--;
        if(enPersonaje <=0 )
           enPersonaje = personajes.Count-1;

        CambiarPersonaje();
    }
    public void CambiarPersonaje()
    {
        spriteAnimacion.CambiarSprites(personajes[enPersonaje]);
        //cambiar texto con el nombre del personaje
        nombrePersonaje_text.SetText(personajes[enPersonaje].nombreUI);
        Debug.Log("Se cambio personaje...mandando sprites");
    }


    /// <summary>
    /// Busca un personaje de acuerdo a su nombre y cambia el sprite, Actualmente llamado por Score_Control
    /// 
    /// /// </summary>
    /// <param name="nombreBuscado">nombre del personaje</param>
    public void BuscarPersonaje(string nombreBuscado)
    {
        // foreach(PersonajeScriptable personaje in personajes)
        // {
        //     if(personaje.nombre == nombreBuscado)
        //     {
        //         enPersonaje = personajes.IndexOf(personaje);
        //         CambiarPersonaje();
        //         Debug.Log("se encontro personaje");
        //         break;
        //     }
        // }

        for (int i = 0; i < personajes.Count; i++)
        {
            if(personajes[i].nombre == nombreBuscado)
            {
                enPersonaje = i;
                CambiarPersonaje();
                Debug.Log("Se encontro personaje");
                break;
            }
        }
    }

    private void GuardarPersonajeSeleccionado()
    {
        if(personajes.Count <= 0)
             return;

        string _nombrePersonaje = "";

        _nombrePersonaje = personajes[enPersonaje].nombre;
        Eventos_Dispatcher.eventos.GuardarPersonaje_Call(_nombrePersonaje);
    }
}
