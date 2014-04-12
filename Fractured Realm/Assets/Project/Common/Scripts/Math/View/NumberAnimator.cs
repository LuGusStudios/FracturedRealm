using UnityEngine;
using System.Collections;

public class NumberAnimator : MonoBehaviour 
{
	protected NumberRenderer _renderer = null;
	public NumberRenderer Renderer
	{
		get
		{ 	
			if( _renderer == null )
				_renderer = GetComponent<NumberRenderer>();
			
			return _renderer; 
		}
		set{ _renderer = value; }
	}

	public Coroutine RotateTowards( Vector3 target )
	{
		Coroutine output = null;

		foreach( Character c in Renderer.Characters )
		{
			if( _renderer.interactionCharacter == c )
				output = c.Animator.RotateTowards( target );
			else
				c.Animator.RotateTowards( target );
		}

		return output;
	}
	
	public Coroutine RotateInDirection(Vector3 direction)
	{
		Coroutine output = null;
		
		foreach( Character c in Renderer.Characters )
		{
			if( _renderer.interactionCharacter == c )
				output = c.Animator.RotateInDirection( direction );
			else
				c.Animator.RotateInDirection( direction );
		}
		
		return output;
	}


	public Coroutine MoveTo( Vector3 target )
	{
		gameObject.MoveTo( target ).Time ( 2.0f ).Execute();

		foreach( Character c in Renderer.Characters )
		{
			c.Animator.CrossFade( FRAnimation.running, 0.05f );
		}
		
		return StartCoroutine( MoveToRoutine(2.0f) );
	}

	protected IEnumerator MoveToRoutine( float duration )
	{
		yield return new WaitForSeconds( duration );
		
		foreach( Character c in Renderer.Characters )
		{
			c.Animator.CrossFade( FRAnimation.idle, 0.05f );
		}
	}


	
	public void SetupLocal()
	{

	}
	
	public void SetupGlobal()
	{

	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
		
	}
}