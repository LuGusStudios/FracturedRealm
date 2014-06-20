// C# example:
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

//http://answers.unity3d.com/questions/418854/getting-a-list-of-mecanim-states-in-animator.html?sort=oldest
using System.IO;


public class StateMachineManipulatorWindow : EditorWindow 
{
	[MenuItem ("FR/StateMachineManipulatorWindow")]
	static void Init () 
	{
		StateMachineManipulatorWindow window = (StateMachineManipulatorWindow)EditorWindow.GetWindow (typeof (StateMachineManipulatorWindow));
	}
	
	/*
	void OnEnable()
	{
		SceneView.onSceneGUIDelegate += OnSceneUpdate;
	}
	
	void OnSceneUpdate(SceneView sceneView)
	{
		if( !enabled )
			return;
		
		Event e = Event.current;
		if( !(e.type == EventType.keyDown) )
			return;
		
		float xDiff = 0.0f;
		float yDiff = 0.0f;
		
		if( e.keyCode == KeyCode.LeftArrow )
		{
			xDiff = -xIncrement;
			//Debug.Log("Pressed left arrow, yay!");
		}
		
		if( e.keyCode == KeyCode.RightArrow )
		{
			xDiff = xIncrement;
		}
		
		if( e.keyCode == KeyCode.UpArrow )
			yDiff = yIncrement;
		
		if( e.keyCode == KeyCode.DownArrow )
			yDiff = -yIncrement;
		
		foreach( Transform t in Selection.transforms )
		{
			t.position = new Vector3(t.position.x + xDiff, t.position.y + yDiff, t.position.z);
		}
		
		Undo.RegisterUndo( Selection.transforms, "Undo move with arrows");
		
		e.Use();
		
		//Debug.Log("GOT EVENT FROM UNITY SCENEVIEW!");
	}
	*/
	
	public Animator animator = null;
	public string animationFolder = "Project/CharactersNew/Animations";


	public class FRAnimationFile
	{
		public Object asset = null;

		public string path = "";
	}


	// ex. call GetAssetsInFolderRecursive("Project/CharactersNew/Animations", "", output);
	protected void GetAssetsInFolderRecursive(string folderName, string path,  List<FRAnimationFile> output)
	{
		//Debug.LogError("GetAssetsInFolderRecursive : " + folderName + " @path " + path + ", #objects = " + output.Count);

		//Object[] assets = AssetDatabase.LoadAllAssetsAtPath( "Assets/" + folderName);
		
		string directoryPath = Application.dataPath + "/" + folderName;

		FileInfo[] assetFiles = new DirectoryInfo( directoryPath ).GetFiles();

		foreach( FileInfo assetFile in assetFiles )
		{
			if( assetFile.Name.Contains(".meta") )
				continue;

			/*
			Object[] asset = AssetDatabase.LoadAllAssetsAtPath( "Assets/" + folderName + "/" + assetFile.Name );
			
			foreach( Object obj in asset )
			{
				FRAnimationFile f = new FRAnimationFile();
				f.path = path;
				f.asset = obj;

				output.Add (f);
			}
			*/

			AnimationClip clip = (AnimationClip) AssetDatabase.LoadAssetAtPath( "Assets/" + folderName + "/" + assetFile.Name, typeof(AnimationClip) );

			if( clip != null )
			{
				FRAnimationFile f = new FRAnimationFile();
				// path is of the form /Type/Animation  -> we need Type.Animation
				f.path = path.Trim(new char[1]{'/'}).Replace("/", ".");
				f.asset = clip;
				
				output.Add (f);
			}
			else
			{
				Debug.LogWarning("Animation file at " + "Assets/" + folderName + "/" + assetFile.Name + " had no AnimationClip!");
			}

		}

		// LoadAllAssetsAtPath


		//Debug.LogError("DIR PATH "+ directoryPath);

		DirectoryInfo[] directories = new DirectoryInfo( directoryPath ).GetDirectories();
		foreach( DirectoryInfo directory in directories )
		{
			GetAssetsInFolderRecursive( folderName + "/" + directory.Name,  path + "/" + directory.Name, output );
			//Debug.LogError("DIRECTORY : " + directory.Name );
		}

	}

