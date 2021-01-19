using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lean.Localization;
public class LocalizacionEspecial : MonoBehaviour
{
    public string enIdioma;

    [SerializeField]
    public TMP_FontAsset fontUniversal;
    public TMP_FontAsset fontJapones;
    public TMP_FontAsset fontRuso;
    public TMP_FontAsset fontChino;

    public TMP_Text[] textosACambiar;
    public List<FraseID> frases = new List<FraseID>();

    public TextoIdioma[] japonesFrases;
    void Start()
    {
        LeanLocalization.OnLocalizationChanged += LeanLocalization_OnLocalizationChanged;
    }

    private void LeanLocalization_OnLocalizationChanged()
    {
         enIdioma = LeanLocalization.CurrentLanguage;
        Debug.Log("Se cambio idioma "+ enIdioma);
        ActivarTextosIdioma(enIdioma);
    }
    private void OnDisable()
    {
        LeanLocalization.OnLocalizationChanged -= LeanLocalization_OnLocalizationChanged;

    }
    public void ActivarIdiomaEspecial(Idioma idiomaElegido)
    {
       
    }

    public void ActivarTextosIdioma(string idioma)
    {
        TMP_FontAsset fontElegido = fontUniversal;
        if(idioma == "Japanese")
        {
            if (!fontJapones)
            {
                Debug.Log("Cuidado font no encontrado");

            }
            else
                fontElegido = fontJapones;
            
        }
        else
        {
          
        }
        for (int i = 0; i < textosACambiar.Length; i++)
        {
            textosACambiar[i].font = fontElegido;
        }
       
    }
    public void ActivarTextosIdioma(Idioma idioma)
    {
        if(idioma == Idioma.JAPONES)
        {
            for (int i = 0; i < japonesFrases.Length; i++)
            {
                japonesFrases[i].textoOutput.font = fontJapones;
            }
        }
    }

}

[System.Serializable]
public class TextoIdioma
{
    public string ID;
    public Idioma idioma;
    public TMPro.TMP_Text textoOutput;
    [SerializeField]
    private string textoCambiado;
}

[System.Serializable]
public class FraseID
{
    public string ID;
    public string japones;
    public string ruso;
    public string chino;
}

[System.Serializable]
public enum Idioma
{
    ESPAÑOL,
    INGLES,
    JAPONES,
    RUSO,
    CHINO,
    FRANCES
}
