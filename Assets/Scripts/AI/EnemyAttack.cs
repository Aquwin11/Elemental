using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float force;
    public EnemyAttackValidation enemyAttackValidation;
    [SerializeField] float damageNumber;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Check if attack");
            if(other.GetComponent<CharacterController>()!=null)
            {
                Vector3 dir = (other.transform.position - transform.position).normalized;
                other.GetComponent<CharacterController>().Move(dir * force);

            }
            if (other.GetComponent<CharacterScript>() != null)
            {
                other.GetComponent<CharacterScript>().TakeDamage(damageNumber);
            }
        }
    }
}
