using System;
using UnityEngine;

public class SettingClose : MonoBehaviour
{
    public GameObject settingsPanel;
    
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
    
}
