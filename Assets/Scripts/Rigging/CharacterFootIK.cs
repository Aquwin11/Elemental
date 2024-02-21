using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterFootIK : MonoBehaviour
{
    private Animator characterAnimator = null;
    private Transform leftFootTransform = null;
    private Transform rightFootTransform = null;


    public Transform leftFootForwardReference;
    public Transform rightFootForwardReference;

    private Vector3 forwardVector;

    public LayerMask groundLayer;
    [SerializeField]
    [Range(0.1f, 2)]
    private float rayCastLength= 2;
    [SerializeField]
    [Range(0, 0.25f)]
    private float lengthFromHeelToToes = 0.1f;
    [SerializeField]
    [Range(-0.05f, 0.125f)]
    private float ankleHeightOffset = 0;


    [SerializeField]
    [Range(0, 1)]
    private float globalWeight = 1;
    [SerializeField]
    [Range(0, 1)]
    private float leftFootWeight = 1;
    [SerializeField]
    [Range(0, 1)]
    private float rightFootWeight = 1;
    [SerializeField]
    [Range(0, 0.1f)]
    private float smoothTime = 0.075f;

    private float leftFootRayHitHeight = 0;
    private float rightFootRayHitHeight = 0;


    private Vector3 leftFootRayHitProjectionVector = new Vector3();
    private Vector3 rightFootRayHitProjectionVector = new Vector3();

    private Vector3 leftFootDirectionVector = new Vector3();
    private Vector3 rightFootDirectionVector = new Vector3();

    private Vector3 leftFootProjectionVector = new Vector3();
    private Vector3 rightFootProjectionVector = new Vector3();

    private float leftFootProjectedAngle = 0;
    private float rightFootProjectedAngle = 0;

    private RaycastHit leftFootRayHit = new RaycastHit();
    private RaycastHit rightFootRayHit= new RaycastHit();

    private float leftFootRayHitProjectedAngle = 0;
    private float rightFootRayHitProjectedAngle = 0;

    private float leftFootHeightOffset = 0;
    private float rightFootHeightOffset = 0;

    private Vector3 leftFootIKPositionBuffer = new Vector3();
    private Vector3 rightFootIKPositionBuffer = new Vector3();

    private Vector3 leftFootIKPositionTarget = new Vector3();
    private Vector3 rightFootIKPositionTarget = new Vector3();

    private float leftFootHeightLerpVelocity = 0;
    private float rightFootHeightLerpVelocity = 0;



    // Start is called before the first frame update
    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        leftFootTransform = characterAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFootTransform = characterAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
        forwardVector = transform.forward;

        //Foot forward position
        leftFootForwardReference.position = leftFootTransform.position;
        rightFootForwardReference.position = rightFootTransform.position;

        leftFootForwardReference.SetParent(leftFootTransform);
        rightFootForwardReference.SetParent(rightFootTransform);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FootRaycast();

        UpdateIKPositionTarget();
    }

    private void OnAnimatorIK()
    {
        LerpIKBufferToTarget();

        ApplyFootIK();
        ApplyBodyIK();
    }

    private void FootRaycast()
    {
        //Leftleg rayCast
        Physics.SphereCast(leftFootTransform.position, 0.05f, Vector3.up * -1, out leftFootRayHit, rayCastLength, groundLayer);

        //Rightleg rayCast
        Physics.SphereCast(rightFootTransform.position, 0.05f, Vector3.up * -1, out rightFootRayHit, rayCastLength, groundLayer);


        // Leftfootray 
        if (leftFootRayHit.collider != null)
        {
            leftFootRayHitHeight = leftFootRayHit.point.y;

            // We are doing this crazy operation because we only want to count rotations that are parallel to the foot
            leftFootRayHitProjectionVector = Vector3.ProjectOnPlane(
                leftFootRayHit.normal,
                Vector3.Cross(leftFootDirectionVector, leftFootProjectionVector));

            leftFootRayHitProjectedAngle = Vector3.Angle(
                leftFootRayHitProjectionVector,
                Vector3.up);
        }
        else
        {
            leftFootRayHitHeight = transform.position.y;
        }

        // Rightfootray 
        if (rightFootRayHit.collider != null)
        {
            rightFootRayHitHeight = rightFootRayHit.point.y;

            /* Angle from the floor is also calculated to isolate the rotation caused by the animation */

            // We are doing this crazy operation because we only want to count rotations that are parallel to the foot
            rightFootRayHitProjectionVector = Vector3.ProjectOnPlane(
                rightFootRayHit.normal,
                Vector3.Cross(rightFootDirectionVector, rightFootProjectionVector));

            rightFootRayHitProjectedAngle = Vector3.Angle(
                rightFootRayHitProjectionVector,
                Vector3.up);
        }
        else
        {
            rightFootRayHitHeight = transform.position.y;
        }


    }

    private void UpdateIKPositionTarget()
    {
        /* We reset the offset values here instead of declaring them as local variables, since other functions reference it */

        leftFootHeightOffset = 0;
        rightFootHeightOffset = 0;

        /* Foot height correction based on the projected angle */

        float trueLeftFootProjectedAngle = leftFootProjectedAngle - leftFootRayHitProjectedAngle;

        if (trueLeftFootProjectedAngle > 0)
        {
            leftFootHeightOffset += Mathf.Abs(Mathf.Sin(
                Mathf.Deg2Rad * trueLeftFootProjectedAngle) *
                lengthFromHeelToToes);

            // There's no Abs here to support negative manual height offset
            leftFootHeightOffset += Mathf.Cos(
                Mathf.Deg2Rad * trueLeftFootProjectedAngle) *
                GetAnkleHeight();
        }
        else
        {
            leftFootHeightOffset += GetAnkleHeight();
        }

        /* Foot height correction based on the projected angle */

        float trueRightFootProjectedAngle = rightFootProjectedAngle - rightFootRayHitProjectedAngle;

        if (trueRightFootProjectedAngle > 0)
        {
            rightFootHeightOffset += Mathf.Abs(Mathf.Sin(
                Mathf.Deg2Rad * trueRightFootProjectedAngle) *
                lengthFromHeelToToes);

            // There's no Abs here to support negative manual height offset
            rightFootHeightOffset += Mathf.Cos(
                Mathf.Deg2Rad * trueRightFootProjectedAngle) *
                GetAnkleHeight();
        }
        else
        {
            rightFootHeightOffset += GetAnkleHeight();
        }

        /* Application of calculated position */

        leftFootIKPositionTarget.y = leftFootRayHitHeight + leftFootHeightOffset;
        rightFootIKPositionTarget.y = rightFootRayHitHeight + rightFootHeightOffset;
    }

    private void LerpIKBufferToTarget()
    {
        /* Instead of wrangling with weights, we switch the lerp targets to make movement smooth */

        if (
            characterAnimator.GetIKPosition(AvatarIKGoal.LeftFoot).y >=
            leftFootIKPositionTarget.y)
        {
            leftFootIKPositionBuffer.y = Mathf.SmoothDamp(
                leftFootIKPositionBuffer.y,
                characterAnimator.GetIKPosition(AvatarIKGoal.LeftFoot).y,
                ref leftFootHeightLerpVelocity,
                smoothTime);
        }
        else
        {
            leftFootIKPositionBuffer.y = Mathf.SmoothDamp(
                leftFootIKPositionBuffer.y,
                leftFootIKPositionTarget.y,
                ref leftFootHeightLerpVelocity,
                smoothTime);
        }

        if (
            characterAnimator.GetIKPosition(AvatarIKGoal.RightFoot).y >=
            rightFootIKPositionTarget.y)
        {
            rightFootIKPositionBuffer.y = Mathf.SmoothDamp(
                rightFootIKPositionBuffer.y,
                characterAnimator.GetIKPosition(AvatarIKGoal.RightFoot).y,
                ref rightFootHeightLerpVelocity,
                smoothTime);
        }
        else
        {
            rightFootIKPositionBuffer.y = Mathf.SmoothDamp(
                rightFootIKPositionBuffer.y,
                rightFootIKPositionTarget.y,
                ref rightFootHeightLerpVelocity,
                smoothTime);
        }
    }

    private float GetAnkleHeight()
    {
        return 0.05f + ankleHeightOffset;
    }
    private void CopyByAxis(ref Vector3 target, Vector3 source, bool copyX, bool copyY, bool copyZ)
    {
        target = new Vector3(
            Mathf.Lerp(
                target.x,
                source.x,
                Convert.ToInt32(copyX)),
            Mathf.Lerp(
                target.y,
                source.y,
                Convert.ToInt32(copyY)),
            Mathf.Lerp(
                target.z,
                source.z,
                Convert.ToInt32(copyZ)));
    }
    private void ApplyFootIK()
    {
        /* Weight designation */

        characterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, globalWeight * leftFootWeight);
        characterAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, globalWeight * rightFootWeight);

        characterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, globalWeight * leftFootWeight);
        characterAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, globalWeight * rightFootWeight);

        /* Position handling */

        CopyByAxis(ref leftFootIKPositionBuffer, characterAnimator.GetIKPosition(AvatarIKGoal.LeftFoot),
            true, false, true);

        CopyByAxis(ref rightFootIKPositionBuffer, characterAnimator.GetIKPosition(AvatarIKGoal.RightFoot),
            true, false, true);

        characterAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKPositionBuffer);
        characterAnimator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKPositionBuffer);
    }

    private void ApplyBodyIK()
    {

        float minFootHeight = Mathf.Min(
                characterAnimator.GetIKPosition(AvatarIKGoal.LeftFoot).y,
                characterAnimator.GetIKPosition(AvatarIKGoal.RightFoot).y);

        /* This part moves the body 'downwards' by the root gameobject's height */

        characterAnimator.bodyPosition = new Vector3(
        characterAnimator.bodyPosition.x,
        characterAnimator.bodyPosition.y +
        LimitValueByRange(minFootHeight - transform.position.y, 0),
        characterAnimator.bodyPosition.z);
    }
    //change this 
    private float LimitValueByRange(float value, float floor)
    {
        if (value < floor - 0)
        {
            return value + 0;
        }
        else if (value > floor + 0)
        {
            return value - 0;
        }
        else
        {
            return floor;
        }
    }

}
