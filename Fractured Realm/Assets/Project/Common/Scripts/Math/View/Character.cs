using UnityEngine;
using System.Collections;

// TODO: Should be refactored to CharacterRenderer?
public class Character : MonoBehaviour 
{
	//public Number _number = null;
	public Number Number
	{
		get
		{ 
			if( _numberRenderer != null )
				return _numberRenderer.Number;
			else
			{
				Debug.LogError("Character:Number : no numberRenderer known! No Number returned!");
				return null;
			}
		}
		
//		set
//		{
//			/*
//			if( _number != null )
//			{
//				_number.OnValueChanged -= OnNumberValueChanged;
//			}
//			*/
//			
//			_number = value; 
//			//_number.OnValueChanged += OnNumberValueChanged;
//			
//			//UpdateRenderer();
//		}
	}
	
	
	protected NumberRenderer _numberRenderer = null;
	public NumberRenderer NumberRenderer
	{
		get{ return _numberRenderer; }
		set{ _numberRenderer = value; }
	}

	/*
	protected CharacterAnimator _animator = null;
	public CharacterAnimator Animator
	{
		get{ return _animator; }
		set{ _animator = value; }
	}
	*/
	public CharacterAnimator Animator
	{
		get{ return GetComponent<CharacterAnimator>(); }
	}
	
	// the actual value this character is rendering
	// is not always equal to Number.Value!!! (from 7, a Number is rendererd using 2 characters)
	protected int _value = -1;
	public int Value
	{
		get{ return _value; }
		set{ _value = value; }
	}
	
	
	public void ShowBody(bool show)
	{
		if( transform.FindChild("body") != null )
			transform.FindChild("body").renderer.enabled = show;
		
		if( transform.FindChild("global_controller1") != null )
			transform.FindChild("global_controller1").gameObject.SetActive( show );
		else if( transform.FindChild("kilt2") )
			transform.FindChild("kilt2").renderer.enabled = show;
		else if( transform.FindChild("kilt") )
			transform.FindChild("kilt").renderer.enabled = show;
			
	}
	
	/*
	public void OnNumberValueChanged(int oldValue, int newValue)
	{
		UpdateRenderer();	
	}
	
	public void UpdateRenderer()
	{
		
	}
	*/
	
	/*
	public NumberAnimator Animator
	{
		get{ return this.gameObject.GetComponent<NumberAnimator>(); }
	}
	*/
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
