using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : Mobile
{
    public Vector2Int atk;
    public Strategy strategy;
    public Vector3 destination;
    public GameObject followingObject;
    public float MoveSpeed;
    public float DiscoverRange;
    public float ForgiveRange;
    public WorldTimer IdleTimer;
    public WorldTimer SoundTimer;
    public Animator SheepAnimator;
    public float IdleTime;
    // Start is called before the first frame update
    void Start()
    {
        strategy = Strategy.IDLE;
        destination = transform.position;
        rb = GetComponent<Rigidbody>();
        StartDetection();
        IdleTimer = new WorldTimer();
        SoundTimer = new WorldTimer();
        IdleTime = Random.Range(1f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (strategy)
        {
            case Strategy.IDLE:
                if (IdleTimer == null)
                {
                    IdleTimer = new WorldTimer();
                }
                SheepAnimator.SetFloat("Speed", 0);
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
                SheepAnimator.SetFloat("Speed", 1);
                transform.position = Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime);
                SheepAnimator.SetFloat("Direction", destination.x - transform.position.x > 0 ? 0.1f : -0.1f);
                if (Vector3.Distance(destination, transform.position) < 1.5)
                {
                    strategy = Strategy.IDLE;
                    Debug.Log("Mobile IDLE");
                }
                break;
            case Strategy.FOLLOWING:
                SheepAnimator.SetFloat("Speed", 1);
                transform.position = Vector3.MoveTowards(transform.position, followingObject.transform.position, MoveSpeed * Time.deltaTime);
                SheepAnimator.SetFloat("Direction", followingObject.transform.position.x - transform.position.x > 0 ? 0.1f : -0.1f);
                //rb.AddForce((followingObject.transform.position - transform.position).normalized);
                if (Vector3.Distance(followingObject.transform.position, transform.position) > ForgiveRange)
                {
                    strategy = Strategy.IDLE;
                    Debug.Log("Mobile IDLE");
                }
                break;
            default: break;
        }
        if (SoundTimer.Elapsed(Random.Range(5f, 10f)))
        {
            AudioClip[] sounds = GameDataHolder.getInstance().SheepMove;
            audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
            SoundTimer.Capture();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        destination = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
    }
    public override void HarmEffect()
    {
        AudioClip[] sounds = GameDataHolder.getInstance().SheepHit;
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }
    public override void DeathEffect()
    {
        AudioClip[] sounds = GameDataHolder.getInstance().SheepDeath;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
    }
}
