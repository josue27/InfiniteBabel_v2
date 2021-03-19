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

    private const string paredTag = "pared";
    private const string paredTNTTag = "paredTNT";

    [Space(10)]
    [Header("VFX")]
    public GameObject explosion_vfx;

  
    void Start()
    {
       // SetObstaculo();
       Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
    }

    // Update is called once per frame
    void Update()
    {
        // #if UNITY_EDITOR
    
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     SetObstaculo();

        // }
        // #endif

      //  this.transform.Translate(Vector3.back * (Time.deltaTime * velociadMovimiento));

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
    public void SetObstaculo(int numeroADescontar, float _nuevaVelocidad,bool spawnTNT)
    {
        posicionesLibres.Clear();
        foreach(Transform posicion in posiciones)
        {
            posicion.gameObject.SetActive(true);
            posicion.transform.GetChild(1).gameObject.SetActive(false);
            posicion.transform.GetChild(0).gameObject.SetActive(true);
            BoxCollider[] collidersInit = posicion.GetComponents<BoxCollider>();
            for (int i = 0; i < collidersInit.Length; i++)
            {
                collidersInit[i].enabled = true;
            }

            //  posicion.transform.tag = paredTag;

        }
        int r = Random.Range(1,posiciones.Length);
        

        posiciones[r].gameObject.SetActive(false);
        posicionesLibres.Add(posiciones[r]);

        //Esto es para asegurarnos de que no estamos dando un numero mas alto de las posiciones disponibles, si lo es le da el Length del array posiciones
        //de lo contrario le permite quedarse con su valor, esta condicional es inecesaria podria ser solo un if
        numeroADescontar = numeroADescontar > posiciones.Length-1 ? posiciones.Length-1 : numeroADescontar;

        int posBuscada = 1;
        //SUGERENCIA: hacer clamp en este algoritmo??
        for(int i = 1; i < numeroADescontar;i++)
        {

            //Esto es para evitar que la caja del piso desaparezca, siempre estara presente pues ej: si
            /*
             *  si r-i = 3 > 1
             *      entonces podemos quitar esa caja y la guardamos a posiciones libres
             *  si no quiere decir que el numero era 1 porque 1-0 = 1 || 1-1=0 y necesitamos que sea mayor a 1
             *      si r+i = 4 y 4 es < posiciones.Length
             *          entonces podemos quitar esa caja
             * si no quiere decir que el numero era la ultima posicion de length y eso esta mal y brincamos
             *      si r +(i-posBucada) = 1 entonces es menor a posiciones.Length   
             */
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

        //Activar dinamitas
        if (spawnTNT)
        {
            int posDinamita = Random.Range(0, posiciones.Length);


            while (posiciones[posDinamita].gameObject.activeInHierarchy == false)
            {
                posDinamita = Random.Range(0, posiciones.Length);

            }

            posiciones[posDinamita].transform.GetChild(1).gameObject.SetActive(true);
            BoxCollider[] colliders = posiciones[posDinamita].GetComponents<BoxCollider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
            posiciones[posDinamita].transform.GetChild(0).gameObject.SetActive(false);
            //posiciones[posDinamita].transform.tag = paredTNTTag;

            Debug.Log("Dinamita en posicion: " + posiciones[posDinamita]);
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

    private void FinJuego()
    {

        velociadMovimiento = 0f;
        return;
        //TODO:Causo un crash a UNITY
        //  LeanTween.value(velociadMovimiento,0f,1.0f).setOnUpdate((float val)=>
        //  {
        //      velociadMovimiento = val;
        //  });
         
        
    }

    public void ExplotarTNT(Transform cajaTNT_pos)
    {
        if (!explosion_vfx)
            return;

        explosion_vfx.transform.position = cajaTNT_pos.position;
        StartCoroutine(ExplotarTNT_Rutina());
        
     
    }

    IEnumerator ExplotarTNT_Rutina()
    {
        explosion_vfx.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        explosion_vfx.SetActive(false);
    }
}
