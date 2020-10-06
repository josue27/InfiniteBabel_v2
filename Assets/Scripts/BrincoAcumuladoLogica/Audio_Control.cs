using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Audio_Control : MonoBehaviour
{

    public Button musica_boton;
    public Slider musicaSlider;
    public AudioSource musicaSource;
    public Button sfx_boton;
    public Slider sfxSlider;
    public AudioSource sfxSoure;

    private void Awake() {
        musicaSlider.onValueChanged.AddListener(SliderMusica);
        sfxSlider.onValueChanged.AddListener(SliderSFX);
    }
    public void Musica()
    {
        if(!musicaSource) {Debug.Log("No hay source de musica"); return;}

        musicaSource.volume = 0.0f;
    }
    public void SFX()
    {
        if(!sfxSoure) {Debug.Log("No hay source de sfx"); return;}

        sfxSoure.volume = 0.0f;
    }

    public void SliderMusica(float v)
    {
        musicaSource.volume = v;
        Debug.Log($"Musica: {v}");
    }
    public void SliderSFX(float v)
    {
        sfxSoure.volume = v;

        Debug.Log($"SFX: {v}");
        
    }

}
