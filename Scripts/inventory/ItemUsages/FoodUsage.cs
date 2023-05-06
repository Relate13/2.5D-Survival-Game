using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodUsage : Usage
{
    public int HealthGain;
    public int EnergyGain;
    public int HungerGain;
    public override int Operate(Vector3 pos) {
        GameDataHolder.getInstance().player.EatFood(HealthGain, EnergyGain, HungerGain);
        return 1;
    }
}
