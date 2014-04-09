using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Animator))]
public class NumberAnimator : MonoBehaviour 
{
    // reference the Animator
    protected Animator animator;

    // reference the current state of the Animator
    protected AnimatorStateInfo currentBaseState;

 
    // Get the states for this character
    public static int idleState = Animator.StringToHash("Base Layer.Idle"); 
    public static int screamingState = Animator.StringToHash("Base Layer.Screaming");
    public static int jumpingState = Animator.StringToHash("Base Layer.Jumping");
    public static int grabbingSignState = Animator.StringToHash("Base Layer.Grab Sign Behind Head");
    public static int hitSidewaysRightState = Animator.StringToHash("Base Layer.Hit Sideways Right");

    public static int turnLeftState = Animator.StringToHash("Base Layer.Turn Left");
    public static int turnRightState = Animator.StringToHash("Base Layer.Turn Right");
    public static int castFireballState = Animator.StringToHash("Base Layer.Cast Fireball");

    public static int walkingState = Animator.StringToHash("Base Layer.Walking"); // not really needed at the moment, but it will be when we want to make a transition from this state
	
	public enum STATES
	{
		
	}
	
	void Start () 
    {
        // Get the attached Animator component
	    animator = GetComponent<Animator>();
	}
	
	public void TurnLeft()
	{
		StartCoroutine( TurnLeftRoutine() );
	}
	
	public void CastFireball()
	{
		StartCoroutine(CastFireballRoutine());
	}
	
	public IEnumerator TurnLeftRoutine()
	{
		yield return StartCoroutine( SetBool("turnLeft") );
	}
	
	public IEnumerator CastFireballRoutine()
	{
		yield return StartCoroutine( SetBool("CastFireball") );
	}
	
	protected IEnumerator SetBool(string boolName)
	{
		Debug.LogError("Setting bool " + boolName);
		
		animator.SetBool(boolName, true);
		
		yield return new WaitForSeconds(0.2f);
		
		animator.SetBool(boolName, false);
		
		Debug.LogError("Setting bool " + boolName + " Done " + currentBaseState.length + " -> " + currentBaseState.normalizedTime );
		
		yield return new WaitForSeconds( currentBaseState.normalizedTime );
		
	}
	
	
	
	void FixedUpdate()
    {
		/*
        // get the vertical input...
        float speedInput = Input.GetAxis("Vertical");

        // ... and this sets the speed
        animator.SetFloat("Speed", speedInput);
		 */
		
        // set the currentState to the current state of the Base Layer (0) of animation
        currentBaseState = animator.GetCurrentAnimatorStateInfo(0);
		
		/*
        // if he's screaming...
        if (currentBaseState.nameHash == screamingState)
        {
            // ... and his not still in the transition to Screaming...
            if (!animator.IsInTransition(0))
            {
                // ... he has arrived in the Screaming state, so the bool can be set to false
                // Transition from this state back to Idle is handled in the State Machine transition from Screaming to Idle (Exit Time)
                animator.SetBool("Screaming", false);
            }
        }


        else if (currentBaseState.nameHash == jumpingState)
        {
            if (!animator.IsInTransition(0))
            {
                animator.SetBool("Jumping", false);
            }
        }

        else if (currentBaseState.nameHash == grabbingSignState)
        {
            if (!animator.IsInTransition(0))
            {
                animator.SetBool("GrabSign", false);
            }
        }

        else if (currentBaseState.nameHash == hitSidewaysRightState)
        {
            if (!animator.IsInTransition(0))
            {
                animator.SetBool("HitSidewaysRight", false);
            }
        }

        else if (currentBaseState.nameHash == turnLeftState)
        {
            if (!animator.IsInTransition(0))
            {
                animator.SetBool("TurnLeft", false);
            }
        }

        else if (currentBaseState.nameHash == turnRightState)
        {
            if (!animator.IsInTransition(0))
            {
                animator.SetBool("turnRight", false);
            }
        }

        else if (currentBaseState.nameHash == castFireballState)
        {
            if (!animator.IsInTransition(0))
            {
                animator.SetBool("CastFireball", false);
            }
        } 
        */
    }
	
	/*
    void OnGUI()
    { 
        int width = 150; 

        if (currentBaseState.nameHash == idleState)
        {
            if (GUI.Button(new Rect(0, 10, width, 30), "Hit Sideways Right"))
                animator.SetBool("HitSidewaysRight", true);
            if (GUI.Button(new Rect(width + 10, 10, width, 30), "Turn Left"))
                animator.SetBool("TurnLeft", true);
            if (GUI.Button(new Rect((width * 2) + 20, 10, width, 30), "Cast Fireball"))
                animator.SetBool("CastFireball", true);
        }
    }
    */
}