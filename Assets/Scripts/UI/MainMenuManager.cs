using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [Header("Audio")]
    public Button loadButton;

    [Header("Audio")]
    [SerializeField] AudioSource menuAduioSource;
    [SerializeField] AudioSource menuMusicAduioSource;
    [SerializeField] AudioClip buttonSound;


    [Header("Main Canvas Object")]
    [SerializeField] GameObject MainMenuCanvas;
    [SerializeField] GameObject SettingsMenuCanvas;
    [SerializeField] GameObject PauseMenuCanvas;
    [SerializeField] GameObject DeathMenuCanvas;
    [SerializeField] GameObject QuitCanvas;
    [SerializeField] GameObject WinCanvas;
    [Header("Setting Canvas Objects")]
    [SerializeField] GameObject generalMenuCanvas;
    [SerializeField] GameObject AudioMenuCanvas;
    [SerializeField] GameObject ControlsMenuCanvas;
    public List<GameObject> settingsPages;


    [Header("Transition")]
    //public Transform TrasitionOrb;
    public string GameplayScene;
    public string MainMenuScene;
    public int CurrentSceneIndex;

    [Header("AudioSetting")]
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider UISoundSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Toggle MuteToggle;


    [Header("ControlSettings")]
    public Slider SensitivitySlider;
    public Toggle InvertY;
    public Toggle InvertX;
    public Toggle normalDiff;
    public Toggle hardDiff;


[Header("UINavigation")]
public GameObject PlayButton;
public GameObject VideoButton;
public GameObject QuitButtonYes;
public GameObject ResolutionDropDown;
public GameObject MusicSliderObject;
public GameObject SensitivitySliderObject;
public bool isSettingsPageOpen;
public bool isQuitPopupEnable;

