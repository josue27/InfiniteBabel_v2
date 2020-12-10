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

        private void PurchaseFailedHandler(IAPProduct producto)
        {
            Debug.Log($"No se pudo procesar la compra de : {producto.Name}");
            NativeUI.Alert("Error al comprar",$"La compra de{producto.Name} no se pudo completar");

        }

        private void PurchaseCompletedHandler(IAPProduct producto)
        {
            if (producto.Name == EM_IAPConstants.Product_Cambio)
            {
                Debug.Log($"Se compro dinero: {producto.Name}");
                Score_Control.instancia.SumarMonedas(20);

            } else if (producto.Name == EM_IAPConstants.Product_RemoverAds)
            {
                this.GetComponent<Ad_Control>().noAdIntermedio = true;
                Debug.Log("se removieron los ads");
            }
            else
            {
                Debug.Log($"Se compro personaje: {producto.Name}");
                //Liberar personaje

            }

        }

        private void OnDisable()
        {
            InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
            InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
        }

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


        void Start()
        {
            isInitialized = InAppPurchasing.IsInitialized();

            //foreach (IAPProduct producto in productos_DB)
            //{
            //    Debug.Log("Producto: " + producto.Name);
            //}
            BuscarProductoLocalized();
        }

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


        public void ComprarPersonaje()
        {
            //PersonajeScriptable personaje = this.GetComponent<SeleccionPersonaje>().BuscarDataPersonaje();
            if(RuntimeManager.IsInitialized())
                 this.GetComponent<SeleccionPersonaje>().DesbloquearPersonaje();

        }
        public void AbrirTienda()
        {
            if (!isInitialized)
            {
                Debug.Log("Compras Control: GPS no inicializado");
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