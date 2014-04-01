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
				f.path = path;
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

		
		EditorGUILayout.LabelField("Build/Destroy trees");
		EditorGUILayout.BeginHorizontal();

		
		if( GUILayout.Button ("\n\nFill\n\n") )
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
		
		if( GUILayout.Button("\n\nClear\n\n") )
		{
			ac.Clear();
		}

		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Tree layouts");
		
		
		EditorGUILayout.BeginHorizontal();
		if( GUILayout.Button("\n\nCircle\n\n") )
		{
			ac.RepositionRadially();
		}
		
		if( GUILayout.Button("\n\nVertical line\n\n") )
		{
			ac.RepositionVertically();
		}
		
		EditorGUILayout.EndHorizontal();

		
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




		//ac.

		//ac.SetEventDefaultBool(ac.FindEvent("TestAutomatic"), true);
		/*
		// States on layer 0:
		UnityEditorInternal.StateMachine sm = ac.GetLayerStateMachine(0);
		List<UnityEditorInternal.State> states = sm.statesRecursive; // Also: sm.states
		foreach (UnityEditorInternal.State s in states) 
		{
			GUILayout.Label( string.Format("State: {0}", s.GetUniqueName()));
			
			Transition[] tss = sm.GetTransitionsFromState(s);
			string tsss = "";
			foreach( Transition t in tss ) 
			{
				int conditions = t.GetConditionCount();
				tsss += "#conditions : " + conditions + "";
				
				//UnityEditorInternal.AnimatorControllerEventType
				
				for( int c = 0; c < conditions; ++c )
				{
					bool b = t.GetEventTreshold(c);
					tsss += " // " + t.GetEventTreshold(c) + "/" + t.GetExitTime(c) + "/" + "" +"/" + t.GetConditionEvent(c) + " , \n";
				}
			}
			
			GUILayout.Label( " -> " + tsss );
		}
		
		
		motion = (Motion) EditorGUILayout.ObjectField( motion, typeof(Motion), true );
		
		if( GUILayout.Button("TestAddState") )
		{
			
			State oldState = sm.FindState("Base Layer.RobinTest");
			if( oldState != null )
			{
				// "works", but gives runtime errors which make the entire graph disappear in the editor and fucks up Unity
				//sm.RemoveState(oldState);
			}
			
			
			State newState = sm.AddState("RobinTest"); 
			newState.SetMotion(0, motion);
			
			
			
			State idleState = sm.FindState("Base Layer.Idle");
			Transition ts = sm.AddTransition(newState, idleState);
			
			ts.SetConditionMode(0, TransitionConditionMode.ExitTime);
			ts.SetExitTime(0, motion.averageDuration / 2.0f );
			
			
			
			ts = sm.AddTransition(idleState, newState);
			
			ts.SetConditionMode(0, TransitionConditionMode.Equals);
			ts.SetConditionEvent(0, "TestAutomatic"); 
			
			//ts.SetTransitionDuration( Mathf.Min(1.0f, newState.GetMotion(0).averageDuration) );
			//ts.SetTransitionOffset(0.0f);
		}
		*/
		
		
		/*
		// Number of layers:
		int layerCount = ac.GetLayerCount();
		Debug.Log(string.Format("Layer Count: {0}", layerCount));
		 
		// Names of each layer:
		for (int layer = 0; layer < layerCount; layer++) {
		    Debug.Log(string.Format("Layer {0}: {1}", layer, ac.GetLayerName(layer)));
		}
		*/
		
		/*
		if( GUILayout.Button("Verwijder onnodige SceneArrows") )
		{
			PrototypeIconManager.ValidateSceneArrowsOnIcons();
		}
		
		GUILayout.Label("Verwijder alle SceneArrow components op icons die niet echt nuttig linken\n(basically alles buiten de PUZZEL-knoppen)");
		
		
		if( GUILayout.Button("SoundOnClick op SOUND en VERTELLER") )
		{
			PrototypeIconManager.ValidateSoundOnClickOnIcons();
		}
		
		GUILayout.Label("Voeg SoundOnClick toe op alle SOUND en VERTELLER iconen waar het nog niet op staat.");
		GUILayout.Label("Print ook welke SoundOnClick nog geen AudioClips hebben.");
		
		if( GUILayout.Button("Toggle icon visibility") )
		{
			PrototypeIconManager.ToggleAllIcons();
		}
		
		GUILayout.Label("Zet alle iconen aan/uit");
		
		GUILayout.Label(" \n \n ");
		
		GUILayout.Label("In game:");
		GUILayout.Label("F1: toggle VERTELLER/SOUND");
		GUILayout.Label("F2: toggle INTERACTION/GATE/PUZZEL");
		GUILayout.Label("F3: toggle KNUFFEL");
		*/
		
		/*	
		enabled = EditorGUILayout.Toggle("Enabled", enabled);
		
		xIncrement = EditorGUILayout.FloatField("x difference", xIncrement);
		yIncrement = EditorGUILayout.FloatField("y difference", yIncrement);
		*/	
	}
	
	
	/*
	protected GameObject _prefab = null;
	protected bool _copyMaterials = false;
	List<GameObject> _originalObjects = new List<GameObject>();
	List<GameObject> _newPrefabs = new List<GameObject>();
	
	string cameraCode = "";
	
    void OnGUI () 
	{
        //GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        //    myString = EditorGUILayout.TextField ("Text Field", myString);
		
		//Vector3 scaleFactor = new Vector3(2,2,2);
		_prefab = (GameObject) EditorGUILayout.ObjectField("Prefab: ", _prefab, typeof(GameObject), false);
		//string 
		
		
		
		if( GUILayout.Button("Replace selection") )
		{
			if( Selection.transforms.Length > 0 )
			{
				_newPrefabs = new List<GameObject>();
				_originalObjects = new List<GameObject>();
				ReplaceWithPrefab();
			}
			else 
				Debug.LogError("There is no selection to replace");
		}
		
		if( _originalObjects.Count > 0 && GUILayout.Button("Undo") )
		{
			UndoReplacements();
		}
		
		if( _originalObjects.Count > 0 && GUILayout.Button("Apply Final") )
		{
			ApplyFinal();
		}
		
		// just for debugging purposes
		bool foldout = true;
		ArrayFoldout( "originals", _originalObjects.ToArray(), ref foldout );
		ArrayFoldout( "newPrefabs", _newPrefabs.ToArray(), ref foldout );
		
		GUILayout.Label("CAMERA FUNCTIONALITY: (@TODO MOVE THIS!)");
		
		if( GUILayout.Button("Show Cam Parameters") )
		{	
			Vector3 pos = SceneView.lastActiveSceneView.camera.transform.position;
			Vector3 rot = SceneView.lastActiveSceneView.camera.transform.eulerAngles;
			cameraCode = ".position = new Vector3("+ pos.x +"f,"+ pos.y +"f,"+ pos.z +"f);\n";
			cameraCode += ".eulerAngles = new Vector3("+ rot.x +"f,"+ rot.y +"f,"+ rot.z +"f);\n";
			
			Camera.main.transform.position = pos;
			Camera.main.transform.eulerAngles = rot;
		}
		
		EditorGUILayout.TextArea(cameraCode);
    }
	
	// adapted from: http://www.unifycommunity.com/wiki/index.php?title=EditorGUIExtension
	public static string[] ArrayFoldout(string label, GameObject[] array, ref bool foldout)
    {
        EditorGUILayout.BeginVertical();
        EditorGUIUtility.LookLikeInspector();
        foldout = EditorGUILayout.Foldout(foldout, label);
        string[] newArray = new string[array.Length];
        if (foldout)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            int arraySize = EditorGUILayout.IntField("Size", array.Length);
            if (arraySize != array.Length)
                newArray = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                string entry = "";
                if (i < array.Length)
                    entry = array[i].name;
                newArray[i] = EditorGUILayout.TextField("Element " + i, entry);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        return newArray;
    }
   
	
	void ReplaceWithPrefab()
	{	
		foreach (Transform transform in Selection.transforms) 
		{
			//Undo.RegisterUndo(transform, transform.name + " is replaced with prefab " + _prefab.name );
			
            GameObject oldObj = transform.gameObject;
			GameObject newObj = null;
			
            newObj = (GameObject) EditorUtility.InstantiatePrefab(_prefab);
            newObj.transform.parent = oldObj.transform.parent;
			
			ChangePropertiesRecursively( oldObj, newObj );
			
			_originalObjects.Add(oldObj);
			_newPrefabs.Add(newObj);
			
			oldObj.SetActiveRecursively(false);
			
           	//DestroyImmediate(oldObj);
        }
	}
	
	void ChangePropertiesRecursively(GameObject oldObj, GameObject newObj)
	{
        newObj.transform.position = oldObj.transform.position;
        newObj.transform.rotation = oldObj.transform.rotation;
		newObj.transform.localScale = oldObj.transform.localScale;
		
		if( oldObj.renderer && newObj.renderer && _copyMaterials )
		{
			newObj.renderer.sharedMaterials = oldObj.renderer.sharedMaterials;
			Debug.Log("Changed materials : " + oldObj.name + " to " + newObj.name );
		}
		
		foreach( Transform nextOldt in oldObj.transform )
		{
			GameObject nextOld = nextOldt.gameObject;
			Transform nextNewt = newObj.transform.FindChild( nextOldt.name );
			if( nextNewt != null )
				ChangePropertiesRecursively( nextOld, nextNewt.gameObject );
		}
	}
	
	void UndoReplacements()
	{
		
		foreach( GameObject pref in _newPrefabs )
			DestroyImmediate(pref);
		
		_newPrefabs = new List<GameObject>();
		
		foreach( GameObject old in _originalObjects )
			old.SetActiveRecursively(true);
	}
	
	void ApplyFinal()
	{
		foreach( GameObject old in _originalObjects )
			DestroyImmediate(old);
		
		_originalObjects = new List<GameObject>();
		_newPrefabs = new List<GameObject>();
	}
	*/
}







