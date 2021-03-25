using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using System;
using UnityEngine.UI;
using TMPro;
using MoreMountains.NiceVibrations;
#if EM_UIAP
using UnityEngine.Purchasing;
#endif

namespace Brinco
{
    public class Compras_Control : MonoBehaviour
    {
        [SerializeField] private bool IAPisInitialized;

        
        public List<Producto> productos = new List<Producto>();
        public IAPProduct[] productos_DB;


        [Header("Tienda UIX")]
        public GameObject panelTienda;
        public GameObject panelCandadoAbierto;

        [Header("UI Reinicio")]
        public GameObject botonAdReward;
        public GameObject botonNoAds;
        public GameObject panelNoAds_Tienda;
        [SerializeField]
        private Transform  posSinAds;
        
        [Space(10)]
        [SerializeField]private GameObject panelCompraParent;
        [SerializeField]private GameObject panelRestauracionEnProceso;
        [SerializeField]private GameObject panelRestauracionFallo;
        [SerializeField]private GameObject botonRestauracionCompra;
        [SerializeField]
        private GameObject panelAnimacionMoneda;
        [SerializeField]
        private float tiempoAnimacion =1.9f;

        private void OnValidate()
        {
            productos_DB = InAppPurchasing.GetAllIAPProducts();

            SetProductos();
        }

        private void OnEnable()
        {
            InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
            InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
            //iOS
            InAppPurchasing.RestoreCompleted += RestoreCompletedHandler;
            InAppPurchasing.RestoreFailed += RestoreFailedHandler;
            InAppPurchasing.PurchaseDeferred += PurchaseDeferredHandler;



        }
        void Start()
        {
            IAPisInitialized = InAppPurchasing.IsInitialized();

            if(IAPisInitialized == false)
            {
                InAppPurchasing.InitializePurchasing();
            }
            //foreach (IAPProduct producto in productos_DB)
            //{
            //    Debug.Log("Producto: " + producto.Name);
            //}

            BuscarProductoLocalized();
            SeComproNoAds();
            PersonajesComprados();
            //Esta autoinicializada, no se necesita
          //  InAppPurchasing.InitializePurchasing();
          #if UNITY_IOS
            botonRestauracionCompra.SetActive(true);
          #else
            botonRestauracionCompra.SetActive(false);
          #endif


            
        }

        private void PurchaseFailedHandler(IAPProduct producto, string failureReason)
        {
            Debug.Log($"No se pudo procesar la compra de : {producto.Name} failureReason:{failureReason}");
            NativeUI.Alert("Error buying",$"The purchase {producto.Name} was not completed");
            PermisoDePadreOtorgado();

        }

        private void PurchaseCompletedHandler(IAPProduct producto)
        {
            
            if (producto.Name == EM_IAPConstants.Product_Americano)
            {
                Debug.Log($"Se compro cafe: {producto.Name}");
                Score_Control.instancia.SumarMonedas(50, true);

                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_MakeItRain);

                AnimacionMonedaComprada();

            }
            if (producto.Name == EM_IAPConstants.Product_Latte)
            {
                Debug.Log($"Se compro cafe: {producto.Name}");
                Score_Control.instancia.SumarMonedas(120, true);

                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_MakeItRain);

                AnimacionMonedaComprada();

            }else if (producto.Name == EM_IAPConstants.Product_Capuccino)
            {
                Debug.Log($"Se compro cafe: {producto.Name}");
                Score_Control.instancia.SumarMonedas(250, true);

                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_MakeItRain);

