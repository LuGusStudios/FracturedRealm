using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour 
{
	public bool changeTextures = true;
	public Texture2D activeTexture = null;
	public Texture2D inactiveTexture = null;
	
	public bool scale = true;
	public Vector3 originalScale = Vector3.zero;
	
	public float scaleFactor = 1.3f;
	
	public AudioClip pressSound = null;
	public AudioClip hoverSound = null;
	
	protected bool _pressed = false;
	public bool pressed
	{
		get
		{
			bool temp = _pressed;
			_pressed = false;
			return temp; 
		}
		
		set{ _pressed = value; }
	}	
	
	// Use this for initialization
	void Awake () 
	{
		if( inactiveTexture == null )
			inactiveTexture = (Texture2D) renderer.material.mainTexture;
		else
		{
			if( changeTextures )
				renderer.material.mainTexture = inactiveTexture;
		}
		
		if( originalScale == Vector3.zero )
			originalScale = transform.localScale;
	}
	
	public bool hover = false;
	
	public void OnEnable()
	{
		if( originalScale != Vector3.zero )
			transform.localScale = originalScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		Transform hit = LugusInput.use.RayCastFromMouse();
		if( hit == this.transform )
		{
			if( !hover )
			{
				hover = true;
				
				
				if( hoverSound != null )
					LugusAudio.use.SFX ().Play( hoverSound );
				
				// for some reason, sometimes the originalScale is not correct in Awake
				// so make sure we have the wanted scale before scaling back to zero (and hding the element!)
				if( originalScale == Vector3.zero )
					originalScale = transform.localScale;
				
				if( scale )
					gameObject.ScaleTo(originalScale * scaleFactor).Time(0.1f).Execute();
				
				if(changeTextures && activeTexture != null )
					renderer.material.mainTexture = activeTexture;
			}
		}
		else
		{
			if( hover )
			{
				hover = false;
				if( scale )
					gameObject.ScaleTo(originalScale).Time(0.1f).Execute();
				
				if( changeTextures )
					renderer.material.mainTexture = inactiveTexture;
			}
		}
		
		
		if( LugusInput.use.RayCastFromMouseUp() == this.transform )
		{
		
			if( pressSound != null )
				LugusAudio.use.SFX ().Play( pressSound );
			
			StartCoroutine( PressRoutine() );	
		}
	}
	
	// postpone the pressed-event by 1 frame
	// this makes sure the button is not triggered on the same frame as the actual MouseUp event
	protected IEnumerator PressRoutine()
	{
		yield return null;
		
		
		pressed = true;
	}
}
