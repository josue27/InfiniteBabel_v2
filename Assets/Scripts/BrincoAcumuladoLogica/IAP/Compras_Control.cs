using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using System;
using UnityEngine.UI;
using TMPro;

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

    }

    private void PurchaseCompletedHandler(IAPProduct producto)
    {
        if (producto.Name == EM_IAPConstants.Product_Overtime_pay)
        {
            Debug.Log($"Se compro dinero: {producto.Name}");
            //Acumular Monedas;
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
        }
    }


    void Start()
    {
        isInitialized = InAppPurchasing.IsInitialized();

        foreach(IAPProduct producto in productos_DB)
        {
            Debug.Log("Producto: "+producto.Name);
        }
    }

    public void ComprarObjeto(Button boton)
    {
        foreach(Producto _producto in productos)
        {
            if(_producto.botonCompra == boton)
            {
                InAppPurchasing.Purchase(_producto.nombreID);
                break;

            }
        }

    }

    public void AbrirTienda()
    {
        if (!panelTienda)
            return;

        panelTienda.SetActive(!panelTienda.activeInHierarchy);
    }
   

}

[System.Serializable]
public class Producto
{
    public string nombreID;
    public bool comprado;
    public Button botonCompra;
    public IAPProduct producto;
}