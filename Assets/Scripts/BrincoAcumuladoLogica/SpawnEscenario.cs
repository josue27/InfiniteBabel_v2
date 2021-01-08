using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brinco;
public class SpawnEscenario : MonoBehaviour
{

    public static SpawnEscenario instancia;

    //TODO: Crear scriptble object ElementoEscenario
    public List<Elemento> elementos = new List<Elemento>();

    public List<ElementoEscenario_Control> elementosIniciales = new List<ElementoEscenario_Control>();
    public bool spawnear;
    public Dictionary<string,Queue<GameObject>> elementosDictionary = new Dictionary<string, Queue<GameObject>>();
    [Header("Sprite Piso")]
    public SpriteRenderer spritePiso;
    public Material spritePiso_material;
    public float velocidadPiso = 4.3f;
    [Header("Sprite Fondo")]
    public SpriteRenderer spriteFondo;
    public Material spriteFondo_material;
    public float velocidadFondo = 0.03f;
    [Header("Sprite Lamparas")]
    public SpriteRenderer spriteLampara;
    public Material spriteLampara_material;
    public float velocidadLampara = 0.23f;


    /// <summary>
    /// Esto NO deberia estar aqui pero gueno
    /// </summary>
    [Space(10)]
    [Header("Obstaculo Cinta Rota")]
    public bool spawnearCintasRotas;
    [SerializeField]bool cintaRotaSpawneada;
    public int casosProbables = 20;
    public int casosFavorables = 3;

    [Space(10)]
    [Header("Obstaculos Caja")]
    public float intervaloTiempo = 3.0f;
    public int probabilidadMoneda = 5;
    [SerializeField] private int cantidadCajas;
    [SerializeField] private float alturaPrevia;
    [SerializeField] private bool spawnTNT;
    [SerializeField] private int sigObstaculo;
    [SerializeField] private int cintasPasadas;
    public int minSigObstaculo = 3;
    public int maxSigObstaculo = 6;
        
    void OnValidate()
    {
        foreach(Elemento elementoEnLista in elementos)
        {
            elementoEnLista.Set();
        }
    }
    public void Start()
    {
        instancia = this;
        SpawnElementos();
        Eventos_Dispatcher.eventos.InicioJuego +=   InicioJuego; 
        Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;
        Eventos_Dispatcher.Reinicio += Reinicio;
        
        if(spritePiso)
            spritePiso_material = spritePiso.material;
        if(spriteFondo)
            spriteFondo_material = spriteFondo.material;
        if (spriteLampara)
            spriteLampara_material = spriteLampara.material;
    }
 
    void Update()
    {
        if(spawnear)
        {
            for (int i = 0; i < elementos.Count; i++)
            {   
                if(Time.time>= elementos[i].sigSpawn)
                {
                  elementos[i].sigSpawn = Time.time+elementos[i].rateSpawn;
                  ActivarObjeto(elementos[i].nombre); 
                }
            }
        }
    }
    public void InicioJuego()
    {
        //InvokeRepeating("ActivarObjeto",1.0f,3.0f);
        // for (int i = 0; i < elementos.Count; i++)
        // {
        //     elementos[i].sigSpawn = Time.time+elementos[i].rateSpawn;
        // }
        ActivarObjetoInicial("paredIntermedia");
        ActivarObjetoInicial("cintaTransportadora");
        spritePiso_material.SetFloat("_Velocidad",velocidadPiso);
        spriteFondo_material.SetFloat("_Velocidad",velocidadFondo);
        spriteLampara_material.SetFloat("_Velocidad",velocidadLampara);

        spawnear = true;
    }
    public void SpawnElementos()
    {
        // foreach(ElementoEscenario elemento in elementosEscenario)
        // {
        //     Queue<GameObject> objetos = new Queue<GameObject>();

        //     for(int i = 0;i<elemento.cantidadSpawn;i++)
        //     {
        //         GameObject objeto = Instantiate(elemento.prefab,elemento.posInicial.position,elemento.posInicial.rotation);
        //         objeto.gameObject.SetActive(false);
        //         objeto.transform.name = elemento.nombre+"_"+i.ToString("00");
        //         objetos.Enqueue(objeto);

        //     }
        //     elementosDictionary.Add(elemento.nombre,objetos);
        // }

        foreach(Elemento elementoEnLista in elementos)
        {
            elementoEnLista.nombre = elementoEnLista.elemento.nombre;
            elementoEnLista.objetos = new Queue<GameObject>();
            for(int i = 0; i<elementoEnLista.elemento.cantidadSpawn; i++)
            {
                GameObject objeto = Instantiate(elementoEnLista.elemento.prefab,elementoEnLista.posInicial.position,elementoEnLista.posInicial.rotation);
                objeto.gameObject.SetActive(false);
                objeto.gameObject.name = elementoEnLista.nombre+"_"+i.ToString("00");
                elementoEnLista.objetos.Enqueue(objeto);

            }
        }
    }

