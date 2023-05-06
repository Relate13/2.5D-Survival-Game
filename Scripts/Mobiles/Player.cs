using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
[System.Serializable]
public class PlayerStatus
{
    public float PlayerPos_x;
    public float PlayerPos_y;
    public float PlayerPos_z;
    public int Health = 100;
    public int MaxHealth = 100;
    public int Energy = 1000;
    public int MaxEnergy = 1000;
    public int Hunger = 100;
    public int MaxHunger = 100;
    public void Hurt(int amount)
    {
        Health -= amount;
        if (Health < 0)
            Health = 0;
    }
    public void AddHealth(int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
            Health = MaxHealth;
    }
    public void ReduceHunger(int amount)
    {
        Hunger -= amount;
        if (Hunger < 0)
            Hunger = 0;
    }
    public void AddHunger(int amount)
    {
        Hunger += amount;
        if (Hunger > MaxHunger)
            Hunger = MaxHunger;
    }
    public void ReduceEnergy(int amount)
    {
        Energy -= amount;
        if (Energy < 0)
            Energy = 0;
    }
    public void AddEnergy(int amount)
    {
        Energy += amount;
        if (Energy > MaxEnergy)
            Energy = MaxEnergy;
    }
}
public class Player : MonoBehaviour
{
    public bool freezeMovement = false;
    public bool spiritMode = false;
    public InventoryObject playerInventory;
    public Animator PlayerAnimator;
    public WorldTimer LaborTimer;
    public WorldTimer HurtTimer;
    public WorldTimer PlayerFootStepTimer;
    Vector3 movement = new Vector3();
    TileTerrain tileTerrain;
    public float accl = 3f;
    public float speed = 5f;
    //Vector2 Direction = new Vector2();
    public BlockSelector blockSelector;
    public DisplayInventory inventoryDisplayer;
    public ShakeEffect CameraShake;
    public PlayerStatus status=new PlayerStatus();
    public Rigidbody rb;
    public List<AudioClip> footStepSounds;
    public bool Alive = true;
    // Start is called before the first frame update
    public void Save()
    {
        status.PlayerPos_x = transform.position.x;
        status.PlayerPos_y = transform.position.y;
        status.PlayerPos_z = transform.position.z;
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.PLAYER_FILE, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, status);
        stream.Close();
    }
    public void Load()
    {
        if (File.Exists(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.PLAYER_FILE))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(SaveManager.SAVE_PATH + SaveManager.CURRENT_WORLD_FOLDER + SaveManager.PLAYER_FILE, FileMode.Open, FileAccess.Read);
            PlayerStatus newStatus = (PlayerStatus)formatter.Deserialize(stream);
            status.PlayerPos_x = newStatus.PlayerPos_x;
            status.PlayerPos_y = newStatus.PlayerPos_y;
            status.PlayerPos_z = newStatus.PlayerPos_z;
            status.Energy = newStatus.Energy;
            status.Health = newStatus.Health;
            status.Hunger = newStatus.Hunger;
            status.MaxHunger = newStatus.MaxHunger;
            status.MaxHealth = newStatus.MaxHealth;
            status.MaxEnergy = newStatus.MaxEnergy;
            transform.position = new Vector3(newStatus.PlayerPos_x, newStatus.PlayerPos_y, newStatus.PlayerPos_z);
            stream.Close();
        }
        else
        {
            Debug.Log("ERROR, FILE DOESN'T EXIST");
        }
    }
    void GetHungry()
    {
        int MaxRev = status.Hunger / (status.MaxHunger / 5);
        if (status.Health < status.MaxHealth)
        {
            status.AddHealth(MaxRev);
            status.ReduceHunger(MaxRev / 2);
        }
        if (status.Energy < status.MaxEnergy)
        {
            status.AddEnergy(MaxRev);
            status.ReduceHunger(MaxRev / 2);
        }
        status.ReduceHunger(1);
        StatusPanelController.GetInstance().SetStatus(status);
    }
    void Start()
    {
        LaborTimer = new WorldTimer();
        HurtTimer = new WorldTimer();
        PlayerFootStepTimer = new WorldTimer();
        tileTerrain = TileTerrain.GetInstance();
        StatusPanelController.GetInstance().SetStatus(status);
        InvokeRepeating("GetHungry", 5, 5);
    }
    public void freezePlayer(float time)
    {
        PlayerAnimator.SetFloat("Speed", 0);
        freezePlayer();
        Invoke("unfreezePlayer", time);
    }
    public void freezePlayer()
    {
        freezeMovement = true;
    }
    public void unfreezePlayer()
    {
        freezeMovement = false;
    }
    public void PlayerHurt(int damage)
    {
        if (!Alive)
        {
            return;
        }
        if (HurtTimer.Elapsed(1))
        {
            Debug.Log("Player Hurt " + damage);
            AudioClip[] sounds = GameDataHolder.getInstance().PlayerHurt;
            SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
            status.Hurt(damage);
            StatusPanelController.GetInstance().SetStatus(status);
            //CameraShake.ShakeMe();
            StatusPanelController.GetInstance().healthIcon.ShakeMe();
            if (status.Health <= 0)
            {
                Alive = false;
                AudioClip[] sounds2 = GameDataHolder.getInstance().PlayerDeath;
                SoundManager.GetInstance().PlaySound(sounds2[Random.Range(0, sounds2.Length)]);
                DeathScreen.GetInstance().DeclareDeath();
            }
            HurtTimer.Capture();
        }
    }
    public void EatFood(int HealthGain,int EnergyGain,int HungerGain)
    {
        AudioClip[] sounds = GameDataHolder.getInstance().PlayerEat;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
        status.AddHealth(HealthGain);
        status.AddEnergy(EnergyGain);
        status.AddHunger(HungerGain);
        StatusPanelController.GetInstance().SetStatus(status);
    }
    // Update is called once per frame
    void Update()
    {
        if (Alive)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //Debug.Log("Shifting");
                if (!spiritMode)
                {
                    spiritMode = true;
                    InvokeRepeating("SpiritProcess", 1, 1);
                }
            }
            else
            {
                if (spiritMode)
                {
                    spiritMode = false;
                    CancelInvoke("SpiritProcess");
                }
            }
            if (!freezeMovement)
                PlayerMove();
            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    //Vector3 pos = blockSelector.GetSelection();
            //    //Debug.Log("Generated in " + pos);
            //    //tileTerrain.GenerateMobile(Random.Range(0,4), pos, 1);

            //    tileTerrain.RandomGenerateMobile();
            //}
            //if (Input.GetKeyDown(KeyCode.H))
            //{
            //    Vector3 pos = blockSelector.GetSelection();
            //    Debug.Log("Generated in " + pos);
            //    tileTerrain.GenerateEntity(14, pos, 1);
            //}
            if (Input.GetKeyDown(KeyCode.E))
            {
                inventoryDisplayer.SwitchToolBarMode();
            }
            if (Input.GetMouseButtonDown(0) && (!EventSystem.current.IsPointerOverGameObject()))
            {
                int itemID = playerInventory.GetSelected();
                Debug.Log("Try Using Item ID: " + itemID);
                if (itemID >= 0)
                {
                    if (playerInventory.database.GetItem[itemID].ItemEntity.TryGetComponent(out Usage usage))
                    {
                        int loss = usage.Operate(blockSelector.GetSelection());
                        playerInventory.RemoveItem(itemID, loss);
                    }
                }
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") < -0.01f && (!EventSystem.current.IsPointerOverGameObject()))
            {
                playerInventory.SelectNext();
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0.01f && (!EventSystem.current.IsPointerOverGameObject()))
            {
                playerInventory.SelectPrev();
            }
            if (Input.GetMouseButtonDown(1))
            {
                float maxDistance = 1f;
                Vector3 rayStart = blockSelector.GetSelection() + new Vector3(0, -0.5f, 0); /*blockSelector.GetSelection()+new Vector3(0,0.25f,0)*/
                Vector3 rayDir = transform.up;
                Vector3 rayEnd = rayStart + rayDir * maxDistance;
                Ray ray = new Ray(rayStart, rayDir);
                Debug.DrawLine(rayStart, rayEnd, Color.red);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    print(hit.point);
                    print(hit.transform.position);
                    print(hit.collider.gameObject);
                    if (hit.collider.gameObject.TryGetComponent(out Interactable interactableEntitie))
                    {
                        interactableEntitie.Interact(this);
                    }
                    else if (hit.collider.gameObject.TryGetComponent(out NPCController nPCController))
                    {
                        nPCController.ShowNPCMenu();
                        freezePlayer();
                    }
                }
            }
        }
    }
    void SpiritProcess()
    {
        status.ReduceEnergy(2);
        
        StatusPanelController.GetInstance().SetStatus(status);
        if (status.Energy <= 0)
        {
            CancelInvoke("SpiritProcess");
            spiritMode = false;
        }
    }
    void PlayerMove()
    {
        
        movement.z = Input.GetAxis("Vertical");
        if (Mathf.Abs(movement.z) > 0.1f) { PlayerAnimator.SetFloat("MoveZ", movement.z); PlayerAnimator.SetFloat("MoveX", 0); }
        movement.x = Input.GetAxis("Horizontal");
        if (Mathf.Abs(movement.x) > 0.1f) { PlayerAnimator.SetFloat("MoveX", movement.x); PlayerAnimator.SetFloat("MoveZ", 0); }
        if (spiritMode && status.Energy > 0)
        {
            if (Mathf.Abs(movement.z) < 0.1f && Mathf.Abs(movement.x) < 0.1f)
            {
                CancelInvoke("SpiritProcess");
                spiritMode = false;
            }
        }
        else
        {
            //Debug.Log("not spirit mode");
            movement *= 0.5f;
        }
        movement *= speed * Time.deltaTime;
        PlayerAnimator.SetFloat("Speed", movement.sqrMagnitude);
        rb.position += movement;
        if(movement.sqrMagnitude > 0.00001 && PlayerFootStepTimer.Elapsed(spiritMode? 0.25f:0.5f))
        {
            SoundManager.GetInstance().PlaySound(footStepSounds[Random.Range(0, footStepSounds.Count)]);
            PlayerFootStepTimer.Capture();
        }
        //transform.position += movement;
            /*rb.AddForce(movement.normalized * accl);
            rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -speed, speed),Mathf.Clamp(rb.velocity.y, -speed, speed),Mathf.Clamp(rb.velocity.z, -speed, speed));*/
    }



    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        //pick up item
        var item=other.GetComponent<GroundItem>();
        if (item)
        {
            if (item.timer.Elapsed(0.5f))
            {
                if (playerInventory.AddItem(item.item.ID, 1))
                {
                    AudioClip[] sounds = GameDataHolder.getInstance().pickUpSound;
                    SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
                    Destroy(other.gameObject); 
                }
                else
                    MessageSystem.GetInstance().NewErrorMessage("ÎïÆ·À¸ÒÑÂú");
            }
        }
    }
    private void OnApplicationQuit()
    {
        playerInventory.Container=new Inventory();
    }
}
