using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public string HitTAG;
    public float damageAmount;
    public float normalDamgeAmount;
    public float highDamgeAmount;
    public bool normalDiffLevel=false;


    private void Start()
    {
        LoadEnemyData();
    }
    public void LoadEnemyData()
    {
        if (PlayerPrefs.HasKey("SettingsControlsNormalDiff"))
        {
            if (PlayerPrefs.GetInt("SettingsControlsNormalDiff") == 1) normalDiffLevel = true;
            else normalDiffLevel = false;
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == HitTAG)
        {
            if(collision.gameObject.GetComponent<CharacterScript>()!=null)
            {
                collision.gameObject.GetComponent<CharacterScript>().TakeDamage(damageAmount = (normalDiffLevel?normalDamgeAmount:highDamgeAmount));
            }

            if(collision.gameObject.GetComponent<RockTargetscript>()!=null)
            {
                collision.gameObject.GetComponent<RockTargetscript>().ResetRocksPosition();
            }
            if (collision.gameObject.GetComponent<RockRollScript>() != null && collision.gameObject.GetComponent<RockRollScript>().canJump)
            {
                //collision.gameObject.GetComponent<RockRollScript>().forceJump = damageAmount;
                collision.gameObject.GetComponent<RockRollScript>().AddForceOnRockJump();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == HitTAG)
        {
            if (other.gameObject.GetComponent<CharacterScript>() != null)
            {
                other.gameObject.GetComponent<CharacterScript>().TakeDamage(damageAmount);
            }
        }
        if (other.gameObject.tag == HitTAG)
        {
            //Debug.Log("CollisionCheck");
            Vector3 dir = (transform.position - other.transform.position).normalized;
            if (other.gameObject.GetComponent<RockRollScript>() != null)
            {
                other.gameObject.GetComponent<RockRollScript>().dirForce = -dir;
                other.gameObject.GetComponent<RockRollScript>().AddForceOnRock();
            }
            if(other.gameObject.GetComponent<EnergyReset>()!=null)
            {
                other.gameObject.GetComponent<EnergyReset>().resetEnergy();
                Debug.Log("CollisionCheck");
            }
        }

        
    }
}
