using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class Audio_Control : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Button musica_boton;
    public Slider musicaSlider;
    public AudioSource musicaSource;
    public Button sfx_boton;
    public Slider sfxSlider;
    public AudioSource sfxSoure;

    const string MUSICA_ID = "musicaVolumen";
    const string SFX_ID = "sfxVolumen";

    private void Awake()
    {
        musicaSlider.onValueChanged.AddListener(SliderMusica);
        sfxSlider.onValueChanged.AddListener(SliderSFX);

        masterMixer.GetFloat(MUSICA_ID, out float v);
        musicaSlider.value = 1f;
        sfxSlider.value = 1f;


    }

    public void ToggleMusica()
    {
      

        if(musicaSlider.value <= musicaSlider.minValue)
        {
            SliderMusica(musicaSlider.maxValue);
            musicaSlider.value = musicaSlider.maxValue;
            return;
        }
        else
        {
            SliderMusica(musicaSlider.minValue);
            musicaSlider.value = musicaSlider.minValue;


        }
    }

    public void ToggleSFX()
    {


        if (sfxSlider.value <= sfxSlider.minValue)
        {
            SliderSFX(sfxSlider.maxValue);
            sfxSlider.value = sfxSlider.maxValue;

        }
        else
        {
            SliderSFX(sfxSlider.minValue);
            sfxSlider.value = sfxSlider.minValue;


        }
    }
 

    public void SliderMusica(float v)
    {
        //musicaSource.volume = v;
        masterMixer.SetFloat(MUSICA_ID, Mathf.Log(v)*20);
        Debug.Log($"Musica: {v}");
    }
    public void SliderSFX(float v)
    {
        sfxSoure.volume = v;
        masterMixer.SetFloat(SFX_ID, Mathf.Log( v)*20);

        Debug.Log($"SFX: {v}");
        
    }

}
