using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public InputManager animationIM;

    Animator animator;
    float velocityX = 0;
    float velocityZ = 0;


    public float acceleration = 2;
    public float deceleration = 2;

    public float maximumWalkVelocity = 1f;
    public float maximumRunVelocity = 2f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        float playerHorizontal = animationIM.movementInput.x;
        float playerVertical = animationIM.movementInput.y;
        bool playerSprint = animationIM.sprintInput;
        //
        float setCurrentVelocity = playerSprint ? maximumRunVelocity : maximumWalkVelocity;
        if (playerVertical>=1  && velocityZ <= setCurrentVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        if (playerVertical <= -1 && velocityZ >= -setCurrentVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }
        if (playerHorizontal<=-1 && velocityX >= -setCurrentVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        if (playerHorizontal>=1 && velocityX <= setCurrentVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }


        ///Deceleration forward
        if (playerVertical==0 && velocityZ > 0)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        ///Deceleration right
        if (playerHorizontal == 0 && !playerSprint && velocityX > 0)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        ///Deceleration Left
        if (playerHorizontal == 0 && !playerSprint && velocityX < 0)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (playerVertical == 0 && !playerSprint && velocityZ < 0)
        {
            velocityZ += Time.deltaTime * deceleration;
        }
        //Decelerate after running is done
        if (!playerSprint && velocityZ>=maximumWalkVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (!playerSprint && velocityX >= maximumWalkVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        if (!playerSprint && velocityX <= -maximumWalkVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (!playerSprint && velocityZ <= -maximumWalkVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        /*velocityX = animationIM.movementInput.x;
        velocityZ = animationIM.movementInput.y;*/
        animator.SetFloat("velcityx", velocityX);
        animator.SetFloat("velocityy", velocityZ);
       
    }
}
