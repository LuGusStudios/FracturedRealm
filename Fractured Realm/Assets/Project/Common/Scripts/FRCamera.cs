using UnityEngine;
using System.Collections;

public class FRCamera : LugusSingletonExisting<FRCamera> 
{
	public FR.Target mode = FR.Target.BOTH;

	public void SetMode(FR.Target newMode )
	{
		//Debug.LogError("FRCamera setting mode " + newMode + " from " + this.mode + ". " + LugusCamera.numerator );

		if( newMode == mode )
			return;

		mode = newMode;

		if( mode == FR.Target.BOTH )
		{
			LugusCamera.numerator.gameObject.SetActive(true);
			LugusCamera.numerator.fieldOfView = 30;
			LugusCamera.numerator.rect = new Rect(0, 0.5f, 1, 1);
			LugusCamera.numerator.transform.position = new Vector3(0,0, -10.0f);

			
			LugusCamera.denominator.gameObject.SetActive(true);
			LugusCamera.denominator.fieldOfView = 30;
			LugusCamera.denominator.rect = new Rect(0, 0, 1, 0.5f);
			LugusCamera.denominator.transform.position = new Vector3(0,-200, -10.0f);

            SetCharactersPosition(4f);
		}

		else if( mode == FR.Target.NUMERATOR)
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

            SetCharactersPosition(2f);
		}

		else if( mode == FR.Target.DENOMINATOR )
		{
			LugusCamera.denominator.gameObject.SetActive(true);
			
			LugusCamera.denominator.rect = new Rect(0, 0, 1, 1);
			LugusCamera.denominator.fieldOfView = 57;


			LugusCamera.numerator.gameObject.SetActive(false);

            SetCharactersPosition(2f);
		}
	}

    void SetCharactersPosition(float zPosition)
    {
        Character[] characters = GameObject.FindObjectsOfType<Character>();

        if (characters.Length < 1)
        {
            Debug.LogWarning("FRCamera: Couldn't find any characters in the scene");
            return;
        }

        foreach (Character character in characters)
        {
            character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zPosition);
        }
    }

}
