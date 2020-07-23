using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoApertura : MonoBehaviour
{
    public float velociadMovimiento = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
        Invoke("DestruirObstaculo", 20.0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.back * (Time.deltaTime * velociadMovimiento));
    }


    void DestruirObstaculo()
    {
        
        //Destroy(this.gameObject);
    }
    private void FinJuego()
    {
       // velociadMovimiento = 0.0f;
    }
    private void OnDestroy()
    {
        Eventos_Dispatcher.eventos.JugadorPerdio -= FinJuego;
    }
}
