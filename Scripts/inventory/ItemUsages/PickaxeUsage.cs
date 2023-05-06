using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeUsage : Tool
{
    public GameObject PickaxeEntity;
    private Vector3 hitPos;
    public override int Operate(Vector3 pos)
    {
        bool firstUse = false;
        if (LaborTimer == null)
        {
            LaborTimer = new WorldTimer();
            firstUse = true;
        }
        if (firstUse || LaborTimer.Elapsed(CD))
        {
            GameDataHolder.getInstance().player.freezePlayer(CD);
            ToolsWielder.getInstance().WieldTool(PickaxeEntity, CD, pos, 90);
            hitPos = pos;
            Invoke("StartHit", CD);
            LaborTimer.Capture();
        }
        return 0;
    }
    void StartHit()
    {
        if (TryRayHit(hitPos, out GameObject hitObj))
        {
            if (hitObj.TryGetComponent(out MineStone Hitted))
            {
                Hitted.Harm(damage);
            }
        }
    }
}
