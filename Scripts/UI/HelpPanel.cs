using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    int current = 0;
    public PanelGroup panelGroup;
    private void Start()
    {
        panelGroup.ShowPanel(current);
    }
    public void NextHelp()
    {
        ++current;
        current %= panelGroup.panels.Count;
        panelGroup.ShowPanel(current);
    }
    public void PrevHelp()
    {
        current = current - 1 + panelGroup.panels.Count;
        current %= panelGroup.panels.Count;
        panelGroup.ShowPanel(current);
    }
}
