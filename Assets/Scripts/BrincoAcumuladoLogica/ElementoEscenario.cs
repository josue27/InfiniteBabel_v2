
using UnityEngine;


[CreateAssetMenu(fileName = "ElementoEscenario", menuName = "ElementoEscenario", order = 0)]
public class ElementoEscenario : ScriptableObject {

    public string nombre;
    public GameObject prefab;
    public int cantidadSpawn;
    public float duracionRecorrido;
    
}
