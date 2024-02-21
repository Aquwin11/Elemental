using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class DashMech : MonoBehaviour
    {
        
        [Header("Reference")]
        public StarterAssetsInputs playerIM;
        public CharacterController characterController;
        public ThirdPersonController movementScript;
        //public Transform playerCamera;
        public Transform playerOrientation;
        public Renderer characterRenderer;

        [Header("Dash")]
        public float DashSpeed = 10f;
        public float DashDuration;
        public float DashCooldown = 1f;
        private float dashCooldownTimer = 0f;
        [SerializeField] float dashEnergyConsumption;

        //public bool resetVal = true;
        public bool allowAllDirection;
        [Header("Effects")]
        public AudioClip dashClip;
        public AudioSource dashSource;
        public float waterDissolveSpeed;
        public float waterRevertSpeed;

        public GameObject dashParticle;
        public GameObject dashTrail;

        [SerializeField] EnergyScript energyScript;

        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            movementScript = GetComponent<ThirdPersonController>();
        }




        // Update is called once per frame
        void Update()
        {
            if (playerIM.dash && !energyScript.energyReset)
            {
                Dashing();
                
            }


            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= Time.deltaTime;
            }
            foreach (var item in characterRenderer.materials)
            {
                item.SetFloat("_Dissolve", disValue);
            }
        }
        private Vector3 delayForceToApply;
        public void Dashing()
        {
            //dashSource.PlayOneShot(dashClip);
            if (dashCooldownTimer > 0) return;
            else dashCooldownTimer = DashCooldown;
            movementScript.isDashing = true;
            Transform forwardT;
            forwardT = playerOrientation;

            Vector3 direction = GetDirection(forwardT);


            Vector3 forceApply = direction * DashSpeed;

            delayForceToApply = forceApply;
            energyScript.ConsumeEnergy(dashEnergyConsumption);
            Invoke(nameof(DelayAddForce), 0.00f);

            Invoke(nameof(ResetDash), DashDuration);
            Invoke(nameof(DisableTrail), DashDuration + 0.85f);
        }

        [Range(-1.35f,1.5f)]public float disValue;
        private void DelayAddForce()
        {
            StartCoroutine(DissolveintoWater());
            Invoke("ReadytoStartBody", 0.5f);
            //playerRigidBody.AddForce(delayForceToApply, ForceMode.Impulse);
            characterController.Move(delayForceToApply * Time.deltaTime);
        }

        IEnumerator DissolveintoWater()
        {
           while(disValue<1.5f)
           {
                disValue += waterDissolveSpeed * Time.deltaTime;
           }
            dashParticle.SetActive(true);
            dashSource.PlayOneShot(dashClip);
            dashTrail.SetActive(true);
            yield return new WaitUntil(() => disValue >= 1.5f);

            //StartCoroutine(RevertToBody());
        }
        void ReadytoStartBody()
        {
            StopAllCoroutines();
            StartCoroutine(RevertToBody());
        }
        IEnumerator RevertToBody()
        {
            while (disValue >= -1.35f)
            {
                disValue -= waterRevertSpeed * Time.smoothDeltaTime;
            }
            dashParticle.SetActive(false);
            yield return new WaitUntil(() => disValue <= 1.35f);
            StopAllCoroutines();
        }
        private void ResetDash()
        {
            movementScript.isDashing = false;
            
        }

        private void DisableTrail()
        {
            dashTrail.SetActive(false);
        }
        private Vector3 GetDirection(Transform forwardT)
        {
            Vector3 direction = new Vector3();
            if (allowAllDirection)
            {
                direction = forwardT.forward * playerIM.move.y + forwardT.right * playerIM.move.x;
            }
            if (playerIM.move.x == 0 && playerIM.move.y == 0)
            {
                direction = forwardT.forward;
            }
            return direction.normalized;
        }
    }
}
