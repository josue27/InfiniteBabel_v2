using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PisoControl : MonoBehaviour
{
    public GameObject prefab_piso;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            PisoSpawn._pisoSpawn.SpawnearPiso();
        }
    }

   
}
