using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeEntity : MonoBehaviour
{
    public int damage;
    public float knockBack;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Mobile mobile)) {
            mobile.Harm(damage);
            Vector3 bias = mobile.gameObject.transform.position - GameDataHolder.getInstance().player.transform.position;
            mobile.rb.AddForce(bias * knockBack);
        }
    }
}
