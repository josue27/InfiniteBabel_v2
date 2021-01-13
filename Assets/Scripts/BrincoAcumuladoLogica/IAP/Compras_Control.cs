using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using System;
using UnityEngine.UI;
using TMPro;
#if EM_UIAP
using UnityEngine.Purchasing;
#endif

namespace Brinco
{
    public class Compras_Control : MonoBehaviour
    {
        [SerializeField] private bool isInitialized;


        public List<Producto> productos = new List<Producto>();
        public IAPProduct[] productos_DB;


        [Header("Tienda UIX")]
        public GameObject panelTienda;

        [Header("UI Reinicio")]
        public GameObject botonReinicio;
        public GameObject botonNoAds;
        public GameObject panelNoAds_Tienda;
        [SerializeField]
        private Transform  posSinAds;
        

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
        }
        void Start()
        {
            isInitialized = InAppPurchasing.IsInitialized();

            //foreach (IAPProduct producto in productos_DB)
            //{
            //    Debug.Log("Producto: " + producto.Name);
            //}
            BuscarProductoLocalized();
            SeComproNoAds();
            //Esta autoinicializada, no se necesita
          //  InAppPurchasing.InitializePurchasing();
        }

        private void PurchaseFailedHandler(IAPProduct producto, string failureReason)
        {
            Debug.Log($"No se pudo procesar la compra de : {producto.Name} failureReason:{failureReason}");
            NativeUI.Alert("Error al comprar",$"The purchase {producto.Name} was not completed");

        }

        private void PurchaseCompletedHandler(IAPProduct producto)
        {
            if (producto.Name == EM_IAPConstants.Product_Cambio)
            {
                Debug.Log($"Se compro dinero: {producto.Name}");
                Score_Control.instancia.SumarMonedas(20,true);
              
                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_MakeItRain);

                AnimacionMonedaComprada();

            }
            else if(producto.Name == EM_IAPConstants.Product_Overtimepay)
            {
                Debug.Log($"Se compro dinero: {producto.Name}");
                Score_Control.instancia.SumarMonedas(50,true);

                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_MakeItRain);

                AnimacionMonedaComprada();

            }
            else if (producto.Name == EM_IAPConstants.Product_Promovido)
            {
                Debug.Log($"Se compro dinero: {producto.Name}");
                Score_Control.instancia.SumarMonedas(100,true);

                Logros_Control.instancia.DesbloquearLogro(EM_GameServicesConstants.Achievement_MakeItRain);

                AnimacionMonedaComprada();

            }
            else if (producto.Name == EM_IAPConstants.Product_RemoverAds)
            {
                this.GetComponent<Ad_Control>().RemoverAds(true);
                Debug.Log("se removieron los ads");
            }
            else
            {
                //Sin usar
                Debug.Log($"Se compro personaje: {producto.Name}");
                //Liberar personaje

            }

        }

        private void OnDisable()
        {
            InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
            InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
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
           
            if (!isInitialized)
            {
                Debug.Log("ComprasControl: InAppPurchasing no inicializado");
            }
            else
            {
                bool remover = InAppPurchasing.IsProductOwned(EM_IAPConstants.Product_RemoverAds);

                //GetComponent<Ad_Control>().RemoverAds(remover);
                if(remover)
                {
                    botonNoAds.gameObject.SetActive(false);
                    botonReinicio.transform.position = posSinAds.position;
                   // panelNoAds_Tienda.gameObject.SetActive(false);
                }

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
            if (isInitialized)
            {
                this.GetComponent<SeleccionPersonaje>().DesbloquearPersonaje();
            }
            else
            {
                NativeUI.Alert("Error", "No internet connection ");
                Sonido_Control.sonidos.ReproducirSonido_UI("errorBoton");

            }
        }
        public void AbrirTienda()
        {
            if (!isInitialized)
            {
                Debug.Log("Compras Control: GPS no inicializado");
                NativeUI.Alert("Error", "No internet connection");
                Sonido_Control.sonidos.ReproducirSonido_UI("errorBoton");
                return;
            }
            if (!panelTienda)
                return;

            panelTienda.SetActive(!panelTienda.activeInHierarchy);
        }


        //Inicia una secuencia exclusiva de UIAP, donde manda a llamar el listoad de los productos y nos da la informacion de manera localizada, util para precios,nombres y descripciones dependiendo del pais
        void BuscarProductoLocalized()
        {
#if EM_UIAP
            if (!isInitialized)
            {
                Debug.Log("Compras Control: GPS no inicializado");
                return;
            }
            
           
            
            foreach (Producto _producto in productos)
            {
                //OJO lo busca por el nombre, no el ID, ya que EasyMobile se encarga de administrar le ID
                ProductMetadata dataProducto = InAppPurchasing.GetProductLocalizedData(_producto.producto.Name);
                if(dataProducto != null)
                {

                    _producto.precioText.text = $"${dataProducto.localizedPrice}";
                    Debug.Log($"Producto {dataProducto.localizedTitle} localizado con precio {dataProducto.isoCurrencyCode} {dataProducto.localizedPrice}");
                }
            }
            Debug.Log("Productos con precios localizados terminados");
#endif
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