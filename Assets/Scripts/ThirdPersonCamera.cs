using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Reference")]
    public Transform Player;
    public Transform PlayerObj;
    public Transform orientation;

    public Rigidbody PlayerRB;

    public InputManager IM;

    public float RoationSpeed;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewDir = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //Vector3 inputDir = orientation.forward * IM.movementInput.y + orientation.forward * IM.movementInput.x;
        Vector3 inputDir = orientation.forward * IM.movementInput.y;

        if (inputDir!= Vector3.zero)
        {
            PlayerObj.forward = Vector3.Slerp(PlayerObj.forward, inputDir.normalized, Time.deltaTime * RoationSpeed); 
        }
    }
}
