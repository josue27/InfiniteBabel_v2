using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Personaje",menuName="Personaje/Nuevo Personaje")]
public class PersonajeScriptable : ScriptableObject
{
   public string nombre;
   public string nombreUI;
   public float precio;
   public string productoID;
   [Space(10)]
   public List<Sprite> sprites_idle;
   public List<Sprite> sprites_corriendo;
   public List<Sprite> sprites_brinco;
   public List<Sprite> sprites_caida;
   public List<Sprite> sprites_muerte;
}