	protected void FillAnyState(UnityEditorInternal.AnimatorController ac)
	{
		
		List<FRAnimationFile> animations = new List<FRAnimationFile>();
		GetAssetsInFolderRecursive(animationFolder, "", animations);
		
		AnimatorControllerLayer baseLayer = ac.GetLayer(0);
		
		foreach( FRAnimationFile animationFile in animations )
		{
			//Debug.LogError("Animation : " + obj.asset.name + " -> " + obj.asset.GetType().Name);
			
			StateMachine subState = baseLayer.stateMachine.FindSubStateMachine( animationFile.path );
			if( subState == null )
			{
				subState = baseLayer.stateMachine.AddStateMachine( animationFile.path );
			}

			subState.anyStatePosition = new Vector3(0,0,0);

			State newState = subState.AddState( animationFile.asset.name );
			newState.SetAnimationClip( (AnimationClip) animationFile.asset );
			
			Transition t = subState.AddAnyStateTransition( newState );
			t.RemoveCondition(0); // default exit time 0.9f
			t.atomic = false; // can be interupted by other animations
			
			AnimatorCondition condition = t.AddCondition();
			condition.mode = TransitionConditionMode.If;
			condition.parameter = newState.name;
			
			
			ac.AddParameter( newState.name, AnimatorControllerParameterType.Trigger );
			
			
			if( animationFile.asset.name == "idle" )
			{
				Debug.LogWarning("Found idle state. Setting as default");
				baseLayer.stateMachine.defaultState = newState;
			}
		}
		
		ac.RepositionRadially();
	}

