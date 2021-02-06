using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EasyMobile;
#if EM_UIAP
using UnityEngine.Purchasing;
#endif

namespace Brinco
{
    public class SeleccionPersonaje : MonoBehaviour
    {
        public static SeleccionPersonaje _seleccionPersonaje;
        public Sprite_Animador spriteAnimacion;
        public int enPersonaje;
        public List<PersonajesEnJuego> personajes = new List<PersonajesEnJuego>();
        [SerializeField] private List<PersonajeSalvado> personajesEnNube;
        [Header("UIX")]
        public GameObject botonesSeleccion;
        public GameObject panelNombrePersonaje;
        public TMP_Text nombrePersonaje_text;
        public TMP_Text precioPersonaje_text;
        public GameObject candado_img;
        public GameObject moneda_img;

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
            Eventos_Dispatcher.Reinicio += Reinicio;
            CambiarPersonaje();
            //GameServices.Init();
        }

        void Reinicio()
        {
            botonesSeleccion.SetActive(true);
            panelNombrePersonaje.SetActive(true);
            VerificarSiEstaComprado();

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
            VibracionesControl.instancia.Vibrar(TipoVibracion.Seleccion);
        }
        public void AnteriorPersonaje()
        {
            enPersonaje--;
            if (enPersonaje < 0)
                enPersonaje = personajes.Count-1;

            CambiarPersonaje();
            VibracionesControl.instancia.Vibrar(TipoVibracion.Seleccion);

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
                precioPersonaje_text.SetText("$"+personajes[enPersonaje].personaje.precio.ToString("00"));
            }
            candado_img.SetActive(!personajes[enPersonaje].comprado);
            //moneda_img.SetActive(!personajes[enPersonaje].comprado);
            //Debug.Log("Se cambio personaje...mandando sprites");
        }


        /// <summary>
        /// Busca un personaje de acuerdo a su nombre y cambia el sprite, Actualmente llamado por Score_Control
        /// 
        /// /// </summary>
        /// <param name="nombreBuscado">nombre del personaje</param>
        public void SetPersonaje(string nombreBuscado)
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
                    
                    Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_MoreCoffee);
                }
                else if(personajes[enPersonaje].personaje.nombre == "punkman" )
                {
                    
                    Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_DownTheSystem);
                }
                else if(personajes[enPersonaje].personaje.nombre == "naked_man")
                {
                   
                    Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_NoShame);

                }
            }
        }


        /// <summary>
        /// Llamado por Compras_Control.cs, se encarga de evaluar si se puede comprar el personaje o no
        /// </summary>
        public void DesbloquearPersonaje()
        {
            //Checamos si el personaje ya esta comprado, aunque esto no deberia pasar pues cuando esta comprado quitamos el boton de venta
            if (personajes[enPersonaje].comprado)
            {
                Debug.Log($"Intento comprar {personajes[enPersonaje].personaje.name} ya comprado");
                return;
            }
#if UNITY_IOS
            //Como no sabemos que tan seguro es que iOS guarde las cosas vamos a proceder a que las skins sean productos particulares

#else
            if (Score_Control.instancia.MonedasTotales >= personajes[enPersonaje].personaje.precio)
            {
                personajes[enPersonaje].comprado = true;
                //Score_Control.instancia.RestarMonedas(personajes[enPersonaje].personaje.precio);
                CambiarPersonaje();
                ///Tenemos que guardar en la nube y local que ya se compro el mono
                ///EN teoria no, porque ya implementamos salvado si quita o se va
             
                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_WorkWithStyle);


                GetComponent<Compras_Control>().PersonajeComprado_Callback();
                Save_Control.instancia.GuardarJuego();

            }

            else
            {
                Debug.Log("Sin monedas suficientes");
                //TODO: Agregar SFX incorrecto y animacion candado
                Sonido_Control.sonidos.ReproducirSonido_UI("errorBoton");

            }
