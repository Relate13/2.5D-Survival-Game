using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SickleUsage : Tool
{
    public GameObject toolEntity;
    private void Start()
    {
        SickleEntity blade = toolEntity.GetComponent<SickleEntity>();
        
    }
    public override int Operate(Vector3 pos)
    {
        ToolsWielder.getInstance().WieldTool(toolEntity, CD, pos, 180);
        return 0;
    }

}
