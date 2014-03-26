using UnityEngine;
using System.Collections;


// An Animator component is required for this script to work
[RequireComponent(typeof (Animator))]

public class character_control_script : MonoBehaviour 
{
//    // reference the Animator
//    private Animator animator;
//
//    // reference the current state of the Animator
//    private AnimatorStateInfo currentBaseState;
//    // reference to the second animation layer
//    private AnimatorStateInfo facialAnimationState;
//    // reference to the fourth animation layer
//    private AnimatorStateInfo upperBodyState;
//
//
// 
//    // Get the states for this character
//    static int idleState = Animator.StringToHash("Base Layer.Idle"); 
//    static int screamingState = Animator.StringToHash("Base Layer.Screaming");
//    static int jumpingState = Animator.StringToHash("Base Layer.jumpInPlace");
//    static int grabbingSignState = Animator.StringToHash("Base Layer.Grab Sign Behind Head");
//    static int hitSidewaysRightState = Animator.StringToHash("Base Layer.Hit Sideways Right");
//
//    static int turnLeftState = Animator.StringToHash("Base Layer.turnLeft");
//    static int turnRightState = Animator.StringToHash("Base Layer.turnRight");
//    static int castFireballState = Animator.StringToHash("Base Layer.castFireball");
//
//    //static int walkingState = Animator.StringToHash("Base Layer.Walking"); // not really needed at the moment, but it will be when we want to make a transition from this state
//
//
//    static int bigMaskLookAngryState = Animator.StringToHash("bigMaskFacialAnimations.bigMaskLookAngry");
//    static int mediumMaskLookAngryState = Animator.StringToHash("mediumMaskFacialAnimations.mediumMaskLookAngry");
//    static int smallMaskLookAngryState = Animator.StringToHash("smallMaskFacialAnimations.smallMaskLookAngry");
//
//    static int jump_and_turnState = Animator.StringToHash("Base Layer.jumpAndTurn");
//
//    static int castOnlyUpperBodyState = Animator.StringToHash("castOnlyUpperBody.castOnlyUpperBody");
//
//
//
//    private bool lookAngry = false;
//    private bool running = false;
//    private bool floating = false;
//
//    // This has to be set in Unity for the big, small and medium masks
//    public int eyeBrowAnimationLayer = 1;
//
//	void Start () 
//    {
//        // Get the attached Animator component
//	    animator = GetComponent<Animator>();
//
//        // For some reason, this weight gets set to 0 when the game starts, eventhough it shows 1 in the animator component
//        // This sets the weight to the appropriate animation layer for eyebrow animation
//        // 1 = the big masks
//        // 2 = the medium masks
//        // 3 = small mask
//        animator.SetLayerWeight(eyeBrowAnimationLayer, 1);
//
//
//        // Animation layer 4 is the animation of the upper body casting a fireball, without any animation on the lower body. 
//        // This animation is available for all masks, so its animation layer should have its weight set to 1. Always. 
//        animator.SetLayerWeight(4, 1);
//	}
//	
//	void FixedUpdate()
//    {
//        // set the currentState to the current state of the Base Layer (0) of animation
//        currentBaseState = animator.GetCurrentAnimatorStateInfo(0);
//        
//        //if there is more than one animation layer, set the references
//        if (animator.layerCount > 1)
//        {
//            facialAnimationState = animator.GetCurrentAnimatorStateInfo(1);
//            upperBodyState = animator.GetCurrentAnimatorStateInfo(4);
//        }
//
//
//        // As soon as a boolean is set to true, immediately set it to false, otherwise it'll keep looping
//        if (upperBodyState.nameHash == castOnlyUpperBodyState)
//            animator.SetBool("CastOnlyUpperBody", false);
//
//        else if (currentBaseState.nameHash == jumpingState)
//            animator.SetBool("Jump", false);
//
//        else if (currentBaseState.nameHash == grabbingSignState)
//            animator.SetBool("GrabSign", false);
//
//        else if (currentBaseState.nameHash == hitSidewaysRightState)
//            animator.SetBool("HitSidewaysRight", false);
//
//        else if (currentBaseState.nameHash == turnLeftState)
//            animator.SetBool("TurnLeft", false);
//
//        else if (currentBaseState.nameHash == turnRightState)
//            animator.SetBool("TurnRight", false);
//
//        else if (currentBaseState.nameHash == castFireballState)
//            animator.SetBool("CastFireball", false);
//
//        else if (currentBaseState.nameHash == jump_and_turnState)
//            animator.SetBool("JumpAndTurn", false);
//        
//
//
//        if (lookAngry == true)
//            animator.SetBool("LookAngry", true);
//        else
//            animator.SetBool("LookAngry", false);
//    }
//
//    void OnGUI()
//    {
//		if( !LuGusUtil.debug )
//			return;
//		
//        int width = 150; 
//
//        if (GUI.Button(new Rect(0, 10, width, 30), "Jump"))
//            animator.SetBool("Jump", true);
//        if (GUI.Button(new Rect(width + 10, 10, width, 30), "Turn Left"))
//            animator.SetBool("TurnLeft", true);
//        if (GUI.Button(new Rect(width + 10, 50, width, 30), "Turn Right"))
//            animator.SetBool("TurnRight", true);
//
//        if (GUI.Button(new Rect((width * 2) + 20, 10, width, 30), "Cast Fireball"))
//            animator.SetBool("CastFireball", true);
//
//        if (GUI.Button(new Rect((width * 3) + 30, 10, width, 30), "Toggle Run"))
//        {
//            animator.SetBool("Running", !running);
//            running = !running;
//        }
//
//        if (GUI.Button(new Rect((width * 3) + 30, 50, width, 30), "Toggle float"))
//        {
//            animator.SetBool("Floating", !floating);
//            floating = !floating;
//        }
//
//        if (GUI.Button(new Rect((width * 3) + 30, 90, width, 30), "Upp. Body Cast"))
//            animator.SetBool("CastOnlyUpperBody", true);
//
//        if (GUI.Button(new Rect((width * 4) + 40, 10, width, 30), "Toggle Angry"))
//            lookAngry = !lookAngry;
//
//
//        if (GUI.Button(new Rect((width * 5) + 50, 10, width, 30), "Turn Left And Shoot"))
//            StartCoroutine(TurnAndShoot("TurnLeft"));
//        if (GUI.Button(new Rect((width * 5) + 50, 50, width, 30), "Turn Right And Shoot"))
//            StartCoroutine(TurnAndShoot("TurnRight"));
//
//
//        if (GUI.Button(new Rect((width * 6) + 60, 10, width + 20, 30), "Jump left and shoot"))
//            StartCoroutine(JumpTurnAndShoot());
//        if (GUI.Button(new Rect((width * 6) + 60, 50, width, 30), "Jump left"))
//            animator.SetBool("JumpAndTurn", true);
//
//
//    }
//
//    IEnumerator TurnAndShoot(string boolToSet)
//    {
//        animator.SetBool(boolToSet, true);
//        yield return new WaitForSeconds(0.1f);
//        animator.SetBool("CastFireball", true);
//    }
//
//    IEnumerator JumpTurnAndShoot()
//    {
//        animator.SetBool("JumpAndTurn", true);
//        yield return new WaitForSeconds(0.4f);
//        animator.SetBool("CastOnlyUpperBody", true); 
//    }
}