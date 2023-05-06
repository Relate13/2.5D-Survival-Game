using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPanelController : MonoBehaviour
{
    public static StatusPanelController Instance;
    public static StatusPanelController GetInstance() { return Instance; }
    public StatusPanelController()
    {
        Instance = this;
    }
    public ProgressBarController healthBar;
    public ProgressBarController energyBar;
    public ProgressBarController hungerBar;
    public ShakeEffect healthIcon;
    public ShakeEffect energyIcon;
    public ShakeEffect hungerIcon;
    public void SetStatus(PlayerStatus status)
    {
        healthBar.setPercentage(((float)status.Health) / ((float)status.MaxHealth));
        energyBar.setPercentage(((float)status.Energy) / ((float)status.MaxEnergy));
        hungerBar.setPercentage(((float)status.Hunger) / ((float)status.MaxHunger));
    }
}