	protected void FillIdle(UnityEditorInternal.AnimatorController ac)
	{
		List<FRAnimationFile> animations = new List<FRAnimationFile>();
		GetAssetsInFolderRecursive(animationFolder, "", animations);
		
		AnimatorControllerLayer baseLayer = ac.GetLayer(0);

		baseLayer.stateMachine.anyStatePosition = new Vector3(-350, -350, 0);

		// First: search idle state
		State idleState = null;
		foreach( FRAnimationFile animationFile in animations )
		{
			if( animationFile.asset.name == "idle" )
			{
				idleState = baseLayer.stateMachine.AddState( animationFile.asset.name );
				idleState.SetAnimationClip( (AnimationClip) animationFile.asset );

				baseLayer.stateMachine.defaultState = idleState;
				break;
			}
		}

		if( idleState == null )
		{
			Debug.LogError("FillIdle : no idle state found! Make sure there is an animation called \"idle\" in " + animationFolder );
			return;
		}

		idleState.position = new Vector3(0,0,0);

		//Debug.LogError("IdleState position set to : " + idleState.position );

		
		foreach( FRAnimationFile animationFile in animations )
		{
			if( animationFile.asset.name == "idle" )
			{
				continue;
			}




			
			StateMachine subState = baseLayer.stateMachine.FindSubStateMachine( animationFile.path );
			if( subState == null )
			{
				subState = baseLayer.stateMachine.AddStateMachine( animationFile.path );
			}
			
			subState.anyStatePosition = new Vector3(-350, -350, 0);
			subState.parentStateMachinePosition = new Vector3(0,0,0);

			
			State newState = subState.AddState( animationFile.asset.name );
			newState.SetAnimationClip( (AnimationClip) animationFile.asset );
			
			Transition idleToState = subState.AddTransition( idleState, newState );
			idleToState.RemoveCondition(0); // default exit time 0.96f
			//idleToState.atomic = false; // can be interupted by other animations
			
			AnimatorCondition idleToStateCondition = idleToState.AddCondition();
			idleToStateCondition.mode = TransitionConditionMode.If;
			idleToStateCondition.parameter = newState.name;

			ac.AddParameter( newState.name, AnimatorControllerParameterType.Trigger );


			Transition stateToIdle = subState.AddTransition( newState, idleState );
			// transition here is default exitTime 0.5, which is a little too small
			stateToIdle.GetCondition(0).exitTime = 0.86f;


			// if it's a looping animation, just stay in the state and use CrossFade to go back to Idle
			if( (animationFile.asset as AnimationClip).wrapMode == WrapMode.Loop ||
			   (animationFile.asset as AnimationClip).isLooping )
			{
				Debug.Log ( "Found looping state " + animationFile.asset.name );
				stateToIdle.RemoveCondition(0);
			}
		}
		
		ac.RepositionRadially();
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField("Drag a scene object with an animator here to edit the AnimatorController");
		animator = (Animator) EditorGUILayout.ObjectField( animator, typeof(Animator), true );
		
		EditorGUILayout.LabelField("The folder where the MovieClips will be found recursively");
		animationFolder = (string) EditorGUILayout.TextField( animationFolder );
		
		if( animator == null )
			return;

		EditorGUILayout.Space();
		
		UnityEditorInternal.AnimatorController ac = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;

		EditorGUILayout.LabelField("Now editing AnimatorController : ");
		EditorGUILayout.ObjectField( ac, typeof(UnityEditorInternal.AnimatorController) );
		
		EditorGUILayout.LabelField("Build/Destroy trees");
		EditorGUILayout.BeginHorizontal();

		
		if( GUILayout.Button ("\n\nFill Any State\n\n") )
		{
			FillAnyState(ac);
		}
		
		if( GUILayout.Button ("\n\nFill Idle State\n\n") )
		{
			FillIdle(ac);
		}

		if( GUILayout.Button("\n\nClear\n\n") )
		{
			ac.Clear();
		}

		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Tree layouts");
		
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if( GUILayout.Button("\n\nCircle recursive\n\n") )
		{
			ac.RepositionRadially();
		}
		
		if( GUILayout.Button("\n\nVertical line recursive\n\n") )
		{
			ac.RepositionVertically();
		}
		EditorGUILayout.EndHorizontal();

		/*
		// this won't work because we can't query the API for which stateMachine is currently shown... weird
		EditorGUILayout.BeginHorizontal();
		
		if( GUILayout.Button("\n\nCircle this page\n\n") )
		{
			ac.RepositionRadially(false);
		}
		
		if( GUILayout.Button("\n\nVertical line this page\n\n") )
		{
			ac.RepositionVertically(false);
		}
		EditorGUILayout.EndHorizontal();
		*/
		EditorGUILayout.EndVertical();


		
		EditorGUILayout.BeginVertical();
		EditorGUILayout.BeginHorizontal();
		if( GUILayout.Button("\n\nBuild FRAnimations class code\n\n") )
		{
			AnimationTemplateGeneratorHelper.GenerateFRAnimationsClass( ac );
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if( GUILayout.Button("\n\nBuild OperationVisualizers\n\n") )
		{
			AnimationTemplateGeneratorHelper.GenerateOperationVisualizerClasses( ac );
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		if( GUILayout.Button("\n\nBuild HTML output\n\n") )
		{
			AnimationTemplateGeneratorHelper.GenerateHTMLAnimationBreakdown( ac );
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		
		EditorGUILayout.Space();
		if( GUILayout.Button("Print movieclip list") )
		{
			List<FRAnimationFile> output = new List<FRAnimationFile>();
			GetAssetsInFolderRecursive(animationFolder, "", output);
			
			foreach( FRAnimationFile obj in output )
			{
				Debug.LogError("Animation : " + obj.asset.name + " -> " + obj.asset.GetType().Name);
			}
		}


		// TODO: FIXME: merge 2 controllers (facial animations custom builden, terug invoegen bij deze later)


		int layerCount = ac.layerCount;
		for( int i = 0; i < layerCount; ++i )
		{

			UnityEditorInternal.AnimatorControllerLayer layer = ac.GetLayer(i);
			GUILayout.Label( "" + i + " : "  + layer.name );

			UnityEditorInternal.StateMachine sm = layer.stateMachine;

			//Transition ts = sm.AddTransition(null, null);


			int stateCount = sm.stateCount;
			for( int j = 0; j < stateCount; ++j )
			{
				UnityEditorInternal.State state = sm.GetState(j);

				GUILayout.Label( " -> " + j + " : " + state.name);
			}
			
			int subStateCount = sm.stateMachineCount;
			for( int k = 0; k < subStateCount; ++k )
			{
				UnityEditorInternal.StateMachine stateMachine = sm.GetStateMachine(k);
				
				GUILayout.Label( " -> " + k + " : " + stateMachine.name);
			}

			/*
			AnimatorCondition cond = ts.AddCondition(); 
			cond.mode = TransitionConditionMode.If;
			cond.
			*/
		}


	}

}







