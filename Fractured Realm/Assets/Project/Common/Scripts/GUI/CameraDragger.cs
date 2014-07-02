using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDragger : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		GameManager.use.onStateChanged += OnGameStateChanged;
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public float followSpeed = 5.0f;

	protected bool tracking = false;
	protected WorldPart worldPart = null;
	protected Camera referenceCam = null;
	protected InteractionGroup interactionGroup = null;
	protected void OnGameStateChanged( FR.GameState oldState, FR.GameState newState )
	{
		if( newState != FR.GameState.WaitingForInput )
		{
			tracking = false;
			return;
		}

		GameManager gm = GameManager.use;

		if( gm.currentWorld.HasNumerator() )
		{
			worldPart = gm.currentWorld.numerator;
			referenceCam = LugusCamera.numerator;
		}
		else // only denominator
		{
			worldPart = gm.currentWorld.denominator;
			referenceCam = LugusCamera.denominator;
		}

		interactionGroup = worldPart.InteractionGroups[ gm.currentInteractionGroupIndex ];

		targetPosition = referenceCam.transform.position.x;

		tracking = true;
	}

	protected float targetPosition = float.MaxValue;
	protected bool dragging = false;
	protected Vector3 startPosScreen = Vector3.zero;
	protected void Update () 
	{
		if( !tracking )
			return;

		// TODO: make this better!
		// shouldn't just be dependent on Collider2D of course, too inflexible
		// possibility: add bool InteractionClaimed on LugusInput that is set by the MathInputManager etc. 
		// this script then waits 1 frame before acting upon the down event

		// should also check if we're not hitting a Character -> then we prefer to have MathInputManager manage it, even if it's in the SlideScreenArea

		if( LugusInput.use.down )
		{
			if( FRCamera.use.SlideScreenArea.Contains( LugusInput.use.lastPoint ) )
			{
				// if nothing is hit, we can use the dragger
				// "nothing" here means: nothing with a 2D collider (FIXME: should be more robust, see above)
				Transform hit = LugusInput.use.RayCastFromMouseDown();
				if( hit != null && hit.GetComponent<Collider2D>() != null )
					return;

				startPosScreen = LugusInput.use.lastPoint;
				dragging = true;
			}
		}
		else if( this.dragging && LugusInput.use.dragging )
		{

			// TODO: FIXME: make work with denom only!!!
			InteractionGroup intGroup = worldPart.InteractionGroups[ GameManager.use.currentInteractionGroupIndex ];
			float minX = intGroup.Camera.position.x - 5.0f; // eyeballing
			float maxX = intGroup.PortalExitCamera.x;
			

			float displacementScreenX = LugusInput.use.lastPoint.x - startPosScreen.x;
			float percentage = Mathf.Abs(displacementScreenX) / ( (float) Screen.width);

			startPosScreen = LugusInput.use.lastPoint; // prepare for next Update()

			//Debug.LogError("drag // " + intGroup.Camera.position + " - " + intGroup.PortalExitCamera + " -> " + displacementScreenX + " #" + percentage);


			percentage *= 100.0f; // eyeballing

			float worldDisplacement = Mathf.Abs( maxX - minX ) * percentage;

			
			//Debug.Log("DISPLACE : " + worldDisplacement + " * " + percentage + " = " + this.transform.position.x + " = " + Mathf.Abs( maxX - minX ) );

			// reversed moving logic ("reverse scrolling") feels more natural
			if( displacementScreenX < 0 ) // moving left with finger, moving right with camera
			{
				targetPosition = LugusCamera.numerator.transform.position.x + worldDisplacement;
			}
			else
			{
				targetPosition = LugusCamera.numerator.transform.position.x - worldDisplacement;
			}

			if( targetPosition > maxX )
				targetPosition = maxX;
			if( targetPosition < minX )
				targetPosition = minX;

			// TODO: make sure we keep arrow's positive / negative state in mind!
			//Debug.LogError("re-creating all arrows");
			UpdateHUDArrows();
		}
		else if( this.dragging && LugusInput.use.up )
		{
			dragging = false;

			InteractionGroup intGroup = worldPart.InteractionGroups[ GameManager.use.currentInteractionGroupIndex ];
			targetPosition = intGroup.Camera.position.x;
		}
		else
		{			
			InteractionGroup intGroup = worldPart.InteractionGroups[ GameManager.use.currentInteractionGroupIndex ];
			targetPosition = intGroup.Camera.position.x;

			float distance = Mathf.Abs ( targetPosition - LugusCamera.numerator.transform.position.x );
			if( distance > 0.1f )
			{
				UpdateHUDArrows();
			}
		}



		//Debug.Log("Target position: " + targetPosition);

		//this.transform.position = this.transform.position.x ( Mathf.Lerp(this.transform.position.x, targetPosition, Time.deltaTime) );
		//this.transform.position = this.transform.position.x ( targetPosition );

		//LugusCamera.numerator.transform.position = LugusCamera.numerator.transform.position.x ( targetPosition );
		//LugusCamera.denominator.transform.position = LugusCamera.denominator.transform.position.x ( targetPosition );
		
		LugusCamera.numerator.transform.position = LugusCamera.numerator.transform.position.x ( Mathf.Lerp(LugusCamera.numerator.transform.position.x, targetPosition, Time.deltaTime * followSpeed) );
		LugusCamera.denominator.transform.position = LugusCamera.denominator.transform.position.x ( Mathf.Lerp(LugusCamera.denominator.transform.position.x, targetPosition, Time.deltaTime * followSpeed) );
	}

	protected void UpdateHUDArrows()
	{
		MathInputManager mim = MathInputManager.use;
		mim.arrow1.CreateArrowFromGameWorldPosition();
		mim.arrow2.CreateArrowFromGameWorldPosition();
		mim.arrow3.CreateArrowFromGameWorldPosition();
	}
}
