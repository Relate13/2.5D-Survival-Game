using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : Mobile
{
    public Strategy strategy;
    public Vector3 destination;
    public GameObject followingObject;
    public float MoveSpeed;
    public WorldTimer IdleTimer;
    public Animator NPCAnimator;
    public WorldTimer FootStepTimer;
    public float IdleTime;

    public NPCData data;

    void Start()
    {
        strategy = Strategy.IDLE;
        destination = transform.position;
        rb = GetComponent<Rigidbody>();
        IdleTimer = new WorldTimer();
        IdleTime = Random.Range(1f, 5f);
        FootStepTimer = new WorldTimer();
    }

    // Update is called once per frame
    void Update()
    {
        switch (strategy)
        {
            case Strategy.FREEZE:
                break;
            case Strategy.IDLE:
                if (IdleTimer == null)
                {
                    IdleTimer = new WorldTimer();
                }
                NPCAnimator.SetFloat("Speed", 0);
                if (IdleTimer.Elapsed(IdleTime))
                {
                    destination = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                    destination += transform.position;
                    strategy = Strategy.MOVING;
                    Debug.Log("Mobile MOVING");
                    IdleTime = Random.Range(1f, 5f);
                    IdleTimer = null;
                }
                break;
            case Strategy.MOVING:
                //rb.AddForce((destination-transform.position).normalized);
                if (FootStepTimer.Elapsed(0.5f))
                {
                    AudioClip[] sounds = GameDataHolder.getInstance().NPCFootSteps;
                    audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
                    FootStepTimer.Capture();
                }
                NPCAnimator.SetFloat("Speed", 1);
                transform.position = Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime);
                NPCAnimator.SetFloat("MoveX", destination.x - transform.position.x);
                NPCAnimator.SetFloat("MoveZ", destination.z - transform.position.z);
                if (Vector3.Distance(destination, transform.position) < 1.5)
                {
                    strategy = Strategy.IDLE;
                    Debug.Log("Mobile IDLE");
                }
                break;
            default: break;
        }
    }
    public override void Harm(int damage)
    {
        return;
    }
    public void Freeze()
    {
        strategy = Strategy.FREEZE;
        NPCAnimator.SetFloat("Speed", 0);
    }
    public void UnFreeze()
    {
        strategy = Strategy.IDLE;
    }
    public void ShowNPCMenu()
    {
        GameDataHolder.getInstance().npcMenu.AssociateNPC(this);
        GameDataHolder.getInstance().npcMenu.ClearContent();
        GameDataHolder.getInstance().npcMenu.SetNPCData(data);
        GameDataHolder.getInstance().npcMenu.Show();
        Freeze();
    }
    private void OnCollisionEnter(Collision collision)
    {
        destination = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
    }
}
