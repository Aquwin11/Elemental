using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using System;

namespace StarterAssets
{
    
    public class ThirdPersonAimController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera aimCamera;
        StarterAssetsInputs _IM;
        public float aimSensitivity;
        public float normalSensitivity;
        [SerializeField]private ThirdPersonController thirdPersonController;
        public Image CrossHair;
        [SerializeField] private Rig rig;
        public float RigWeight_Value;
        public Transform aimPoint;
        public float maxRotationValue;
        private float rotationSpeed=1;

        private Coroutine LookCorotine;
        // Start is called before the first frame update
        void Start()
        {
            _IM = GetComponent<StarterAssetsInputs>();
            LoadPlayerAimData();
        }

        // Update is called once per frame
        void Update()
        {
            rig.weight = Mathf.Lerp(rig.weight, RigWeight_Value, Time.deltaTime * 20f);
            if(_IM.aim)
            {
                RigWeight_Value = 1f;
                aimCamera.gameObject.SetActive(true);
                thirdPersonController.SetSensitivity(aimSensitivity);
                CrossHair.color =new Color(CrossHair.color.r, CrossHair.color.g, CrossHair.color.b, Mathf.Lerp(CrossHair.color.a, 1, 0.25f));
                //transform.LookAt(aimPoint);
                float angle = Vector3.Angle(new Vector3(aimPoint.position.x,transform.position.y,aimPoint.position.z), transform.forward);
                //Debug.Log(angle);
                if (angle> maxRotationValue)
                {
                    transform.LookAt(new Vector3(aimPoint.position.x, transform.position.y, aimPoint.position.z));
                    //StartRotating();
                }
            }
            else
            {
                RigWeight_Value = 0f;
                aimCamera.gameObject.SetActive(false);
                thirdPersonController.SetSensitivity(normalSensitivity);
                CrossHair.color = new Color(CrossHair.color.r, CrossHair.color.g, CrossHair.color.b, Mathf.Lerp(CrossHair.color.a, 0, 0.25f));
            }
            
        }


        public void StartRotating()
        {
            //Debug.Log("angle");
            if (LookCorotine!=null)
            {
                StopCoroutine(LookCorotine);
            }
            LookCorotine = StartCoroutine(SmoothLookAT());

        }

        IEnumerator SmoothLookAT()
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(aimPoint.position.x, transform.position.y, aimPoint.position.z) - transform.forward);
            float time = 0;
            while(time<1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
                time += Time.deltaTime * rotationSpeed;
                yield return null;
            }
        }

        public void LoadPlayerAimData()
        {
            if (PlayerPrefs.HasKey("SettingsControlsSensitivity"))
            {
                normalSensitivity = PlayerPrefs.GetFloat("SettingsControlsSensitivity");
            }
        }
    }

    
}

