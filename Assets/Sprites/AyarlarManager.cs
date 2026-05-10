using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AyarlarManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider sesSlider;

    public void SesAyarla(float volume)
    {
        // Slider 0.0001 ile 1 arasýnda olmalý (Logaritmik hesaplama için)
        audioMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
    }
}