using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class EnemyBasicAI : CharacterScript
{
    //public GameObject target;
    public float followSpeed;
    public float lengthFOV;
    public float awareFOV;
    public float non_awareFOV;
    public float lookRotationSpeed;
    public bool isChase;
    public bool canShoot;
    public bool isAttacking;
    public LayerMask whatisPlayer;
    public Transform player;
    public float chaseSpeed;
    //public NavMeshAgent enemyAgent;
    public bool inAttackRange;
    public float attachRange;
    public bool canAttack;
    public List<Transform> closestLeg;
    public float attackCounter;
    public float attackBuffer;
    public Vector3 playerAttackOffset;
    public float resetAttackDuration;
    public float attackLeadSpeed;
    public float attackSpeed;
    public float attackDistance;
    public Ease attackEase;


    [Header("Aim and Projectile")]
    public LineRenderer projectileLine;
    public Transform tailPointer;
    public GameObject aimer;
    public Vector3 aimerOffset;
    public float aimCounter;
    public float aimBuffer;
    [SerializeField]
    private float shootCounter;
    [SerializeField]
    private float shootBuffer;

    [Header("Test")]
    public Transform TestObject;
    public GameObject Bullet;
    public float bulletSpeed;

    [Header(" EnemyHealthUI ")]
    public Transform healthBarParent;

    [Header("EnemyAudio")]
    public AudioSource enemyAudio;
    public AudioClip attackgAudioClip;

    public bool normalDiffLevel = false;
    void Start()
    {
        //player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameManagerInstance != null && GameManager.gameManagerInstance.gamePause)
            return;
        CheckFOV();
        CheckIfCanAttack();
        HealthBarFacePlayer();
        LerpHealthValue();
    }

    private void CheckIfCanAttack()
    {
        if (attackCounter < attackBuffer)
        {
            attackCounter += Time.deltaTime;
        }
        else
        {
            canAttack = true;
        }
    }
    void CheckIfCanAim()
    {
        if (aimCounter < aimBuffer)
        {
            aimCounter += Time.deltaTime;
        }
        else
        {
            StartDrawLine();
            CheckIfCanShoot();

           /* RaycastHit[] hits = Physics.RaycastAll(aimer.transform.position, aimer.transform.forward, Vector3.Distance(player.position,aimer.transform.position)+0.5f);
            Debug.Log("target0 ");
            if (hits.Length!=0)
            {
                Transform target = hits[0].transform;
                Debug.Log("target1 " + target.gameObject.name);

                if (target.tag=="Player")
                {
                    Debug.Log("target2 " + target.gameObject.name);
                }
            }*/
        }
    }
    public void HealthBarFacePlayer()
    {
        if(Camera.main!=null)
        {
            healthBarParent.rotation = Quaternion.LookRotation(healthBarParent.position - Camera.main.transform.position);
        }
    }

    void CheckIfCanShoot()
    {
        if (shootCounter < shootBuffer)
        {
            shootCounter += Time.deltaTime;
        }
        else
        {
            ShootPlayer();
            /* RaycastHit[] hits = Physics.RaycastAll(aimer.transform.position, aimer.transform.forward, Vector3.Distance(player.position,aimer.transform.position)+0.5f);
             Debug.Log("target0 ");
             if (hits.Length!=0)
             {
                 Transform target = hits[0].transform;
                 Debug.Log("target1 " + target.gameObject.name);

                 if (target.tag=="Player")
                 {
                     Debug.Log("target2 " + target.gameObject.name);
                 }
             }*/
        }
    }

    private void ShootPlayer()
    {
        //var newBullet = Instantiate(Bullet, tailPointer.position, Quaternion.LookRotation((player.position - transform.position),Vector3.up));
        ObjectSpawer.Instance.GetObject(ObjectSpawer.ObjectType.EnemyBullet, tailPointer.position, Quaternion.LookRotation((player.position - transform.position), Vector3.up));
        shootCounter = 0;
        aimCounter = 0;
        projectileLine.enabled = false;
    }

    Vector3 FOVhitnew;
    public void CheckFOV()
    {

        if (Physics.CheckSphere(transform.position, lengthFOV, whatisPlayer))
        {
            isChase = true;
            StartMove();
            lengthFOV = awareFOV;
            if ((Vector3.Distance(transform.position, player.position)) < attachRange)
            {
                inAttackRange = true;
                aimCounter = 0f;
                chaseSpeed = 0;
                projectileLine.enabled = false;
                enemyAudio.enabled = false;
                if (canAttack)
                {
                    StartCoroutine(Attach());
                }
            }
            else
            {
                inAttackRange = false;
                CheckIfCanAim();
                chaseSpeed = 2.5f;
                enemyAudio.enabled = false;
            }

        }
        else
        {
            lengthFOV = non_awareFOV;
            isChase = false;
            aimCounter = 0f;
            chaseSpeed = 2.5f;
            projectileLine.enabled = false;
            enemyAudio.enabled = false;
        }

    }

    private void StartMove()
    {
        transform.LookAt(player);
        transform.position += transform.forward * chaseSpeed * Time.deltaTime;
        enemyAudio.enabled=true;
    }

    private void StartDrawLine()
    {
        /* projectileLine.enabled = true;
         projectileLine.SetPosition(0, tailPointer.position);
         projectileLine.SetPosition(1, player.position + aimerOffset);*/
        if (isChase && aimCounter >= aimBuffer)
        {
            projectileLine.enabled = true;
            projectileLine.SetPosition(0, aimer.transform.position);
            projectileLine.SetPosition(1, player.position + aimerOffset);
        }
        else
        {
            projectileLine.enabled = false;
        }
    }

    IEnumerator Attach()
    {
        Transform newLeg = FindLeg();
        Vector3 dir = (  transform.position - (player.position + playerAttackOffset)).normalized;
        TestObject.position = transform.position + (dir * attackDistance);
        newLeg.DOMove(TestObject.position, attackLeadSpeed).SetEase(attackEase);
        if (newLeg.GetComponent<EnemyAttackValidation>()!=null)
        {
            newLeg.GetComponent<EnemyAttackValidation>().isAbletoAttack = true;
        }
        
        yield return new WaitForSeconds(resetAttackDuration);
        canAttack = false;
        attackCounter = 0;
        
        if (newLeg.GetComponent<EnemyAttackValidation>() != null)
        {
            newLeg.GetComponent<EnemyAttackValidation>().isAbletoAttack = false;
        }
    }
    /*void FinalAttack()
    {
        Transform newLeg = FindLeg();
        newLeg.DOMove(player.position + playerAttackOffset , attackLeadSpeed).SetEase(Ease.InExpo);
    }*/
    private Transform FindLeg()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        foreach (var legs in closestLeg)
        {
            float dist = Vector3.Distance(legs.position, player.position);
            if (dist < minDist)
            {
                tMin = legs;
                minDist = dist;
            }
        }
        return tMin;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position, lengthFOV);
        if(isChase)
        {
            Gizmos.color = Color.red;
            Vector3 dir = (player.position - transform.position).normalized;
            Gizmos.DrawRay(transform.position, dir*4);
        }
        //Gizmos.DrawRay(aimer.transform.position + aimerOffset/2, (aimer.transform.forward)* Vector3.Distance(player.position, aimer.transform.position));
    }
    private Coroutine LookCorotine;
    public void StartRotating()
    {
        //Debug.Log("angle");
        if (LookCorotine != null)
        {
            StopCoroutine(LookCorotine);
        }
        LookCorotine = StartCoroutine(SmoothLookAT());

    }
    IEnumerator SmoothLookAT()
    {
        Debug.Log("Check1");
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(player.position.x, player.position.y, player.position.z) - transform.position);
        float time = 0;
        while (time < 1)
        {
            Debug.Log("Check22");
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * lookRotationSpeed;
            yield return null;
        }
    }
    public GameObject coin;
    public override void OnDeath()
    {
        /*gameObject.SetActive(false);
        coin.SetActive(true);*/
        if (GameManager.gameManagerInstance != null)
            GameManager.gameManagerInstance.coinsCollected++;
    }
}
