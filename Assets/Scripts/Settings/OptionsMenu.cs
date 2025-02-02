using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    Resolution[] resolutions;

    [SerializeField] TMP_Dropdown resolutionsDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle fullscreenToggle;

    private void Start()
    {
        
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();

        int currentResolution = 0;
        List<string> optionsNames = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            optionsNames.Add(resolutions[i].width + "x" + resolutions[i].height);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
        }

        resolutionsDropdown.AddOptions(optionsNames);
        resolutionsDropdown.value = currentResolution;
        resolutionsDropdown.RefreshShownValue();

        if (!PlayerPrefs.HasKey("resolutionIndex"))
        {
            PlayerPrefs.SetInt("resolutionIndex", resolutions.Length - 1);
            PlayerPrefs.SetFloat("volume", -40f);
            PlayerPrefs.SetInt("quality", 5);
            PlayerPrefs.SetInt("fullscreen", 1);
        }

        Load();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolume(float newVolume)
    {
        audioMixer.SetFloat("Main Volume", newVolume);
    }

    public void SetQualityLevel(int newQualityLevel)
    {
        QualitySettings.SetQualityLevel(newQualityLevel);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("resolutionIndex", resolutionsDropdown.value);
        float volume;
        audioMixer.GetFloat("Main Volume", out volume);
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetInt("quality", QualitySettings.GetQualityLevel());
        if (Screen.fullScreen)
        {
            PlayerPrefs.SetInt("fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("fullscreen", 0);
        }

        PlayerPrefs.Save();
    }

    public void Load()
    {
        int index = PlayerPrefs.GetInt("resolutionIndex");
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionsDropdown.value = index;
        resolutionsDropdown.RefreshShownValue();

        float volume = PlayerPrefs.GetFloat("volume");
        audioMixer.SetFloat("Main Volume", volume);
        volumeSlider.value = volume;

        index = PlayerPrefs.GetInt("quality");
        QualitySettings.SetQualityLevel(index);
        qualityDropdown.value = index;
        qualityDropdown.RefreshShownValue();

        int isFullscreen = PlayerPrefs.GetInt("fullscreen");
        if (isFullscreen == 1)
        {
            fullscreenToggle.isOn = true;
            Screen.fullScreen = true;
        }
        else
        {
            fullscreenToggle.isOn = false;
            Screen.fullScreen = false;
        }
        fullscreenToggle.Select();

    }
}
