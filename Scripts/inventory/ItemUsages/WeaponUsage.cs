using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUsage : Tool
{
    public GameObject toolEntity;
    public int KnockBack;
    private void Start()
    {
        BladeEntity blade = toolEntity.GetComponent<BladeEntity>();
        blade.damage = damage;
        blade.knockBack = KnockBack;
    }
    public override int Operate(Vector3 pos)
    {
        ToolsWielder.getInstance().WieldTool(toolEntity, CD, pos, 180);
        return 0;
    }
    
}
