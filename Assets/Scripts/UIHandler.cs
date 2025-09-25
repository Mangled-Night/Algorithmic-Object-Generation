using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GenerateTerrain Generator;
    public TMP_Dropdown genDropdown;
    public TMP_InputField[] fields;
    public GameObject[] BatchGen;
    public Slider probSlider;
    public TMP_InputField probField;
    public GameObject RandSettings;
    public GameObject Settings;
    public GameObject SettingsButton;
    public GameObject ViewGenButton;
    public GameObject Background;
    public GameObject BatchSizeSetting;

    private bool batchOn;

    public void Generate()
    {
        int[] values = new int[fields.Length];
        int i = 0;
        foreach (TMP_InputField input in fields)
        {
            if (!input.gameObject.activeSelf) { values[i] = 0; }
            values[i] = int.Parse(input.text);
            i++;
        }

        Generator.UIGen(genDropdown.value, values, probSlider.value, batchOn);
        Settings.SetActive(false);
        SettingsButton.SetActive(true);
        Background.SetActive(false);
        ViewGenButton.SetActive(false);
    }

    public void ToggleSettings()
    {
        if (genDropdown.value == 3)
        {
            foreach (GameObject setting in BatchGen)
            {
                setting.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject setting in BatchGen)
            {
                setting.SetActive(false);
            }
        }

        if (genDropdown.value == 2)
        {
            RandSettings.SetActive(true);
        }
        else
        {
            RandSettings.SetActive(false);
        }
    }


    public void ModifySlider()
    {
        if (string.IsNullOrEmpty(probField.text))
        {
            probSlider.value = 0;
            probField.text = "0";
            return;
        }


        var value = int.Parse(probField.text);
        if (value > 100) { value = 100; }
        if (value < 0) { value = 0; }

        probSlider.value = value;
        probField.text = value.ToString();
    }

    public void BatchOptions(TMP_Dropdown batchDropdown)
    {
        batchOn = (batchDropdown.value == 1);
        BatchSizeSetting.SetActive(batchOn);
    }
    public void ToggleGenerateSettings()
    {
        Settings.SetActive(!Settings.activeSelf);
        SettingsButton.SetActive(!SettingsButton.activeSelf);
        Background.SetActive(!Background.activeSelf);
        ViewGenButton.SetActive(!ViewGenButton.activeSelf);
    }
}
