using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] GameObject mutedImage;
    float masterV = 1;
    bool doMute = false;

    public void MuteAudio()
    {
        doMute = !doMute;

        if (doMute)
        {
            mutedImage.SetActive(true);
            mixer.SetFloat("MasterVolume", 0);
        }
        else
        {
            mutedImage.SetActive(false);
            mixer.SetFloat("MasterVolume", masterV);
        }
    }

    public void ChangeMasterVolume(float v)
    {
        masterV = v;
        mixer.SetFloat("MasterVolume", v);
    }
    public void ChangeMusicVolume(float v) => mixer.SetFloat("MusicVolume", v);
    public void ChangeFxVolume(float v) => mixer.SetFloat("MusicVolume", v);
}