                AnimacionMonedaComprada();

            }
            else if(producto.Name == EM_IAPConstants.Product_beardMan ||
                    producto.Name == EM_IAPConstants.Product_naked_man ||
                    producto.Name == EM_IAPConstants.Product_punkman ||
                    producto.Name == EM_IAPConstants.Product_cyberGirl ||
                    producto.Name == EM_IAPConstants.Product_coffeeguy ||
                    producto.Name == EM_IAPConstants.Product_madScientist ||
                    producto.Name == EM_IAPConstants.Product_quietGuy ||
                    producto.Name == EM_IAPConstants.Product_xmasLover )
            {
                //Sin usar
                Debug.Log("Se compro" + producto.Name);
                this.GetComponent<SeleccionPersonaje>().DesbloquearPersonaje(producto.Name);
                MMVibrationManager.Haptic(HapticTypes.Success);

                //Liberar personaje

            }


            PermisoDePadreOtorgado();

        }


        

        private void OnDisable()
        {
            InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
            InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;

            InAppPurchasing.RestoreCompleted -= RestoreCompletedHandler;
            InAppPurchasing.RestoreFailed -= RestoreFailedHandler;
            InAppPurchasing.PurchaseDeferred -= PurchaseDeferredHandler;


        }

        private void PurchaseDeferredHandler(IAPProduct product)
        {
            //Activar UI para pedir permiso a un padre para activar
            panelCompraParent.SetActive(true);

        }
        public void PermisoDePadreOtorgado()
        {
            //Ocultar UI de permiso parental
            panelCompraParent.SetActive(false);

        }

        /// <summary>
        /// Acomoda los productos llenando la base de datos que se utilizara en runtime
        /// </summary>
        public void SetProductos()
        {
            for (int i = 0; i < productos_DB.Length; i++)
            {
                productos[i].nombreID = productos_DB[i].Name;
                productos[i].producto = productos_DB[i];
                //productos[i].precioText.text = productos_DB[i].Price;
            }
            BuscarProductoLocalized();
        }


     
       
        /// <summary>
        /// Verficia si se compro los ads o no, deshabilita
        /// el boton de compra de noAds en caso de estar comprado
        /// </summary>
        public void SeComproNoAds()
        {
           
            if (!IAPisInitialized)
            {
                Debug.Log("ComprasControl: InAppPurchasing no inicializado");
            }
            else
            {
                bool remover = InAppPurchasing.IsProductOwned(EM_IAPConstants.Product_RemoverAds);

               
                if(remover)
                {
                    botonNoAds.gameObject.SetActive(false);
                    botonAdReward.transform.position = posSinAds.position;
                   // panelNoAds_Tienda.gameObject.SetActive(false);
                }

            }
        }

        public void PersonajesComprados()
        {
            if(!IAPisInitialized)
            {
                Debug.Log("ComprasControl: InAppPurchasing no inicializado");

            }
            else
            {
                GetComponent<SeleccionPersonaje>().SetPersonajesComprados();
            }
        }
        /// <summary>
        /// Llamado por los botones en tiend UI para buscar un objeto a comprar, lo encuentras
        /// pasando el mismo boton que va a ser buscado
        /// </summary>
        /// <param name="boton"></param>
        public void ComprarObjeto(Button boton)
        {
            foreach (Producto _producto in productos)
            {
                if (_producto.botonCompra == boton)
                {
                    InAppPurchasing.Purchase(_producto.nombreID);
                    break;

                }
            }

        }
        /// <summary>
        /// Llamapo por botones en UI por si necesitamos ingresar o colocar botones de otra manera
        /// </summary>
        /// <param name="id">NombreId como aparece en la base de datos</param>
        public void ComprarObjeto(string id)
        {
            foreach (Producto _producto in productos)
            {
                if (_producto.nombreID == id)
                {
                    InAppPurchasing.Purchase(_producto.nombreID);
                    break;

                }
            }
        }

        
        public void ComprarPersonaje()
        {
            //PersonajeScriptable personaje = this.GetComponent<SeleccionPersonaje>().BuscarDataPersonaje();
            if (IAPisInitialized)
            {
                string IDProductoPersonaje = GetComponent<SeleccionPersonaje>().ObtenerIDProductoPersonaje();
                InAppPurchasing.Purchase(IDProductoPersonaje);

                //Deprecated
                // this.GetComponent<SeleccionPersonaje>().DesbloquearPersonaje();
            }
            else
            {
                NativeUI.Alert("Error", "No internet connection ");
                Sonido_Control.sonidos.ReproducirSonido_UI("errorBoton");
                MMVibrationManager.Haptic(HapticTypes.Warning);
                InAppPurchasing.InitializePurchasing();
                InAppPurchasing.InitializeSucceeded += ComprasActivadas;


            }
        }
        

        /// <summary>
        /// Debe ser utilizado por los dispositivos iOS
        /// </summary>
        /// <param name="_id"></param>
        public void ComprarPersonaje(string _id)
        {
            if(IAPisInitialized)
            {
                //Se tiene que pasar el _id del producto como viene en EasyMobile
                InAppPurchasing.Purchase(_id);
            }
            else
            {
                NativeUI.Alert("Error", "No internet connection ");
                Sonido_Control.sonidos.ReproducirSonido_UI("errorBoton");
                InAppPurchasing.InitializePurchasing();

                InAppPurchasing.InitializeSucceeded += ComprasActivadas;

            }
        }


        /// <summary>
        /// Llamado(por Seleccionpersonaje) cuando un personaje es comprado
        /// </summary>
        /// 
        [EasyButtons.Button]
        public void PersonajeComprado_Callback()
        {
            if (!panelCandadoAbierto)
                return;

            panelCandadoAbierto.SetActive(true);
            LeanTween.value(0, 1, 1.0f).setOnComplete(()=> {
                panelCandadoAbierto.SetActive(false);

            });

        }

        public void AbrirTienda()
        {
            if (!IAPisInitialized)
            {
                Debug.Log("Compras Control: GPS no inicializado");
                NativeUI.Alert("Error", "No internet connection");
                Sonido_Control.sonidos.ReproducirSonido_UI("errorBoton");
                InAppPurchasing.InitializePurchasing();
                InAppPurchasing.InitializeSucceeded += ComprasActivadas;
                return;
            }
            if (!panelTienda)
                return;

            panelTienda.SetActive(!panelTienda.activeInHierarchy);
            // Eventos_Dispatcher.eventos.OcultarBannerAdd_Call(panelTienda.activeInHierarchy);
        }

        /// <summary>
        /// Llamado cuando se pudo inicializar el modulo de Unity IAP, es activado mediante un EventListener
        /// Una vez llamado vuelve a buscar todo los personajes comprados y localizar
        /// </summary>
        private void ComprasActivadas()
        {
            InAppPurchasing.InitializeSucceeded -= ComprasActivadas;
            IAPisInitialized = InAppPurchasing.IsInitialized();
            BuscarProductoLocalized();
            SeComproNoAds();
            PersonajesComprados();
        }
