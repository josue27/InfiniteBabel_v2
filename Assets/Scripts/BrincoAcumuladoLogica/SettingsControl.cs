using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsControl : MonoBehaviour
{
    public GameObject panelSettings;



    public void Toggle_PanelSettings()
    {
        if (!panelSettings) return;

        panelSettings.SetActive(!panelSettings.activeInHierarchy);
    }
        
     
}