    public void ActivarObjeto()
    {
        ActivarObjeto("vigaVertical");
    }

    public void ActivarObjeto(string nombreObjeto)
    {
        GameObject objetoActivar;
        foreach(Elemento elemenoEnLista in elementos){

            if(elemenoEnLista.nombre.Equals(nombreObjeto))
            {
                objetoActivar = elemenoEnLista.objetos.Dequeue();
                objetoActivar.transform.position = elemenoEnLista.posInicial.position;
                objetoActivar.gameObject.SetActive(true);
                objetoActivar.GetComponent<ElementoEscenario_Control>().Mover(elemenoEnLista.posFinal.position,elemenoEnLista.duracionRecorrido);


                ///Spawn de cintra rota
                if(objetoActivar.CompareTag ("piso") && Master_Level._masterBrinco.estadoJuego != EstadoJuego.tutorial)
                {
                    if (cintasPasadas == sigObstaculo)
                    {
                        ActivarObstaculo(objetoActivar);
                        sigObstaculo = Random.Range(minSigObstaculo, maxSigObstaculo);
                        cintasPasadas = 0;
                    }else
                        cintasPasadas++;

                }


                elemenoEnLista.objetos.Enqueue(objetoActivar);


               
               
                break;
            }
        }
        
    }

    /// <summary>
    /// Regresa si debemos Spawnear una cinta transportadora rota mediante probabilidad,
    /// tambien incluye una logica que podrai ser mejor pero consiste en que:
    /// si se spawneo una cintaRota anteriormente 
    ///     regresa false y ademas declaramos que ya no se spawneao una cintaRota, de esa manera hacemos un salto por lo menos de 1 vez
    ///     Si no se spawnea una cintaRota entonces hacemos la loteria
    ///         si sale de cierto rango Random de enteros se regresa true, de lo contrario es false
    /// </summary>
    /// <returns></returns>
    bool TransportadoraRota()
    {
        if (cintaRotaSpawneada)
        {
            cintaRotaSpawneada = false;
            return false;
        }

        int r = Random.Range(0, casosProbables);
        if(r <= casosFavorables)
        {
            cintaRotaSpawneada = true;
            return true;
        }

        return false;
    }

    public void ActivarObjetoInicial(string nombreObjeto)
    {
        GameObject objetoActivar;
        foreach(Elemento elemenoEnLista in elementos){

            if(elemenoEnLista.nombre.Equals(nombreObjeto))
            {
                if(elemenoEnLista.objetoInical.Length <= 0)
                    return;
                
                foreach(GameObject objetoI in elemenoEnLista.objetoInical)
                {
                // objetoActivar = elemenoEnLista.objetoInical;
                    //objetoActivar.transform.position = elemenoEnLista.posInicial.position;
                    objetoI.gameObject.SetActive(true);
                    
                    float distanciaPos =   elemenoEnLista.posFinal.position.z-elemenoEnLista.posInicial.position.z;
                    //Debug.Log("Distancia:"+distanciaPos);
                    Vector3 posFinalInicial = objetoI.transform.position;
                    posFinalInicial.z += distanciaPos;
                    objetoI.GetComponent<ElementoEscenario_Control>().Mover(posFinalInicial,elemenoEnLista.duracionRecorrido);
                    //elemenoEnLista.objetos.Enqueue(objetoActivar);
                    //Debug.Log("objeto inicial activado");
                    elementosIniciales.Add(objetoI.GetComponent<ElementoEscenario_Control>());

                }
                Debug.Log("objetos iniciales activado");

                
            }
        }
        
    }

