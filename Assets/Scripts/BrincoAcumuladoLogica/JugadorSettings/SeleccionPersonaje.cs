using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Brinco
{
    public class SeleccionPersonaje : MonoBehaviour
    {
        public static SeleccionPersonaje _seleccionPersonaje;
        public Sprite_Animador spriteAnimacion;
        public int enPersonaje;
        public List<PersonajesEnJuego> personajes = new List<PersonajesEnJuego>();
        [Header("UIX")]
        public GameObject botonesSeleccion;
        public GameObject panelNombrePersonaje;
        public TMP_Text nombrePersonaje_text;
        public TMP_Text precioPersonaje_text;
        public GameObject candado_img;

        int personajeCompradoListo = 0;//en teoria deberia ser le primer dude
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
            panelNombrePersonaje.SetActive(false);
            // GuardarPersonajeSeleccionado();
            VerificarSiEstaComprado();

        }
        public void SiguientePersonaje()
        {
            enPersonaje++;
            if (enPersonaje >= personajes.Count)
                enPersonaje = 0;

            CambiarPersonaje();
        }
        public void AnteriorPersonaje()
        {
            enPersonaje--;
            if (enPersonaje <= 0)
                enPersonaje = personajes.Count - 1;

            CambiarPersonaje();
        }
        public void CambiarPersonaje()
        {
            spriteAnimacion.CambiarSprites(personajes[enPersonaje]);
            //cambiar texto con el nombre del personaje
            nombrePersonaje_text.SetText(personajes[enPersonaje].personaje.nombreUI);
            if (personajes[enPersonaje].comprado)
            {
                precioPersonaje_text.SetText("OWNED");
                personajeCompradoListo = enPersonaje;
            }
            else
            {
                precioPersonaje_text.SetText($"{personajes[enPersonaje].personaje.precio} x");
            }
            candado_img.SetActive(!personajes[enPersonaje].comprado);
            //Debug.Log("Se cambio personaje...mandando sprites");
        }


        /// <summary>
        /// Busca un personaje de acuerdo a su nombre y cambia el sprite, Actualmente llamado por Score_Control
        /// 
        /// /// </summary>
        /// <param name="nombreBuscado">nombre del personaje</param>
        public void BuscarPersonaje(string nombreBuscado)
        {


            for (int i = 0; i < personajes.Count; i++)
            {
                if (personajes[i].personaje.nombre == nombreBuscado)
                {
                    enPersonaje = i;
                    CambiarPersonaje();
                    Debug.Log("Se encontro personaje guardado y en teoria comprado");
                    break;
                }
            }
        }

        /// <summary>
        /// Llama a guardar el personaje seleccionado a Score_Control, normalmente
        /// es llamada para guardar un personaje que el Usuario tiene comprado
        ///  </summary>
        private void GuardarPersonajeSeleccionado()
        {
            if (personajes.Count <= 0)
                return;

            string _nombrePersonaje = "";

            _nombrePersonaje = personajes[enPersonaje].personaje.nombre;
            Eventos_Dispatcher.eventos.GuardarPersonaje_Call(_nombrePersonaje);
        }

        public PersonajeScriptable BuscarDataPersonaje(string nombre)
        {
            foreach (PersonajesEnJuego _personaje in personajes)
            {
                if (_personaje.personaje.nombre == nombre)
                {
                    return _personaje.personaje;


                }
            }
            return null;
        }

        /// <summary>
        /// Verifica si el personaje seleccionado esta comprado, de lo contrario 
        /// simplemente cambia la variable enPersonaje a una que se guardo de el ultimo
        /// personaje que
        /// en teoria ya deberia estar comprado, de lo contrario significa que el usuario ya compro ese personaje y lo puede utilizar, ademas de guardarlo
        /// </summary>
        public void VerificarSiEstaComprado()
        {

            if (personajes[enPersonaje].comprado == false)
            {
                enPersonaje = personajeCompradoListo;
                CambiarPersonaje();
            }
            else
            {
                GuardarPersonajeSeleccionado();

                if (personajes[enPersonaje].personaje.nombre == "coffeeguy")
                {
                    Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_more_coffee_please);
                }else if(personajes[enPersonaje].personaje.nombre == "punkman")
                {
                    Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_down_the_system);
                }
                else if(personajes[enPersonaje].personaje.nombre == "naked_man")
                {
                    Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_no_shame);
                }
            }
        }


        /// <summary>
        /// Llamado por Compras_Control.cs, se encarga de evaluar si se puede comprar el personaje o no
        /// </summary>
        public void DesbloquearPersonaje()
        {
            if (personajes[enPersonaje].comprado)
            {
                Debug.Log($"Intento comprar {personajes[enPersonaje].personaje.name} ya comprado");
                return;
            }

            if (Score_Control.instancia.MonedasTotales >= personajes[enPersonaje].personaje.precio)
            {
                personajes[enPersonaje].comprado = true;
                Score_Control.instancia.RestarMonedas(personajes[enPersonaje].personaje.precio);
                CambiarPersonaje();
                ///Tenemos que guardar en la nube y local que ya se compro el mono
                ///EN teoria no, porque ya implementamos salvado si quita o se va
                // Score_Control.instancia.Guardar_MonedasYPersonajes();
                Logros_Control.instancia.DesbloquearLogro(EM_GPGSIds.achievement_work_with_style);
            }
            else
            {
                Debug.Log("Sin monedas suficientes");
                //TODO: Agregar SFX incorrecto y animacion candado

            }

        }

        /// <summary>
        /// Llamado para desbloquear personajes si el jugador los compro en base a los datos guardados(nube o local)
        /// </summary>
        /// <param name="personajesEnNube"></param>
        public void VerificarPersonajesComprados(List<PersonajeSalvado> personajesEnNube)
        {
            if (personajesEnNube.Count <= 0)
            {
                Debug.Log("No habia personajes guardados en nube o local");
                return;
            }
            foreach (PersonajeSalvado personajeEN in personajesEnNube)
            {
                foreach (PersonajesEnJuego personajeEJ in personajes)
                {
                    if (personajeEJ.personaje.nombre == personajeEN.nombre)
                    {
                        //si el personajeEnJuego esta comprado entonces es verdad
                        //si es false, entonces checamos el estatus del personaje en la nube porque puede ser que si este comprado
                        //Esto es para que el sistema de guardado no se confunda
                        personajeEJ.comprado = personajeEJ.comprado ? true : personajeEN.comprado;
                        Debug.Log($"Personaje{personajeEJ.personaje.nombre} comprado: {personajeEJ.comprado}");
                    }
                }
            }
            CambiarPersonaje();
            Debug.Log("Personajes estatus compra recuperados");
        }
    }

    [System.Serializable]
    public class PersonajesEnJuego
    {
        public PersonajeScriptable personaje;
        public bool comprado;
    }
}
