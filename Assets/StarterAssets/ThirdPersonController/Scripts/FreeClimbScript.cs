using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

namespace StarterAssets
{
    
    public class FreeClimbScript : MonoBehaviour
    {
        bool inPostion;
        float posT;
        Vector3 startPos;
        Vector3 TargetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float positionOffset;
        public float dis2;
        public float forceToApply;
        public bool isAbleToMove;
        Transform helper;
        [SerializeField] ThirdPersonController controller;
        [SerializeField] StarterAssetsInputs _inputManager;
        public float climbspeed;
        public float offsetFromWall;
        private Quaternion originalRotation;
        public float rotateSpeed;
        public float positionLerpSpeed;
        public RaycastHit newHit;
        [SerializeField] CharacterController Movecontroller;
        public Transform PlayerModel;

        [Header("IK")]
        public  Vector3 r_handIKOffset;
        public  Vector3 l_handIKOffset;
        public  Transform r_raycastDirection;
        public  Vector3 new_r_raycastDirection;
        public  Transform l_raycastDirection;
        public TwoBoneIKConstraint rightIkConstrain;
        public TwoBoneIKConstraint leftIkConstrain;
        public Transform rightHandTarget;
        public Transform leftHandTarget;
        public float handRayLength;
        public Transform rightHand;
        public Transform leftHand;



        [Header("Climb Ledge")]
        public bool canClimbLedge;
        public Transform headPos;
        public Transform leftPos;
        public Transform rightPos;
        public float raycastLength;
        public Vector3 ledgeOffset;
        public Vector3 vaultingOffsetValue;
        public float vaultingDuration;
        public float vaultingDuration1;
        public float vaultingDuration2;
        public bool isVaulting=false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            Tick(delta);
            MoveOnWall();
            ClimbIKHands();
            //ClimbMovements();
            CheckIfledge();
        }

        private void ClimbIKHands()
        {
            RaycastHit rHit;
            Vector3 r_dir = ( r_raycastDirection.position- rightHand.position).normalized;
            //Debug.Log("RightShoulderDIrection" + r_dir);
            Debug.DrawRay(rightHand.position, new_r_raycastDirection * handRayLength,Color.black);
            if(Physics.Raycast(rightHand.position, new_r_raycastDirection, out rHit, handRayLength))
            {
                //Debug.Log("printCheck");
                rightHandTarget.position = Vector3.Lerp(rightHandTarget.position, rHit.point + r_handIKOffset, 25f * Time.deltaTime);
            }
            RaycastHit lHit;
            if (Physics.Raycast(leftHand.position, l_raycastDirection.position, out lHit, handRayLength))
            {
                leftHandTarget.position = Vector3.Lerp(leftHandTarget.position, lHit.point + l_handIKOffset, 25f * Time.deltaTime);
            }
        }

        private void Tick(float delta)
        {
            if (!inPostion)
            {
                GetInPosition(delta);
                return;
            }
        }
        private void OnEnable()
        {
            helper = new GameObject().transform;
            helper.name = "ClimbHelper";
            originalRotation = transform.rotation;
            //onClimbstart();
        }

        private void OnDisable()
        {
            if(helper!=null)
            {
                Destroy(helper.gameObject);
            }
            transform.rotation = originalRotation;
        }

        public void InitForClimb(RaycastHit hit)
        {
            newHit = hit;
            helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
            startPos = transform.position;
            TargetPos = hit.point + (hit.normal * offsetFromWall);
            posT = 0;
            inPostion = false;
            //Debug.Log("Start " + transform.position + " OutputHit " + newHit.point);
        }
        void MoveOnWall()
        {
            Vector3 h = helper.right * _inputManager.move.x;
            Vector3 v = helper.up * _inputManager.move.y;
            if(_inputManager.move.y==-1)
            {
                CheckIfGround();
            }
            Vector3 moveDir = (h + v).normalized;
            if(!isVaulting)
            {
                isAbleToMove = canMove(moveDir) && !controller.isStillMoving;
            }
            if(!isAbleToMove || moveDir==Vector3.zero)
            {
                return;
            }
            Movecontroller.Move(moveDir * climbspeed * Time.deltaTime);
        }