#region  Restauracion Compras IOS
        public void ReataurarCompras_UI()
        {
            InAppPurchasing.RestorePurchases();
            //Activar UI para mostrar que estamos restaurando
            if(panelRestauracionEnProceso)
            {
                panelRestauracionEnProceso.SetActive(true);
            }
        }
        // Successful restoration handler
        void RestoreCompletedHandler()
        {
            Debug.Log("All purchases have been restored successfully.");
            //Desactivar UI y se debe entender que se completo la restauracion
            if(panelRestauracionEnProceso)
            {
                panelRestauracionEnProceso.SetActive(false);
            }
        }

        // Failed restoration handler
        void RestoreFailedHandler()
        {
            Debug.Log("The purchase restoration has failed.");
            //Mostrar UI donde se muestra error al tratar de restaurar compras
            
            panelRestauracionEnProceso?.SetActive(false);
            panelRestauracionFallo?.SetActive(false);

            
        }
        public void CerrarPanelFalloRestauracion()
        {
            panelRestauracionFallo?.SetActive(false);
        }


#endregion

        //Inicia una secuencia exclusiva de UIAP, donde manda a llamar el listoad de los productos y nos da la informacion de manera localizada, util para precios,nombres y descripciones dependiendo del pais
        void BuscarProductoLocalized()
        {

            if (!IAPisInitialized)
            {
                Debug.Log("Compras Control: UIAP no inicializado");
                return;
            }
            
           
            
            foreach (Producto _producto in productos)
            {
                //OJO lo busca por el nombre, no el ID, ya que EasyMobile se encarga de administrar le ID
                ProductMetadata dataProducto = InAppPurchasing.GetProductLocalizedData(_producto.producto.Name);
                if(dataProducto != null)
                {
                    if(_producto.precioText)
                        _producto.precioText.text = $"${dataProducto.localizedPrice}";
                    Debug.Log($"Producto {dataProducto.localizedTitle} localizado con precio {dataProducto.isoCurrencyCode} {dataProducto.localizedPrice}");
                }
            }
            Debug.Log("Productos Tienda con precios localizados terminados");
            GetComponent<SeleccionPersonaje>().BuscarPrecioLocalizadoPersonajes();

        }


        public void AnimacionMonedaComprada()
        {
            StartCoroutine(AnimacionMonedaComprada_Rutina());
            
        }
        IEnumerator AnimacionMonedaComprada_Rutina()
        {
            panelAnimacionMoneda.SetActive(true);

            yield return new WaitForSeconds(tiempoAnimacion);
            panelAnimacionMoneda.SetActive(false);


        }

    }

    [System.Serializable]
    public class Producto
    {
        public string nombreID;
        public bool comprado;
        public Button botonCompra;
        public TMP_Text precioText;
        public IAPProduct producto;
    }
}