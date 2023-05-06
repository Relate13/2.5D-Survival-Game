using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUsage : Usage
{
    [SerializeField]
    private int TileID;
    public override int Operate(Vector3 pos) 
    {
        MapBlock block = TileTerrain.GetInstance().GetMapBlock(pos);
        if (block.tileBase != 2)
        {
            if (block.tileBase != TileID)
            {
                TileTerrain.GetInstance().SetGameMapTile(pos, TileID);
                AudioClip[] sounds = GameDataHolder.getInstance().plowSound;
                SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
                return 1;
            }
            else
            {
                MessageSystem.GetInstance().NewWarningMessage("�޷�����ͬ�ؿ��Ϸ��õؿ�");
            }
        }
        else MessageSystem.GetInstance().NewErrorMessage("�޷���ˮ���Ϸ��õؿ�");
        return 0; 
    }
}
