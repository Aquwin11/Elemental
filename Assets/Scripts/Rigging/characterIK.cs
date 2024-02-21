using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Animator))]
public class characterIK : MonoBehaviour
{
    protected Animator animator;


    public float weightL;
    public float weightR;

    public Vector3 l_footIKOffset;
    public Vector3 r_footIKOffset;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK()
    {
        Vector3 p_leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        Vector3 p_rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

        p_leftFoot = GetHitPoint(p_leftFoot + Vector3.up, p_leftFoot - Vector3.up*5)+l_footIKOffset;
        p_rightFoot = GetHitPoint(p_rightFoot + Vector3.up, p_rightFoot - Vector3.up*5 + r_footIKOffset);


        //transform.localPosition = new Vector3(transform.localPosition.x, -Mathf.Abs(p_leftFoot.y - p_rightFoot.y) / 2, transform.localPosition.z);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weightL);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weightR);


        animator.SetIKPosition(AvatarIKGoal.LeftFoot, p_leftFoot) ;
        animator.SetIKPosition(AvatarIKGoal.RightFoot, p_rightFoot) ;
        Debug.Log("Check IK");

        Debug.Log(" Left Difference between leg and ground " + Vector3.Distance(p_leftFoot, animator.GetBoneTransform(HumanBodyBones.LeftFoot).position));

    }


    private Vector3 GetHitPoint(Vector3 start,Vector3 end)
    {
        RaycastHit Hit;
        if(Physics.Linecast(start,end,out Hit))
        {
            return Hit.point;
        }
        return end;
    }
}
