using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPuzzle : MonoBehaviour
{
    public BoxCollider selfCollider; 
    public List<GameObject> puzzleElement;
    public float minHeight;
    public float maxHeight;
    public Vector3 minScale;
    public Vector3 maxScale;
    public Vector3 placementOffset;
    [Range(0, 1)] [SerializeField] float rotateTowardsNormal;
    public List<RockHolder> rockHolders;
    int num ;

    private void Start()
    {
        //placeOBjectAcuratelly();
    }

    private void OnEnable()
    {
        //Debug.Log("Check if instantiated");
        gameObject.GetComponent<BoxCollider>().enabled = false;
        //placeOBjectAcuratelly();

    }
    public void CheckIfPuzzleComplete()
    {
        num = 0;
        foreach (var item in rockHolders)
        {
            if(item.rockInPlace)
            {
                num += 1;
            }
        }
        if(num==rockHolders.Count)
        {
            if (GameManager.gameManagerInstance != null)
                GameManager.gameManagerInstance.coinsCollected++;
        }

    }
    public void  placeOBjectAcuratelly()
    {
        for (int i = 0; i < puzzleElement.Count; i++)
        {
            Vector3 rayStart = new Vector3(puzzleElement[i].transform.position.x, maxHeight, puzzleElement[i].transform.position.x);
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                continue;
            if (hit.point.y < minHeight)
                continue;
            puzzleElement[i].transform.position = hit.point + placementOffset;
            puzzleElement[i].transform.transform.rotation = Quaternion.Lerp(transform.rotation,
                transform.rotation * Quaternion.FromToRotation
                (puzzleElement[i].transform.transform.up, hit.normal), rotateTowardsNormal);
            puzzleElement[i].transform.transform.localScale = new Vector3(Random.Range(minScale.x, maxScale.x),
                Random.Range(minScale.y, maxScale.y),
                Random.Range(minScale.z, maxScale.z));
        }
        
    }
}