#endif

        }

        public void DesbloquearPersonaje(string productoID)
        {
            foreach(PersonajesEnJuego personaje in personajes)
            {
                if(personaje.personaje.nombre == productoID)
                {
                    personaje.comprado = true;
                    Debug.Log($"Personaje {personaje.personaje.nombre} encontradod desbloqueando...");


                    Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_WorkWithStyle);
                   
                    CambiarPersonaje();

                    //GetComponent<Compras_Control>().PersonajeComprado_Callback();
                    //Save_Control.instancia.GuardarJuego();

                    break;
                }
            }
        }

        
        public void BuscarPrecioLocalizadoPersonajes()
        {

#if EM_UIAP

            foreach (PersonajesEnJuego personajeProducto in personajes)
            {
                //OJO lo busca por el nombre, no el ID, ya que EasyMobile se encarga de administrar le ID, 
                //personaje.nombre == easymobile.product.name
                ProductMetadata dataProducto = InAppPurchasing.GetProductLocalizedData(personajeProducto.personaje.nombre);
                if (dataProducto != null)
                {

                    personajeProducto.personaje.precio = (float)dataProducto.localizedPrice;
                    Debug.Log($"Producto {dataProducto.localizedTitle} localizado con precio {dataProducto.isoCurrencyCode} {dataProducto.localizedPrice}");
                }
            }
            Debug.Log("Productos con precios localizados terminados");
#endif
        }

        /// <summary>
        /// Llamado por ComprasControl(para asegurarse que IAP
        /// </summary>
        public void SetPersonajesComprados()
        {
            foreach(PersonajesEnJuego personaje in personajes)
            {
                if (personaje.personaje.nombre == "internDude" || personaje.personaje.nombre == "internGirl")
                {
                    personaje.comprado = true;
                }
                else
                {
                    personaje.comprado = InAppPurchasing.IsProductOwned(personaje.personaje.nombre);
                    //TODO:Al finalizar haz beardMan non-consumable pues te lo agarra como no comprado
                }
            }
            Debug.Log("Personajes desbloqueados por tienda termiandos");
        }

        public string ObtenerIDProductoPersonaje()
        {
            return personajes[enPersonaje].personaje.nombre;
        }

        /// <summary>
        /// Llamado para desbloquear personajes si el jugador los compro en base a los datos guardados(nube )//por el momento no tratar de poner los personajes guardados en local
        /// </summary>
        /// <param name="personajesEnNube"></param>
        public void VerificarPersonajesComprados(List<PersonajeSalvado> personajesEnNube)
        {
            if (personajesEnNube.Count <= 0)
            {
                Debug.Log("No habia personajes guardados en nube o local");
                return;
            }
            foreach (PersonajeSalvado personajeEnNube in personajesEnNube)
            {
                foreach (PersonajesEnJuego personajeEnJuego in personajes)
                {
                    if (personajeEnJuego.personaje.nombre == personajeEnNube.nombre)
                    {
                        //si el personajeEnJuego esta comprado entonces es verdad
                        //si es false, entonces checamos el estatus del personaje en la nube porque puede ser que si este comprado
                        //Esto es para que el sistema de guardado no se confunda
                        personajeEnJuego.comprado = personajeEnNube.comprado;
                        //personajeEnJuego.comprado = personajeEnJuego.comprado ? true : personajeEnNube.comprado;
                        Debug.Log($"Personaje{personajeEnJuego.personaje.nombre} comprado: {personajeEnJuego.comprado}");
                    }
                }
            }
            CambiarPersonaje();
            Debug.Log("Personajes estatus compra recuperados");
        }


        public void SetPersonajesEnNube(List<PersonajeSalvado> _personajesEnNube)
        {
            personajesEnNube = _personajesEnNube;
        }
    }

    [System.Serializable]
    public class PersonajesEnJuego
    {
        public PersonajeScriptable personaje;
        public bool comprado;
    }
}
