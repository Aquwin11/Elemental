using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRollScript : MonoBehaviour
{
    public Rigidbody rigidbody;
    public bool canRoll;
    public bool canJump;
    public float force;
    public float forceJump;
    public Vector3 dirForce;
    public float rollingDuration;
    public float jumpDuration;
    // Update is called once per frame


    private void OnEnable()
    {
        canRoll = true;
        canJump = true;
    }
    private void Update()
    {
        if(canRoll)
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);

        }
    }

    public void AddForceOnRock()
    {
        rigidbody.AddForce(dirForce * force,ForceMode.Impulse);
        canRoll = false;
        StopCoroutine(ResetTime());
        StartCoroutine(ResetTime());
    }

    IEnumerator ResetTime()
    {
        yield return new WaitForSeconds(rollingDuration);
        canRoll = true;
    }

    public void AddForceOnRockJump()
    {
        canJump= false;
        rigidbody.AddForce(new Vector3(rigidbody.velocity.x, 1 * forceJump, rigidbody.velocity.z), ForceMode.Impulse);
        StopCoroutine(ResetJumpTime());
        StartCoroutine(ResetJumpTime());
    }

    IEnumerator ResetJumpTime()
    {
        yield return new WaitForSeconds(jumpDuration);
        canJump = true;
    }
}
