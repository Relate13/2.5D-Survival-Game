using UnityEngine;

public class keyboardControl : MonoBehaviour
{
    //public Animator PlayerAnimator;
    //public WorldTimer LaborTimer;
    //Vector3 movement = new Vector3();
    //TileTerrain tileTerrain;
    //public float accl = 3f;
    //public float speed = 5f;
    ////Vector2 Direction = new Vector2();
    //public BlockSelector blockSelector;
    //public DisplayInventory inventoryDisplayer;
    //public InventoryObject playerInventory;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    LaborTimer = new WorldTimer();
    //    tileTerrain = TileTerrain.GetInstance();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    PlayerMove();
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        Vector3 pos = blockSelector.GetSelection();
    //        Debug.Log("Generated in " + pos);
    //        tileTerrain.GenerateEntity(7, pos, 1);
    //    }
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        inventoryDisplayer.SwitchToolBarMode();
    //    }
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        int itemID = playerInventory.GetSelected();
    //        Debug.Log("Try Using Item ID: " + itemID);
    //        if (itemID >= 0)
    //        {
    //            if (playerInventory.database.GetItem[itemID].ItemEntity.TryGetComponent(out Usage usage))
    //            {
    //                int loss = usage.Operate(blockSelector.GetSelection());
    //                playerInventory.RemoveItem(itemID, loss);
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("No Item Selected");
    //        }
    //    }
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        if (LaborTimer.Elapsed(1))
    //        {
    //            float maxDistance = 1f;
    //            Vector3 rayStart = blockSelector.GetSelection() + new Vector3(0, -0.5f, 0); /*blockSelector.GetSelection()+new Vector3(0,0.25f,0)*/
    //            Vector3 rayDir = transform.up;
    //            Vector3 rayEnd = rayStart + rayDir * maxDistance;
    //            Ray ray = new Ray(rayStart, rayDir);
    //            Debug.DrawLine(rayStart, rayEnd, Color.red);
    //            RaycastHit hit;
    //            if (Physics.Raycast(ray, out hit, maxDistance))
    //            {
    //                print(hit.point);
    //                print(hit.transform.position);
    //                print(hit.collider.gameObject);
    //                Harvestable crop;
    //                hit.collider.gameObject.TryGetComponent(out crop);
    //                if (crop != null)
    //                {
    //                    crop.Harm(1);
    //                }
    //            }
    //            LaborTimer.Capture();
    //            /*Vector3 pos = blockSelector.GetSelection();
    //            Vector2Int ChunkID = tileTerrain.WorldPosTransChunkID(pos);
    //            Vector3 inChunkPos = tileTerrain.WorldPosTransInChunkPos(pos);
    //            MapBlock selectedBlock = tileTerrain.gameMap.GetChunk(ChunkID).GetMapBlock(new Vector2Int((int)inChunkPos.x, (int)inChunkPos.z));
    //            if (selectedBlock.ECU != null) 
    //            {
    //                selectedBlock.ECU.entity.SelfDestruct();
    //            }*/
    //        }
    //        else Debug.Log("ELAPSED TIME:" + LaborTimer.GetElapsedTime());
    //    }

    //}
    //void PlayerMove()
    //{
    //    movement.z = Input.GetAxis("Vertical");
    //    if (Mathf.Abs(movement.z) > 0.1f) { PlayerAnimator.SetFloat("MoveZ", movement.z); PlayerAnimator.SetFloat("MoveX", 0); }
    //    movement.x = Input.GetAxis("Horizontal");
    //    if (Mathf.Abs(movement.x) > 0.1f) { PlayerAnimator.SetFloat("MoveX", movement.x); PlayerAnimator.SetFloat("MoveZ", 0); }
    //    movement *= speed * Time.deltaTime;
    //    PlayerAnimator.SetFloat("Speed",movement.sqrMagnitude);
    //    transform.position += movement;
    //    /*rb.AddForce(movement.normalized * accl);
    //    rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -speed, speed),Mathf.Clamp(rb.velocity.y, -speed, speed),Mathf.Clamp(rb.velocity.z, -speed, speed));*/
    //}
}
