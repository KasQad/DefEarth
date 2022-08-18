using System.Collections.Generic;
using Asteroids;
using Planets;
using ScriptableObject.Waves;
using UnityEngine;
using UnityEngine.UI;

public class WaveEnemyController : MonoBehaviour
{
	[SerializeField] private PlanetController planetController;
	[SerializeField] private AsteroidController asteroidController;
	[SerializeField] private WavesInfo wavesInfo;
	[SerializeField] private new Camera camera;
	
	[SerializeField] private Button buttonSmall;
	[SerializeField] private Button buttonMiddle;
	[SerializeField] private Button buttonBig;
	[SerializeField] private Button buttonNextWave;


	private Vector2 _pointSpawnEnemyEntity;
	[SerializeField] private float radiusSpawnEnemyEntity = 2f;

	[Range(0f, 2f)]
	[SerializeField] private float difficultySpawnEnemy = 0.5f;

	private float currenttWaveTime;
	private float waveTime = 15f;
	private int currentWaveNumber;
	private bool waveIsRun;
	
	public int asteroidTypeSmallCount;
	public float asteroidTypeSmallForwardSpeedMin;
	public float asteroidTypeSmallForwardSpeedMax;


	private void Awake()
	{
		buttonSmall.onClick.AddListener(() => CreateAsteroid(AsteroidType.Small));
		buttonMiddle.onClick.AddListener(() => CreateAsteroid(AsteroidType.Middle));
		buttonBig.onClick.AddListener(() => CreateAsteroid(AsteroidType.Big));
		buttonNextWave.onClick.AddListener(() => NextWave());

		_pointSpawnEnemyEntity = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
		_pointSpawnEnemyEntity = new Vector2(
			_pointSpawnEnemyEntity.x + radiusSpawnEnemyEntity,
			_pointSpawnEnemyEntity.y + radiusSpawnEnemyEntity - 4f);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z)) CreateAsteroid(AsteroidType.Small);
		if (Input.GetKeyDown(KeyCode.X)) CreateAsteroid(AsteroidType.Middle);
		if (Input.GetKeyDown(KeyCode.C)) CreateAsteroid(AsteroidType.Big);


		if (Time.time <= currenttWaveTime)
		{
			// CreateAsteroid(AsteroidType.Small);
			//currenttWaveTime -= waveTime;
		}
		else if (!GameConfig.GamePaused) GameConfig.GamePaused = true;
	}

	private void NextWave()
	{
		currentWaveNumber++;
		currenttWaveTime = Time.time + waveTime;
		GameConfig.GamePaused = false;
	}

	public float GetWaveLeftTine() => GameConfig.GamePaused ? 0 : currenttWaveTime - Time.time;
	public int GetWaveNumber() => currentWaveNumber;

	private void CreateAsteroid(AsteroidType asteroidType)
	{
		var pointStart = MathFunctions.GetPointInsideCircle(_pointSpawnEnemyEntity, radiusSpawnEnemyEntity);
		var planet = planetController.GetPlanetByType(PlanetType.Earth);
		var pointEnd = MathFunctions.GetPointInsideCircle(planet.GetPosition(),
			planet.radiusPlanet + planet.radiusPlanet * difficultySpawnEnemy);
		pointEnd = MathFunctions.LineExtension(pointStart, pointEnd, 10f);
		float minSpeed = 10 + (Time.time);
		float maxSpeed = 20 + (Time.time);
		float newSpeed = Random.Range(minSpeed, maxSpeed);
		float newSpeedRotate = newSpeed;
		var pathPointsList = new List<Vector2> { pointStart, pointEnd };
		asteroidController.CreateAsteroid(asteroidType, pathPointsList, true, newSpeed, newSpeedRotate);
	}
}
