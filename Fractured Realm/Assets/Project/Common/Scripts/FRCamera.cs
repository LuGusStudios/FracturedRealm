using UnityEngine;
using System.Collections;

public class FRCamera : MonoBehaviour 
{
	public enum Mode
	{
		NONE = -1,
		Numerator = 1,
		Denominator = 2,
		Both = 3
	}

	public Mode mode = Mode.Both;

	public void SetMode(Mode newMode )
	{
		//Debug.LogError("FRCamera setting mode " + newMode + " from " + this.mode + ". " + LugusCamera.numerator );

		if( newMode == mode )
			return;

		mode = newMode;

		if( mode == Mode.Both )
		{
			LugusCamera.numerator.gameObject.SetActive(true);
			LugusCamera.numerator.fieldOfView = 30;
			LugusCamera.numerator.rect = new Rect(0, 0.5f, 1, 1);
			LugusCamera.numerator.transform.position = new Vector3(0,0, -10.0f);

			
			LugusCamera.denominator.gameObject.SetActive(true);
			LugusCamera.denominator.fieldOfView = 30;
			LugusCamera.denominator.rect = new Rect(0, 0, 1, 0.5f);
			LugusCamera.denominator.transform.position = new Vector3(0,-200, -10.0f);
		}
		else if( mode == Mode.Numerator )
		{
			LugusCamera.numerator.gameObject.SetActive(true);

			LugusCamera.numerator.rect = new Rect(0, 0, 1, 1);
			
			// looks fine, but if you look straight, it will actually drop some details that were visible on the sides 
			// so the visible are actually becomes a little smaller this way
			//LugusCamera.numerator.transform.position = new Vector3(0,0, -20.0f); 

			// gives almost the exact same view
			// only thing that probably needs to be done here is y-scaling of the background gradient
			// since that is a big difference depening on the FOV
			// note: i think for the desert example it actually looks nicer in Both if the background is cropped
			LugusCamera.numerator.fieldOfView = 57;


			LugusCamera.denominator.gameObject.SetActive(false);
		}
		else if( mode == Mode.Denominator )
		{
			LugusCamera.denominator.gameObject.SetActive(true);
			
			LugusCamera.denominator.rect = new Rect(0, 0, 1, 1);
			LugusCamera.denominator.fieldOfView = 57;


			LugusCamera.numerator.gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
