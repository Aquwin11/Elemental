using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulletscript : MonoBehaviour
{
    public Rigidbody bulletRB;
    public float bulletSpeed;
    public Transform myTransf;
    public Collider collider;
    public float InvokeCollider;
    private void OnEnable()
    {
        collider.enabled = false;
        bulletRB.velocity = (transform.forward * bulletSpeed);
        Invoke("EnableCollider", InvokeCollider);
    }

    void EnableCollider()
    {
        collider.enabled = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        ObjectSpawer.Instance.GetObject(ObjectSpawer.ObjectType.ExplosionEffect, pos, rot);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        collider.enabled = false;
    }
}
