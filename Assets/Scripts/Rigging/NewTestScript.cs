using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTestScript : MonoBehaviour
{

    public Animator _animator;
    private Vector3 rightFootPos, leftFootPos, rightFootIKPos, leftFootIkPos;
    private Quaternion leftFootIKRot, rightFootIKRot;
    private float lastPelvisPositionY, lastRightFootPosY, lastLeftFootPosY;
    [Header("FeetGrounder")]
    public bool enableFeetIk = true;
    [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float pelvisOffset = 2f;
    [Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1)] [SerializeField] private float feetToIkPositionSpeed = 0.5f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    public bool useProIkFeatures = false;
    public bool showSolverDebuger = true;
    private void FixedUpdate()
    {
        if (!enableFeetIk) return;
        if (_animator == null) return;


        AdjustFeetTarget(ref rightFootPos, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPos, HumanBodyBones.LeftFoot);

        //find and raycast to ground to find position
        FeetPositionSolver(rightFootPos, ref rightFootIKPos, ref rightFootIKRot);
        FeetPositionSolver(leftFootPos, ref leftFootIkPos, ref leftFootIKRot);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!enableFeetIk) return;
        if (_animator == null) return;
        MovepelvisHeight();
        Debug.Log(" OnAnimatorIK ");
        //rightfoot position and rotation -- utilize  the pro feature in here
        //Debug.Log("Check if entering loop");
        if (useProIkFeatures)
        {
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat(rightFootAnimVariableName));
        }
        MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPos, rightFootIKRot, ref lastRightFootPosY);

        //leftfoot position and rotation -- utilize  the pro feature in here

        if (useProIkFeatures)
        {
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat(leftFootAnimVariableName));
        }
        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIkPos, leftFootIKRot, ref lastLeftFootPosY);


    }
    void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationikHolder, ref float lastFootPositionY)
    {
        Debug.Log(" MoveFeetToIKPoint ");
        Vector3 targetIkPos = _animator.GetIKPosition(foot);
        if (positionIKHolder != Vector3.zero)
        {
            targetIkPos = transform.InverseTransformPoint(targetIkPos);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIkPositionSpeed);
            targetIkPos.y += yVariable;
            lastFootPositionY = yVariable;
            targetIkPos = transform.TransformPoint(targetIkPos);
            _animator.SetIKRotation(foot, rotationikHolder);

        }

        _animator.SetIKPosition(foot, targetIkPos);
    }
    private void MovepelvisHeight()
    {
        Debug.Log(" MovepelvisHeight ");
        if (rightFootIKPos == Vector3.zero || leftFootIkPos == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = _animator.bodyPosition.y;
            return;
        }
        float l_offsetPosition = leftFootIkPos.y - transform.position.y;
        float r_offsetPosition = rightFootIKPos.y - transform.position.y;

        float totaloffset = (l_offsetPosition < r_offsetPosition) ? l_offsetPosition : r_offsetPosition;
        Vector3 newPelvisPosition = _animator.bodyPosition + Vector3.up * totaloffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);
        _animator.bodyPosition = newPelvisPosition;
        lastPelvisPositionY = _animator.bodyPosition.y;
    }
    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPosition, ref Quaternion feetIkRotation)
    {
        Debug.Log(" FeetPositionSolver ");
        //raycast Handling
        RaycastHit feetoutHit;
        if (showSolverDebuger)
        {
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);
        }
        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetoutHit, raycastDownDistance + heightFromGroundRaycast, groundLayer))
        {
            feetIKPosition = fromSkyPosition;
            feetIKPosition.y = feetoutHit.point.y + pelvisOffset;
            feetIkRotation = Quaternion.FromToRotation(Vector3.up, feetoutHit.normal) * transform.rotation;
            return;
        }
        feetIKPosition = Vector3.zero;
    }
    private void AdjustFeetTarget(ref Vector3 feetPosition, HumanBodyBones foot)
    {
        feetPosition = _animator.GetBoneTransform(foot).position;
        feetPosition.y = transform.position.y + heightFromGroundRaycast;
    }
}
