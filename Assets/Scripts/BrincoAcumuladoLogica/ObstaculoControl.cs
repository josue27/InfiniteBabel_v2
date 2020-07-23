using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoControl : MonoBehaviour
{   
    public float velociadMovimiento = 1.0f;

    public Transform[] posiciones;
    public List<Transform> posicionesLibres = new List<Transform>();
    // Start is called before the first frame update
    int posElegida;

    public Moneda_Control moneda;

    
    void Start()
    {
       // SetObstaculo();
     //  Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
    
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetObstaculo();

        }
        #endif

        this.transform.Translate(Vector3.back * (Time.deltaTime * velociadMovimiento));

    }

    public void SetObstaculo()
    {
        foreach(Transform posicion in posiciones)
        {
            posicion.gameObject.SetActive(true);
        }
        int r = Random.Range(1,posiciones.Length);
        

        posiciones[r].gameObject.SetActive(false);
        posicionesLibres.Add(posiciones[r]);

        if((r-1)>1)
        {
             posiciones[r-1].gameObject.SetActive(false);
             posicionesLibres.Add(posiciones[r-1]);

            Debug.Log("posiciones:"+r+(r-1).ToString());
        }else if((r+1)<posiciones.Length)
        {
             posiciones[r+1].gameObject.SetActive(false);
             posicionesLibres.Add(posiciones[r+1]);
            Debug.Log("posiciones:"+r+(r+1).ToString());


        }
        moneda.gameObject.SetActive(false);
        
    }
    public void SetObstaculo(int numeroADescontar, float _nuevaVelocidad)
    {
        posicionesLibres.Clear();
        foreach(Transform posicion in posiciones)
        {
            posicion.gameObject.SetActive(true);
        }
        int r = Random.Range(1,posiciones.Length);
        

        posiciones[r].gameObject.SetActive(false);
        posicionesLibres.Add(posiciones[r]);


        numeroADescontar = numeroADescontar > posiciones.Length-1 ? posiciones.Length-1 : numeroADescontar;
        int posBuscada = 1;
        for(int i = 1;i<numeroADescontar;i++)
        {
            if((r-i)>1)
            {
                posiciones[r-i].gameObject.SetActive(false);
                posicionesLibres.Add(posiciones[r-i]);

                //Debug.Log("posiciones:"+r+(r-1).ToString());
            }else if((r+i)<posiciones.Length)
            {
                posiciones[r+i].gameObject.SetActive(false);
                posicionesLibres.Add(posiciones[r+i]);
               // Debug.Log("posiciones:"+r+(r+1).ToString());


            }
            else if(r+ (i-posBuscada)<posiciones.Length)
            {
                posiciones[r+(i-posBuscada)].gameObject.SetActive(false);
                posicionesLibres.Add(posiciones[r+(i-posBuscada)]);
               // Debug.Log("posiciones:"+r+(r+1).ToString());
                 posBuscada++;

            }
            else if(r - (i-posBuscada)>1)
            {
                posiciones[r-(i-posBuscada)].gameObject.SetActive(false);
                posicionesLibres.Add(posiciones[r-(i-posBuscada)]);
              //  Debug.Log("posiciones:"+r+(r+1).ToString());
                 posBuscada++;
            }
           
        }

        moneda.gameObject.SetActive(false);
         velociadMovimiento = _nuevaVelocidad;
    }
    public void SetObstaculo(int numEspacios)
    {
        foreach(Transform pos in posiciones)
        {
            pos.gameObject.SetActive(false);
            
        }
         posiciones[0].gameObject.SetActive(true);
         int r = Random.Range(1,posiciones.Length);//1 a posiciones.Lenght ignorando a 0

         posiciones[r].gameObject.SetActive(true);

        int posicionesActivadas = 1;//iniciamos en 1 porque la posicion 0 siempre va  estar activada
        for(int j = 1; j < numEspacios; j++)
        {
            if(r+j < posiciones.Length)
            {
                posiciones[r+j].gameObject.SetActive(true);
                posicionesActivadas++;
                
            }else if( r-j > 0)
            {
                posiciones[r-j].gameObject.SetActive(true);
                posicionesActivadas++;
            }
        }
    }
    public void ActivarMoneda()
    {

        if(posicionesLibres.Count == 0)
        return;

        int r = Random.Range(0,posicionesLibres.Count);


        moneda.transform.position = posicionesLibres[r].position;
        moneda.gameObject.SetActive(true);

    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("zonaMuerte"))
        {
            moneda.gameObject.SetActive(false);
            posicionesLibres.Clear();
            this.gameObject.SetActive(false);
        }
    }

     private void OnDestroy()
    {
        ///Eventos_Dispatcher.eventos.JugadorPerdio -= FinJuego;
    }
}
