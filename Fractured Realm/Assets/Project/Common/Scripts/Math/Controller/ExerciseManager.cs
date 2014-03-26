using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExerciseManager : MonoBehaviour 
{
	public List<Exercise> exercises = new List<Exercise>();
	
	public void Awake()
	{
		Exercise e = new Exercise();
		e.Add ( new Fraction(1,4) );
		e.Add ( new Fraction(2,4) );
		exercises.Add( e );
		
		
		e = new Exercise();
		e.Add ( new Fraction(1,6) );
		e.Add ( new Fraction(6,1) );
		exercises.Add( e );
		
		
		e = new Exercise();
		e.Add ( new Fraction(2,4) );
		e.Add ( new Fraction(1,6) );
		exercises.Add( e );
		
		currentExerciseIndex = -1;
	}
	
	public int currentExerciseIndex = -1;
	public List<Fraction> currentFractions = new List<Fraction>();
	
	// Use this for initialization
	void Start () 
	{
		LoadNextExercise();
	}
	
	protected void LoadNextExercise()
	{
		ClearCurrentFractions();
			
		currentExerciseIndex++;
		if( currentExerciseIndex >=  exercises.Count )
			currentExerciseIndex = 0;
		
		LoadExercise( exercises[currentExerciseIndex] );
	}
	
	protected void LoadExercise(Exercise e)
	{
		Fraction firstData = e.fractions[0];
		Fraction secondData = e.fractions[1];
		
		
		Fraction left = FractionFactory.use.CreateFraction(firstData.Numerator.Value, firstData.Denominator.Value);
		
		//GraphicsManager.PositionElement( left.Renderer.transform, 0 );
		
		Fraction right = FractionFactory.use.CreateFraction(secondData.Numerator.Value, secondData.Denominator.Value);
		
		//GraphicsManager.PositionElement( right.Renderer.transform, 1 );
		
		currentFractions.Add (left);
		currentFractions.Add (right);
	}
	
	protected void ClearCurrentFractions()
	{
		foreach( Fraction fraction in currentFractions )
		{
			//GameObject.DestroyImmediate( fraction.Renderer.gameObject );
		}
		
		currentFractions.Clear();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( Input.GetKeyDown(KeyCode.N) )
		{
			LoadNextExercise();
		}
	}
}
