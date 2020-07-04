using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoControl : MonoBehaviour
{
    public Transform[] posiciones;
    public List<Transform> posicionesLibres = new List<Transform>();
    // Start is called before the first frame update
    int posElegida;

    public Moneda_Control moneda;

    
    void Start()
    {
       // SetObstaculo();
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
}