    private void FinJuego()
    {
        spawnear = false;
        
        foreach(Elemento elementoEnLista in elementos)
        {
            foreach(GameObject objeto in elementoEnLista.objetos)
            {
                if(objeto.activeInHierarchy)
                {
                    objeto.GetComponent<ElementoEscenario_Control>().Parar();
                }
            }
        }
        foreach(ElementoEscenario_Control elementoInicial in elementosIniciales)
        {
            elementoInicial.Parar();
        }
        spritePiso_material.SetFloat("_Velocidad",0f);
        spriteFondo_material.SetFloat("_Velocidad",0f);
        spriteLampara_material.SetFloat("_Velocidad",0f);

    }



    #region Obstaculos

    public void ActivarObstaculo(GameObject objetoActivar)
    {

        if(spawnearCintasRotas && TransportadoraRota())
        {
            objetoActivar.transform.GetChild(0).gameObject.SetActive(false);
            objetoActivar.transform.GetChild(2).gameObject.SetActive(true);
            Debug.Log("TRANSPORTADORA ROTA");
        }
        else
        {
            objetoActivar.GetComponent<ElementoEscenario_Control>().ActivarObstaculo(cantidadCajas,spawnTNT,LoteriaMoneda());
            Debug.Log("OBSTACULO SPAWNEADO");
        }
       
    }

    public void SetDificultad(int _cantidadCajas, float _rateSpawn, int _probabilidadMoneda, bool _spawnTNT,bool _spawnCintaRota,int _minSigObstaculo,int _maxSigObstaculo)
    {

        cantidadCajas = _cantidadCajas;
        intervaloTiempo = _rateSpawn > 0 ? _rateSpawn : intervaloTiempo;//asgurarse que no de 0
        probabilidadMoneda = _probabilidadMoneda;
        spawnTNT = _spawnTNT;
        spawnearCintasRotas = _spawnCintaRota;

        minSigObstaculo = _minSigObstaculo;
        maxSigObstaculo= _maxSigObstaculo;
    }

    public bool LoteriaMoneda()
    {
        bool spawnearMoneda = false;

        int r = Random.Range(0, 10);

        // si r > 7 la probabilidad de que r sea mayor es del 30%?
        // si r < 7 la probabilidad de que r sea mayor es del 70%?

        if (r > 10 - probabilidadMoneda)// %50?    
        {
            spawnearMoneda = true;
            Debug.Log("Spaneando moneda");
        }
        return spawnearMoneda;
    }


    #endregion
    public void Reinicio()
    {
        
    }

}


[System.Serializable]
public class Elemento
{
    
    public ElementoEscenario elemento;
    public string nombre;
    public int cantidadSpawn;
    public float duracionRecorrido;
    public Transform posInicial;
    public Transform posFinal;
    
    public GameObject[] objetoInical;
    public float rateSpawn = 3.0f;
    public bool rateRandom;
    public bool minRate, maxRate;
    [SerializeField]
    public float sigSpawn;
    public Queue<GameObject> objetos;


    public void Set()
    {
        if(!elemento)
        return;

        nombre = elemento.nombre;
        cantidadSpawn = elemento.cantidadSpawn;
        duracionRecorrido = elemento.duracionRecorrido;
    }

}
