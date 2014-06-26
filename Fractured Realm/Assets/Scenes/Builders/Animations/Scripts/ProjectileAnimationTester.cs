using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileAnimationTester : MonoBehaviour 
{


	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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

	public void OnGUI()
	{
		GUILayout.BeginArea( new Rect(0,0, 200, 200) );

		bool all = false;
		
		if( GUILayout.Button("\nStart ALL\n") )
		{
			all = true;
		}

		if( all || GUILayout.Button("Start math") )
		{
			GameObject start = GameObject.Find ("Start");
			GameObject stop = GameObject.Find("Stop");
			GameObject projectile = GameObject.Find ("Projectile");
			projectile = (GameObject) GameObject.Instantiate( projectile );
			projectile.name = "" + Random.Range(0, 10000000);

			projectile.transform.parent = null;
			projectile.transform.position = start.transform.position;

			iTween.ProjectileTo(projectile, stop.transform.position, 1.0f);
		}
		
		if(all ||  GUILayout.Button("Start math complex") )
		{
			GameObject start = GameObject.Find ("Start");
			GameObject stop = GameObject.Find("Stop");
			GameObject projectile = GameObject.Find ("Projectile");
			projectile = (GameObject) GameObject.Instantiate( projectile );
			projectile.name = "" + Random.Range(0, 10000000);
			
			projectile.transform.parent = null;
			projectile.transform.position = start.transform.position;
			
			iTween.ProjectileTo(projectile, stop.transform.position, 2.0f, 1.0f);
		}

		if(all ||  GUILayout.Button("Start dirrrty") )
		{
			GameObject start = GameObject.Find ("Start");
			GameObject stop = GameObject.Find("Stop");
			GameObject projectile = GameObject.Find ("Projectile");
			projectile = (GameObject) GameObject.Instantiate( projectile );
			projectile.name = "" + Random.Range(0, 10000000);

			GameObject projectileParent = GameObject.Find("ProjectileParent");
			if( projectileParent == null )
				projectileParent = new GameObject("ProjectileParent"); 

			foreach( Transform child in projectileParent.transform )
			{
				child.parent = null;
			}

			projectile.transform.parent = projectileParent.transform;
			projectile.transform.localPosition = new Vector3(0,0,0);


			projectileParent.transform.position = start.transform.position;

			float lobHeight = 4.0f;
			float lobTime = 1.0f;

			iTween.MoveBy(projectile, iTween.Hash("y", lobHeight, "time", lobTime/2, "easeType", iTween.EaseType.easeOutQuad, "orienttopath", true));
			iTween.MoveBy(projectile, iTween.Hash("y", -lobHeight, "time", lobTime/2, "delay", lobTime/2, "easeType", iTween.EaseType.easeInCubic, "orienttopath", true));    
			iTween.MoveTo(projectileParent, iTween.Hash("position", stop.transform.position, "time", lobTime, "easeType", iTween.EaseType.linear));
		}

		GUILayout.EndArea();
	}
}
