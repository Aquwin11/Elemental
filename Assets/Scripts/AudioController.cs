using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioMixer mixer;

    private float manualMusicVolume = 0;
    private float manualUIVolume = 0;
    private float manualSFXVolume = 0;
    private float manualMasterVolume = 0;
    private bool manualMute;

    private void Start()
    {
        UpdateAudioLevels();
    }

    public void UpdateAudioLevels()
    {
        if (PlayerPrefs.HasKey("SettingsAudioMute") && PlayerPrefs.GetInt("SettingsAudioMute") == 1)
        {
            manualMute = true;
            mixer.SetFloat("UIVolume", -80);
            mixer.SetFloat("MusicVolume", -80);
            mixer.SetFloat("SFXVolume", -80);
            mixer.SetFloat("MasterVolume", -80);
            return;
        }
        else manualMute = false;
        if (PlayerPrefs.HasKey("SettingsAudioMaster"))
        {
            float v = PlayerPrefs.GetFloat("SettingsAudioMaster");
            mixer.SetFloat("MasterVolume", Mathf.Log10(v) * 20);
        }
        if (PlayerPrefs.HasKey("SettingsAudioUI"))
        {
            float v = PlayerPrefs.GetFloat("SettingsAudioUI");
            mixer.SetFloat("UIVolume", Mathf.Log10(v) * 20);
        }
        if (PlayerPrefs.HasKey("SettingsAudioMusic"))
        {
            float v = PlayerPrefs.GetFloat("SettingsAudioMusic");
            mixer.SetFloat("MusicVolume", Mathf.Log10(v) * 20);
        }
        if (PlayerPrefs.HasKey("SettingsAudioSFX"))
        {
            float v = PlayerPrefs.GetFloat("SettingsAudioSFX");
            mixer.SetFloat("SFXVolume", Mathf.Log10(v) * 20);
        }
    }

    public void SetMusicVolumeManual(float v)
    {
        manualMusicVolume = v;
        if (!manualMute) mixer.SetFloat("MusicVolume", Mathf.Log10(v) * 20);
    }
    public void SetUIVolumeManual(float v)
    {
        manualUIVolume = v;
        if (!manualMute) mixer.SetFloat("UIVolume", Mathf.Log10(v) * 20);
    }
    public void SetSFXVolumeManual(float v)
    {
        manualSFXVolume = v;
        if (!manualMute) mixer.SetFloat("SFXVolume", Mathf.Log10(v) * 20);
    }
    public void SetMasterVolumeManual(float v)
    {
        manualSFXVolume = v;
        if (!manualMute) mixer.SetFloat("MasterVolume", Mathf.Log10(v) * 20);
    }
    public void SetMuteManual(bool m)
    {
        manualMute = m;
        if (m)
        {
            mixer.SetFloat("UIVolume", -80);
            mixer.SetFloat("MusicVolume", -80);
            mixer.SetFloat("SFXVolume", -80);
            mixer.SetFloat("MasterVolume", -80);
        }
        else
        {
            SetMusicVolumeManual(manualMusicVolume);
            SetUIVolumeManual(manualUIVolume);
            SetSFXVolumeManual(manualSFXVolume);
            SetMasterVolumeManual(manualMasterVolume);
        }
    }
}
