using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brinco;

public class SpawnObstaculos_Apertura : MonoBehaviour
{

    public static SpawnObstaculos_Apertura spawner;
  
    public float intervaloTiempo = 3.0f;
    public float velocidadObstaculos = 4.0f;
    public int probabilidadMoneda = 5;
    [SerializeField]
    float sigSpawn ;
    public bool puedeSpawnear;
    public GameObject obstaculos_prefab;

    public GameObject obstaculos_prefab_B;
    public GameObject obstaculos_prefab_C;
    public float maxY=6.0f, minY=1.0f;

    [SerializeField ]private int cantidadCajas;
    [SerializeField] private float alturaPrevia;
    // Start is called before the first frame update
    public int cantidadObstaculos;
    public List<ObstaculoControl> obstaculos = new List<ObstaculoControl>();

    public List<Pool> pools = new List<Pool>();
    public Dictionary<string,Queue<GameObject>> obstaculosB;

     /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        obstaculosB = new Dictionary<string, Queue<GameObject>>();
        spawner = this;
    }
    void Start()
    {
       

       
        Eventos_Dispatcher.eventos.JugadorPerdio += FinJuego;

        Eventos_Dispatcher.CambioVelocidad += CambioVelocidad;

        //SpawnearPool();
        LlenarDiccionario();
        
    }
     private void Update()
    {
        if(puedeSpawnear)
        {
            if(Time.time >= sigSpawn)
            {
                sigSpawn = Time.time + intervaloTiempo;
                ActivarObstaculo("obstaculoA");
                
            }
        }

        
    }

    
    void SpawnearPool()
    {
       // Vector3 nuevasPos = new Vector3(this.transform.position.x, RandomPosY(), this.transform.position.z);
        for(int i = 0;i<cantidadObstaculos;i++)
        {
             GameObject obs = Instantiate(obstaculos_prefab, this.transform.position, Quaternion.identity) as GameObject;
             obs.gameObject.SetActive(false);
             obs.transform.name = "obstaculo_"+i;
             obstaculos.Add(obs.GetComponent<ObstaculoControl>());
        }
        
        
    }

    public void LlenarDiccionario()
    {
        foreach(Pool pool in pools)
        {
            Queue<GameObject> objetos = new Queue<GameObject>();

            for(int i = 0;i<pool.cantidad;i++)
            {
                GameObject objeto = Instantiate(pool.objeto);
                objeto.SetActive(false);
                objeto.transform.name = pool.nombre+"_"+i;
                objetos.Enqueue(objeto);
            }

             obstaculosB.Add("obstaculoA",objetos);
            
        }
    }

    public void ActivarObstaculo()
    {
        foreach(ObstaculoControl obstaculo in obstaculos)
        {
            if(!obstaculo.gameObject.activeInHierarchy)
            {
                obstaculo.transform.position = this.transform.position;
                obstaculo.gameObject.SetActive(true);
                obstaculo.SetObstaculo();
                if(LoteriaMoneda())
                {
                    obstaculo.GetComponent<ObstaculoControl>().ActivarMoneda();    
                }
                break;
            }

        }
    }

    public void LlamarObstaculo()
    {
        ActivarObstaculo("obstaculoA");
    }

    public GameObject ActivarObstaculo(string tag)
    {

        if(!obstaculosB.ContainsKey(tag))
        {
            return null;
        }

        GameObject obstaculo = obstaculosB[tag].Dequeue();
        obstaculo.transform.position = this.transform.position;
        obstaculo.gameObject.SetActive(true);
        obstaculo.GetComponent<ObstaculoControl>().SetObstaculo(cantidadCajas,velocidadObstaculos);
        if(LoteriaMoneda())
        {
           obstaculo.GetComponent<ObstaculoControl>().ActivarMoneda();    
        }

        obstaculosB[tag].Enqueue(obstaculo);
        return obstaculo;

    }
    
    public void SetDificultad(float _velocidadObstaculos,int _cantidadCajas, float _rateSpawn)
    {
        velocidadObstaculos = _velocidadObstaculos;
        cantidadCajas =_cantidadCajas;
        intervaloTiempo = _rateSpawn > 0 ? _rateSpawn : intervaloTiempo;
        
        
    }
     public void SetDificultad(int _cantidadCajas, float _rateSpawn,int _probabilidadMoneda)
    {
        
        cantidadCajas =_cantidadCajas;
        intervaloTiempo = _rateSpawn > 0 ? _rateSpawn : intervaloTiempo;//asgurarse que no de 0
        probabilidadMoneda = _probabilidadMoneda;
        
    }

    public void EmpezoJuego()
    {
       // InvokeRepeating("LlamarObstaculo", 0.1f, intervaloTiempo);
        ActivarObstaculo("obstaculoA");
        sigSpawn = Time.time + intervaloTiempo;
        puedeSpawnear = true;
    }

    void FinJuego()
    {
        puedeSpawnear = false;
        DesactivarColisiones();
        CancelInvoke();
    }

    ///<sumary>
    ///En deshuso porque ahora se maneja por cubos para quitar
    ///</sumary>
    float RandomPosY()
    {
        float rand = Random.Range(minY, maxY);
        while(rand == alturaPrevia)
        {
            rand = Random.Range(minY, maxY);
        }
        alturaPrevia = rand;
        return rand;
    }

    public bool LoteriaMoneda()
    {
        bool spawnearMoneda = false;

        int r = Random.Range(0,10);

        // si r > 70 la probabilidad de que r sea mayor es del 30%?
        // si r < 70 la probabilidad de que r sea mayor es del 70%?

        if(r > 10-probabilidadMoneda)// %50?    
        {
            spawnearMoneda = true;
            Debug.Log("Spaneando moneda");
        }
        return spawnearMoneda;
    }
    private void OnDestroy()
    {

   
        Eventos_Dispatcher.eventos.JugadorPerdio -= FinJuego;
        Eventos_Dispatcher.CambioVelocidad -=CambioVelocidad;
    }

    public void CambioVelocidad(float nuevaVelocidad)
    {
        velocidadObstaculos = nuevaVelocidad;

    }

    public void DesactivarColisiones()
    {
        foreach(GameObject obstaculo in obstaculosB["obstaculoA"])
        {
            if(obstaculo.activeInHierarchy)
            {
                BoxCollider[] colliders = obstaculo.GetComponentsInChildren<BoxCollider>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                }
           }
        }
    }
}

[System.Serializable]
public class Pool{

    public string nombre;
    public GameObject objeto;
    public int cantidad;

}