        private void CheckIfGround()
        {
            Vector3 origin = transform.position + controller.rayHeightoffset;
            float dis = positionOffset * 2.5f;
            Vector3 dir = Vector3.down;
            Debug.DrawRay(origin, dir * dis, Color.green);
            RaycastHit Hit;
            if (Physics.Raycast(origin, dir, out Hit, dis))
            {
                controller.DropFromClimb();
            }
        }

        public float sphereRadius;
        public float sphereMaxDistance;
        private void CheckIfledge()
        {
            RaycastHit sphereHit;
            Vector3 origin = headPos.transform.position + ledgeOffset;
            Vector3 dir = helper.forward;

            // Regular raycast
            Debug.DrawRay(origin, dir * raycastLength, Color.green);
            if (!Physics.Raycast(origin, dir, raycastLength))
            {
                // Sphere cast at the end of the raycast
                if (Physics.SphereCast(origin + (dir * raycastLength), sphereRadius, -helper.up, out sphereHit))
                {
                    //Debug.Log(origin);
                    StartCoroutine(LerpToPositionY(origin+(dir*(raycastLength/2))+vaultingOffsetValue,vaultingDuration));
                }
                else
                {
                    isVaulting = false;
                }
            }
            else
            {
                canClimbLedge = false;
                isVaulting = false;
            }
        }

        IEnumerator LerpToPositionY(Vector3 targetPosition,float duration)
        {
            isVaulting = true;
            Movecontroller.detectCollisions = false;
            yield return new WaitForSeconds(vaultingDuration2);
            transform.DOLocalMove(targetPosition, duration);
            controller.checkIfMove(duration);
            yield return new WaitForSeconds(vaultingDuration1);
            Movecontroller.detectCollisions = true;
            isVaulting = false;
        }



        bool canMove(Vector3 moveDirection)
        {
            if (isVaulting)
                return false;
            Vector3 origin = transform.position+ controller.rayHeightoffset;
            float dis = positionOffset;
            Vector3 dir = moveDirection;
            Debug.DrawRay(origin, dir * dis,Color.red);
            RaycastHit Hit;
            if(Physics.Raycast(origin,dir, out Hit,dis))
            {
                return false;
            }
            origin += moveDirection * dis;
            dir = helper.forward;
            Debug.DrawRay(origin, dir * dis2, Color.blue);
            if(Physics.Raycast(origin,dir,out Hit,dis))
            {
                helper.position = PositionWithOffset(origin, Hit.point);
                helper.rotation = Quaternion.LookRotation(-Hit.normal);
                return true;
            }
            origin += dir * dis2;
            dir = -Vector3.up;
            Debug.DrawRay(origin, dir,Color.yellow);
            if(Physics.Raycast(origin,dir,out Hit,dis2))
            {
                float angle = Vector3.Angle(helper.up, Hit.normal);
                if(angle<40)
                {
                    helper.position = PositionWithOffset(origin, Hit.point);
                    helper.rotation = Quaternion.LookRotation(-Hit.normal);
                    return true;
                }
            }
            return false;
        }


        void GetInPosition(float delta)
        {
            posT += delta;
            if(posT>1)
            {
                posT = 1;inPostion = true;
            }
            helper.transform.rotation = Quaternion.LookRotation(-newHit.normal);
            float TargetDist = Vector2.Distance((transform.position+controller.rayHeightoffset),newHit.point);
            Vector3 dirDiff = (newHit.point- (transform.position + controller.rayHeightoffset));
            transform.rotation = Quaternion.Slerp(transform.rotation,helper.rotation, Time.deltaTime * rotateSpeed);

        }

        Vector3 PositionWithOffset(Vector3 origin , Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * offsetFromWall;
            return target + offset;
        }

        
        /*private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            if(helper!=null)
            {
                Vector3 h = helper.right * _inputManager.move.x;
                Vector3 v = helper.up * _inputManager.move.y;
                Vector3 moveDir = (h + v).normalized;
                Gizmos.DrawRay(transform.position, moveDir * positionOffset);
                //Debug.Log("KeepmovingtoPoint1 " + moveDir );
                Gizmos.DrawSphere(headPos.transform.position + ledgeOffset + (helper.forward * raycastLength), sphereRadius);
            }
            Gizmos.DrawRay(transform.position + controller.rayHeightoffset, transform.forward * controller.climb_raylength);
            
        }*/
    }
}

