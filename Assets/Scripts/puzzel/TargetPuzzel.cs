using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TargetPuzzel : MonoBehaviour
{
    public List<GameObject> puzzleElement;
    public Vector3 placementOffset;
    [Range(0, 1)] [SerializeField] float rotateTowardsNormal;
    public List<TargetParticeScript> targetParticeScripts;
    public float rotationTime;
    public bool challengeStart;
    public float challengeTimer;
    public float challengeBuffer;

    public float score;
    private void Start()
    {
        CloseallTarget();
        chooseRandomTarget();
        challengeTimer = challengeBuffer;
        challengeStart = false;
    }



    public void OnEnable()
    {
        //placeOBjectAcuratelly();
    }
    void CloseallTarget()
    {
        foreach (var item in targetParticeScripts)
        {
            item.isTargetable = false;
            item.transform.parent.DOLocalRotate(new Vector3(90, 0, 0), rotationTime);
        }
    }
    void placeOBjectAcuratelly()
    {
        
        for (int i = 0; i < puzzleElement.Count; i++)
        {
            if (!Physics.Raycast(puzzleElement[i].transform.position + placementOffset, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                continue;
            Debug.Log("hit complete" + hit.point);
            puzzleElement[i].transform.localPosition = hit.point;
            puzzleElement[i].transform.rotation = puzzleElement[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    public void chooseRandomTarget()
    {
        CloseallTarget();
        int randomint = Random.Range(0, targetParticeScripts.Count);
        Transform picked = targetParticeScripts[randomint].transform;
        picked.parent.DOLocalRotate(new Vector3(0, 0, 0), rotationTime);
        picked.GetComponent<TargetParticeScript>().isTargetable = true;
    }
    private void Update()
    {
        if(challengeTimer<challengeBuffer)
        {
            challengeTimer += Time.deltaTime;
            challengeStart = true;
            if(score>11)
            {
                if (GameManager.gameManagerInstance != null)
                    GameManager.gameManagerInstance.coinsCollected++;
            }
        }
        else
        {
            challengeStart = false;
            score = 0;
        }
    }
}
