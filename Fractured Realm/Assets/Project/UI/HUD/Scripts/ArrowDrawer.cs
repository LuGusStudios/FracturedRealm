using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ArrowDrawer : MonoBehaviour 
{
	protected bool debug = true;
	protected float arrowWidth = 3.0f;
	
	public Material positiveMaterial = null;
	public Material negativeMaterial = null;
	
	public Transform startTransform = null;
	public Transform targetTransform = null;

	// needed to make arrows stay on target even when moving the game cameras while the UI camera stays in place (ex. CameraDragger)
	public bool underlyingPositionSet = false;
	public Camera usedWorldCamera = null;
	protected Vector3 _underlyingWorldPosition; // start and targetTransform are in "UI" space. This is in game-world space for mapping from game-object to UI. Needed in CameraDragger for ex.
	public Vector3 underlyingWorldPosition
	{
		get{ return _underlyingWorldPosition; }
		set
		{
			underlyingPositionSet = true;
			_underlyingWorldPosition = value;
		}
	}

	void Awake()
	{
		if( startTransform == null )
		{
			startTransform = transform.FindChild("Start");
		}

		if( targetTransform == null )
		{
			targetTransform = transform.FindChild("Target");
		}
	}

	// Use this for initialization
	void Start () 
	{
		MakePositive();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	public void MakePositive()
	{
		renderer.material = positiveMaterial;
	}
	
	public void MakeNegative()
	{
		renderer.material = negativeMaterial;
	}
	
	public void Clear()
	{
		GetComponent<MeshFilter>().mesh.Clear();
	}

	public void CreateArrow(bool positive = true)
	{
		CreateArrow( startTransform.position, targetTransform.position, positive );
	}

	public void CreateArrowFromGameWorldPosition(bool positive = true)
	{
		if( !underlyingPositionSet )
			return;

		Vector3 screenPos = usedWorldCamera.WorldToScreenPoint( _underlyingWorldPosition );

		//Debug.Log ("ArrowFromGameWorldPos : " + screenPos + " // " + underlyingWorldPosition );

		Vector3 uiWorldPos = LugusInput.use.ScreenTo3DPoint( screenPos.z(0.0f), targetTransform.position, LugusCamera.ui );

		this.targetTransform.position = uiWorldPos;//new Vector3( screenPos.x / 100.0f, screenPos.y / 100.0f, this.targetTransform.position.z );

		CreateArrow( positive );
	}

	public void MoveTowards(Vector3 target, float time = 1.0f )
	{
		this.gameObject.StartLugusRoutine( MoveTowardsRoutine(target, time) );
	}

	public IEnumerator MoveTowardsRoutine(Vector3 target, float time = 1.0f)
	{
		targetTransform.gameObject.MoveTo( target ).Time ( time ).Execute();

		float startTime = Time.time;
		while( Time.time - startTime < time )
		{
			CreateArrow(true);
			yield return null;
		}

		// small times (0.3f or less) would otherwhise sometimes stop short of the target
		targetTransform.gameObject.StopTweens();
		targetTransform.position = target;
		CreateArrow(true);

		//Debug.Log ("END of MOVE TOWARDS routine");
	}
	
	public void CollapseTowards(Vector3 target, float time = 1.0f )
	{
		this.gameObject.StartLugusRoutine( CollapseTowardsRoutine(target, time) );
	}
	
	public IEnumerator CollapseTowardsRoutine(Vector3 target, float time = 1.0f)
	{
		startTransform.gameObject.MoveTo( target ).Time ( time ).Execute();
		
		float startTime = Time.time;
		while( Time.time - startTime < time )
		{
			CreateArrow(true);
			yield return null;
		}
		
		// small times (0.3f or less) would otherwhise sometimes stop short of the target
		startTransform.gameObject.StopTweens();
		startTransform.position = target;
		CreateArrow(true);
	}
	
	public void CreateArrowScreen(Vector3 startScreen, Vector3 endScreen, bool positive)
	{
		if( positive )
			renderer.material = positiveMaterial;
		else
			renderer.material = negativeMaterial;
			
			
		// start and end are in SCREEN space!
		// first convert them to world space
		Vector3 start = LugusInput.use.ScreenTo3DPointOnPlane( startScreen, new Plane(-Vector3.forward, transform.position) );
		Vector3 end = LugusInput.use.ScreenTo3DPointOnPlane( endScreen, new Plane(-Vector3.forward, transform.position) );
				
		if( Vector3.Distance(start, end) < 1.0f )
			return;

		CreateArrow( start, end, positive );
	}

	public void CreateArrow( Vector3 start, Vector3 end, bool positive ) // start and end in WORLD coordinates
	{
		// NOTE: The debug.Draw functions will appear offset by the parent transform. This is normal!
		start = transform.InverseTransformPoint(start);
		end = transform.InverseTransformPoint(end);
		
		// the arrow consists of 3 separate rectangles
		// the "bottom" one contains the arrow's "feathers"
		// the "top" one contains the arrow point
		// the "center" one contains the arrow shaft
		// this center one is also the one with variable length : the other two are fixed in length
		
		// so we need 8 different points:
		// bottomLeft, bottomRight
		// bottomLeftShaft, bottomRightShaft
		// topLeftShaft, topRightShaft
		// topLeft, topRight
		
		
		
		// step 1: calculate the 4 outermost corners
		// start and end are at the center of the outer lines of our rectangle
		// so we need to find lines perpendicular to the line between start and end
		
		Vector3 normal = -Vector3.forward; // facing towards camera, perpendicular to XY plane (which we're drawing on)
		// the side is then perpendicular to this normal
		Vector3 side = Vector3.Cross(normal, end - start);
		side.Normalize();
		
		if( debug )
		{
			Debug.DrawLine(start, end, Color.blue);
			Debug.DrawRay(start, normal, Color.red);
			Debug.DrawRay(start, side, Color.green);
		}
		
		// now we can actually create the 4 outermost corners
		// side is a DIRECTION vector, which basically means it is based on the Origin, rather than already on another point
		// to base it around start and end, we just need to add them to it
		// also: side is normalized, which means it has a length of 1 : multiply it with the correct length
		Vector3 bottomRight = start + side * (arrowWidth / 2);
		Vector3 bottomLeft 	= start + side * (arrowWidth / 2) * -1;
		Vector3 topRight 	= end   + side * (arrowWidth / 2);
		Vector3 topLeft 	= end   + side * (arrowWidth / 2) * -1;
		
		if( debug )
		{
			Debug.DrawLine(start, 	bottomLeft, Color.white);
			Debug.DrawLine(start, 	bottomRight, Color.white);
			Debug.DrawLine(end, 	topLeft, Color.white);
			Debug.DrawLine(end, 	topRight, Color.white);
		}
		
		float bottomHeight = 0.5f;
		float topHeight = 1.0f;//0.5f;

		topHeight = Mathf.Min( Vector3.Distance(start, end), topHeight );
		
		// step 2: calculate the 4 innermost corners
		// 1st we need to find position along the line between start and end at appropriate positions
		Vector3 lineDirection = (end-start).normalized;
		Vector3 startShaft = start + (lineDirection * bottomHeight);
		Vector3 endShaft = end - (lineDirection * topHeight);
		
		
		if( debug )
		{
		
			Debug.DrawLine(bottomLeft, 	startShaft, Color.white);
			Debug.DrawLine(bottomRight, startShaft, Color.white);
			Debug.DrawLine(topLeft, 	endShaft, Color.white);
			Debug.DrawLine(topRight, 	endShaft, Color.white);
		}
		
		Vector3 bottomRightShaft = startShaft + side * (arrowWidth / 2);
		Vector3 bottomLeftShaft  = startShaft + side * (arrowWidth / 2) * -1;
		Vector3 topRightShaft 	 = endShaft   + side * (arrowWidth / 2);
		Vector3 topLeftShaft 	 = endShaft   + side * (arrowWidth / 2) * -1;
		
		if( debug )
		{
		
			Debug.DrawLine(startShaft, 	bottomRightShaft, Color.white);
			Debug.DrawLine(startShaft,  bottomLeftShaft, Color.white);
			Debug.DrawLine(endShaft, 	topRightShaft, Color.white);
			Debug.DrawLine(endShaft, 	topLeftShaft, Color.white);
		}
		
		
		
		
		
		
		Vector3[] normals = new Vector3[8];
		normals[0] = -Vector3.forward;
		normals[1] = -Vector3.forward;
		normals[2] = -Vector3.forward;
		normals[3] = -Vector3.forward;
		normals[4] = -Vector3.forward;
		normals[5] = -Vector3.forward;
		normals[6] = -Vector3.forward;
		normals[7] = -Vector3.forward;
		
		Vector3[] vertices = new Vector3[8];
		
		vertices[0] = bottomLeft;
		vertices[1] = bottomRight;
 		vertices[2] = bottomLeftShaft;
		vertices[3] = bottomRightShaft;
		vertices[4] = topLeftShaft;
		vertices[5] = topRightShaft;
		vertices[6] = topLeft;
		vertices[7] = topRight;
		
		Vector2[] uvs = new Vector2[vertices.Length];
		uvs[0] = new Vector2(0,0);
		uvs[1] = new Vector2(1,0);
		uvs[2] = new Vector2(0,0.333f);
		uvs[3] = new Vector2(1,0.333f);
		uvs[4] = new Vector2(0,0.666f);
		uvs[5] = new Vector2(1,0.666f);
		uvs[6] = new Vector2(0,1);
		uvs[7] = new Vector2(1,1);
		
		int[] triangles = new int[18]; // 6 triangles, 3 points each
		// this is a bit tricky... we need to manually assign the indices in the vertices array
		// to tell unity which vertices to connect to triangles
		// start with the bottom "left bottom triangle", which runs from bottomRight, to bottomLeft, to bottomLeftShaft
		triangles[0] = 1;
		triangles[1] = 0;
		triangles[2] = 2;
		// then the bottom "right top triangle", which runs from bottomLeftShaft, to bottomRightShaft, to bottomRight
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 1;
		// then the shaft "left bottom triangle", which runs from bottomRightShaft, to bottomLeftShaft, to topLeftShaft
		triangles[6] = 3;
		triangles[7] = 2;
		triangles[8] = 4;
		// then the bottom "right top triangle", which runs from topLeftShaft, to topRightShaft, to bottomRightShaft
		triangles[9]  = 4;
		triangles[10] = 5;
		triangles[11] = 3;
		// then the top "left bottom triangle", which runs from topRightShaft, to topLeftShaft, to topLeft
		triangles[12] = 5;
		triangles[13] = 4;
		triangles[14] = 6;
		// then the top "right top triangle", which runs from topLeft, to topRight, to topRightShaft
		triangles[15] = 6;
		triangles[16] = 7;
		triangles[17] = 5;
		
		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.normals = normals;
		
		//mesh.RecalculateNormals();
		
		//GetComponent<MeshFilter>().mesh = mesh;
	}
	
	//http://games.deozaan.com/unity/MeshTutorial.pdf
	/*
	public void CreateRectangle(Vector3 start, Vector3 end)
	{
		// NOTE: The debug.Draw functions will appear offset by the parent transform. This is normal!
		start = transform.InverseTransformPoint(start);
		end = transform.InverseTransformPoint(end);
		
		// step 1: calculate the 4 outer corners
		// start and end are at the center of the outer lines of our rectangle
		// so we need to find lines perpendicular to the line between start and end
		
		// for this: we calculate the normal on our line first
		// then we find the line perpendicular to this normal, which will be what we are looking for
		// the normal is always in reference to a plane.
		// in our case, we want to draw in the XY plane, so the normal should be in the YZ plane
		// so we use Vector3.right (which is in the XY plane) to generate the normal
		//Vector3 normal = Vector3.Cross(end - start, Vector3.right );
		Vector3 normal = -Vector3.forward;
		// the side is then perpendicular to this normal
		Vector3 side = Vector3.Cross(normal, end - start);
		side.Normalize();
		
		if( debug )
		{
			Debug.DrawLine(start, end, Color.blue);
			Debug.DrawRay(start, normal, Color.red);
			Debug.DrawRay(start, side, Color.green);
		}
		
		// now we can actually create the 4 corners
		// side is a DIRECTION vector, which basically means it is based on the Origin, rather than already on another point
		// to base it around start and end, we just need to add them to it
		// also: side is normalized, which means it has a length of 1 : multiply it with the correct length
		Vector3 bottomRight = start + side * (arrowWidth / 2);
		Vector3 bottomLeft 	= start + side * (arrowWidth / 2) * -1;
		Vector3 topRight 	= end   + side * (arrowWidth / 2);
		Vector3 topLeft 	= end   + side * (arrowWidth / 2) * -1;
		
		if( debug )
		{
			Debug.DrawLine(start, 	bottomLeft, Color.white);
			Debug.DrawLine(start, 	bottomRight, Color.white);
			Debug.DrawLine(end, 	topLeft, Color.white);
			Debug.DrawLine(end, 	topRight, Color.white);
		}
		
		Vector3[] vertices = new Vector3[4];
		
		vertices[0] = bottomLeft;
		vertices[1] = topLeft;
		vertices[2] = topRight;
		vertices[3] = bottomRight;
		
		
		Vector3[] normals = new Vector3[4];
		normals[0] = -Vector3.forward;
		normals[1] = -Vector3.forward;
		normals[2] = -Vector3.forward;
		normals[3] = -Vector3.forward;
		
		Vector2[] uvs = new Vector2[4];
		uvs[0] = new Vector2(0,0);
		uvs[1] = new Vector2(1,0);
		uvs[2] = new Vector2(0,1);
		uvs[3] = new Vector2(1,1);
		
		int[] triangles = new int[6];
		// this is a bit tricky... we need to manually assign the indices in the vertices array
		// to tell unity which vertices to connect to triangles
		// start with the "left bottom triangle", which runs from bottomRight, to bottomLeft, to topLeft
		triangles[0] = 3;
		triangles[1] = 0;
		triangles[2] = 1;
		// then the "right top triangle", which runs from topLeft, to topRight, to bottomRight
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.normals = normals;
		
		//mesh.RecalculateNormals();
		
		//GetComponent<MeshFilter>().mesh = mesh;
	}
	*/
	/*
	public void Draw(Vector3 start, Vector3 end)
	{
		float lineWidth = 2.0f;
		Mesh mesh = new Mesh();
		
		Vector3 normal = Vector3.Cross(start, end);
		Vector3 side = Vector3.Cross(normal, end-start);
		side.Normalize();
		Vector3 a = start + side * (lineWidth / 2);
		Vector3 b = start + side * (lineWidth / -2);
		Vector3 c = end + side * (lineWidth / 2);
		Vector3 d = end + side * (lineWidth / -2);
		
		
		
		Graphics.DrawMeshNow(mesh, new Vector3(0,0,0), Quaternion.identity);
	}
	*/
}