// Start is called before the first frame update
    void Start()
    {
        LoadSettings();
        if (loadButton != null)
        {
            if (File.Exists(Application.persistentDataPath + "/ElementalData.game")) {
                loadButton.interactable = true;
            }
            else
            {
                loadButton.interactable = false;
            }
        }
    }

    // Update is called once per frame
    public void LoadSettings()
    {
        //Debug.Log("Load");

        // Its not nice, but it works
        // Since controls are disabled when not on screen, settings are not updated until the control is put on screen, so we have to do things manually

        // Video

        // Audio
        //master
        if (PlayerPrefs.HasKey("SettingsAudioMaster"))
        {
            MasterSlider.value = PlayerPrefs.GetFloat("SettingsAudioMaster");
            //AudioManager.audioInstance.ChangeMusicVolume(MusicSlider.value);
        }
        // Music
        if (PlayerPrefs.HasKey("SettingsAudioMusic"))
        {
            MusicSlider.value = PlayerPrefs.GetFloat("SettingsAudioMusic");
            //AudioManager.audioInstance.ChangeMusicVolume(MusicSlider.value);
        }
        // UI
        if (PlayerPrefs.HasKey("SettingsAudioUI"))
        {
            UISoundSlider.value = PlayerPrefs.GetFloat("SettingsAudioUI");
            //AudioManager.audioInstance.ChangeBackgroundVolume(BackgropundSoundSlider.value);
        }
        // SFX
        if (PlayerPrefs.HasKey("SettingsAudioSFX"))
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SettingsAudioSFX");
            //AudioManager.audioInstance.ChangeSFXVolume(SFXSlider.value);
        }
        // Mute
        if (PlayerPrefs.HasKey("SettingsAudioMute"))
        {
            if (PlayerPrefs.GetInt("SettingsAudioMute") == 1)
            {
                MuteToggle.isOn = true;
                //AudioManager.audioInstance.MutevolumeUpdate();
            }
            else
            {
                MuteToggle.isOn = false;
                //AudioManager.audioInstance.UnMutevolumeUpdate();
            }
        }

        // General - No controls have been properly implemented yet to have anything update
        // Sensitivity
        if (PlayerPrefs.HasKey("SettingsControlsSensitivity")) SensitivitySlider.value = PlayerPrefs.GetFloat("SettingsControlsSensitivity");
        // Invert x
        if (PlayerPrefs.HasKey("SettingsControlsInvertX"))
        {
            //Debug.Log(PlayerPrefs.GetInt("SettingsControlsInvertX"));
            if (PlayerPrefs.GetInt("SettingsControlsInvertX") == 1) InvertX.isOn = true;
            else InvertX.isOn = false;
        }
        // Invert y
        if (PlayerPrefs.HasKey("SettingsControlsInvertY"))
        {
            if (PlayerPrefs.GetInt("SettingsControlsInvertY") == 1) InvertY.isOn = true;
            else InvertY.isOn = false;
        }
        // Normal Diff
        if (PlayerPrefs.HasKey("SettingsControlsNormalDiff"))
        {
            if (PlayerPrefs.GetInt("SettingsControlsNormalDiff") == 1) normalDiff.isOn = true;
            else normalDiff.isOn = false;
        }
        // Hard Diff
        if (PlayerPrefs.HasKey("SettingsControlsHardDiff"))
        {
            if (PlayerPrefs.GetInt("SettingsControlsHardDiff") == 1) hardDiff.isOn = true;
            else hardDiff.isOn = false;
        }



        GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>().UpdateAudioLevels();
    }

    public void OnSettingsButtonClikc()
    {
        SettingsMenuCanvas.gameObject.SetActive(true);
        isSettingsPageOpen = true;
        EventSystem.current.SetSelectedGameObject(VideoButton);
        menuAduioSource.PlayOneShot(buttonSound);
    }
    public void OnSettingsBackButtonClikc()
    {
        if (AudioMenuCanvas.activeSelf || ControlsMenuCanvas.activeSelf || generalMenuCanvas.activeSelf)
        {
            foreach (var item in settingsPages)
            {
                if (item.activeSelf)
                {
                    item.GetComponent<UITransition>().CloseAlphaDialog();
                    Invoke("SettingPageCloseDelay", 0.4f);
                    break;
                }
            }
        }
        else
        {
            SettingPageCloseDelay();

        }
        EventSystem.current.SetSelectedGameObject(PlayButton);
        isSettingsPageOpen = false;
        menuAduioSource.PlayOneShot(buttonSound);

        SaveSettings();
    }
    void SettingPageCloseDelay()
    {
        SettingsMenuCanvas.GetComponent<UITransition>().CloseAlphaDialog();
    }

    public void SaveSettings()
    {

        // Audio
        // Master
        PlayerPrefs.SetFloat("SettingsAudioMaster", MasterSlider.value);
        // Music
        PlayerPrefs.SetFloat("SettingsAudioMusic", MusicSlider.value);
        // Background
        PlayerPrefs.SetFloat("SettingsAudioUI", UISoundSlider.value);
        // SFX
        PlayerPrefs.SetFloat("SettingsAudioSFX", SFXSlider.value);
        // Mute
        if (MuteToggle.isOn) PlayerPrefs.SetInt("SettingsAudioMute", 1);
        else PlayerPrefs.SetInt("SettingsAudioMute", 0);

        // General
        // Sensitivity
        PlayerPrefs.SetFloat("SettingsControlsSensitivity", SensitivitySlider.value);
        // Invert x
        if (InvertX.isOn) PlayerPrefs.SetInt("SettingsControlsInvertX", 1);
        else PlayerPrefs.SetInt("SettingsControlsInvertX", 0);
        //Debug.Log(InvertX.isOn);
        //Debug.Log(PlayerPrefs.GetInt("SettingsControlsInvertX"));
        // Invert y
        if (InvertY.isOn) PlayerPrefs.SetInt("SettingsControlsInvertY", 1);
        else PlayerPrefs.SetInt("SettingsControlsInvertY", 0);

        if (normalDiff.isOn) PlayerPrefs.SetInt("SettingsControlsNormalDiff", 1);
        else PlayerPrefs.SetInt("SettingsControlsNormalDiff", 0);
        if (hardDiff.isOn) PlayerPrefs.SetInt("SettingsControlsHardDiff", 1);
        else PlayerPrefs.SetInt("SettingsControlsHardDiff", 0);

        GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>().UpdateAudioLevels();
    }
    public void OnAudioPageClick()
    {
        foreach (var item in settingsPages)
        {
            if (item.activeSelf)
            {
                item.GetComponent<UITransition>().CloseAlphaDialog();
            }
        }
        AudioMenuCanvas.SetActive(true);
        menuAduioSource.PlayOneShot(buttonSound);
    }
    public void OnControlPageClick()
    {
        foreach (var item in settingsPages)
        {
            if (item.activeSelf)
            {
                item.GetComponent<UITransition>().CloseAlphaDialog();
            }
        }
        ControlsMenuCanvas.SetActive(true);
        menuAduioSource.PlayOneShot(buttonSound);
    }

    public void OnGeneralPageClick()
    {
        foreach (var item in settingsPages)
        {
            if (item.activeSelf)
            {
                item.GetComponent<UITransition>().CloseAlphaDialog();
            }
        }
        generalMenuCanvas.SetActive(true);
        menuAduioSource.PlayOneShot(buttonSound);
    }

    public void OnStartNewGameButtonClick()
    {
        menuAduioSource.PlayOneShot(buttonSound);
        GameManager.gameManagerInstance.DeleteLoadFile();
        GameManager.gameManagerInstance.NewLoadNextLevel();
    }

    public void OnLoadPreviousGameButtonClick()
    {
        menuAduioSource.PlayOneShot(buttonSound);
        GameManager.gameManagerInstance.PreviousLoadNextLevel();
    }

    public void OnReturnButtonClick()
    {
        menuAduioSource.PlayOneShot(buttonSound);
        GameManager.gameManagerInstance.ReturntoMianMenuLevel();
    }

    public void OnDeathOnReturnButtonClick()
    {
        menuAduioSource.PlayOneShot(buttonSound);
        Debug.Log("death file exsist");
        GameManager.gameManagerInstance.ReturntoMianMenuLevel();
        GameManager.gameManagerInstance.DeleteLoadFile();
    }
    public void EnablePauseScreen()
    {
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.gamePause = true;
        }
        PauseMenuCanvas.SetActive(true);
    }
    public void EndScreen()
    {
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.gamePause = true;
        }
        PauseMenuCanvas.SetActive(true);
        DeathMenuCanvas.SetActive(true);
    }

    public void onWin()
    {
        WinCanvas.SetActive(true);
        GameManager.gameManagerInstance.gamePause = true
;    }

    public void onResumeButtonClick()
    {
        if (GameManager.gameManagerInstance != null)
        {
            GameManager.gameManagerInstance.gamePause = false;
        }
        PauseMenuCanvas.SetActive(false);
        menuAduioSource.PlayOneShot(buttonSound);
    }


    [Header("Input action")]
    [SerializeField] InputActionAsset inputAction;
    public void ResetAllKeyBindings()
    {
        foreach (InputActionMap inputActions in inputAction.actionMaps)
        {
            inputAction.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
        menuAduioSource.PlayOneShot(buttonSound);
    }

    public void DisableStoryCanvas(GameObject storyCanvas)
    {
        storyCanvas.SetActive(false);
        if (GameManager.gameManagerInstance != null)
            GameManager.gameManagerInstance.gamePause = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
