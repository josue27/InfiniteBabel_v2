using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Saved_Data 
{
    public int score;
    public int monedas;
    public List<PersonajeSalvado> personajes = new List<PersonajeSalvado>();


    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}


[System.Serializable]
public class PersonajeSalvado
{
    public string nombreID;
    public bool comprado;
}