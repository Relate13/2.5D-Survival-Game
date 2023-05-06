using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGroup : MonoBehaviour
{
    public int DefaultPanel;
    public List<GameObject> panels;
    private void Start()
    {
        if(DefaultPanel>=0)
            ShowPanel(DefaultPanel);
    }
    public void HideAll()
    {
        foreach (GameObject panel in panels)
            panel.SetActive(false);
    }
    public void ShowPanel(int id)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            if(i == id)
                panels[i].SetActive(true);
            else panels[i].SetActive(false);
        }
    }
}
