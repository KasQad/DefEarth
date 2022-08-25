using System.Collections.Generic;
using Asteroids;
using Planets;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveEnemyController : MonoBehaviour
{
	[SerializeField] private PlanetController planetController;
	[SerializeField] private AsteroidController asteroidController;
	[SerializeField] private new Camera camera;
	[SerializeField] private LinesManager linesManager;

	private Vector2 _pointSpawnEnemyEntity;

	private const float WaveTime = 25f;
	private const int AsteroidCount = 20;
	private float _spawnEnemyCounter;

	private readonly Functions.Counter _counterSpawnEnemy = new Functions.Counter();
	private readonly Functions.Timer _timerWaveTimeCount = new Functions.Timer();

	private void Awake()
	{
		_pointSpawnEnemyEntity = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
		_pointSpawnEnemyEntity = new Vector2(
			_pointSpawnEnemyEntity.x + GameConfig.RadiusSpawnEnemyEntity + GameConfig.RadiusSpawnEnemyEntityShiftX,
			_pointSpawnEnemyEntity.y + GameConfig.RadiusSpawnEnemyEntity + GameConfig.RadiusSpawnEnemyEntityShiftY);

		linesManager.DrawCircle(transform, _pointSpawnEnemyEntity, GameConfig.RadiusSpawnEnemyEntity, 0.03f,
			Color.yellow);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z)) CreateAsteroid(AsteroidType.Small);
		if (Input.GetKeyDown(KeyCode.X)) CreateAsteroid(AsteroidType.Middle);
		if (Input.GetKeyDown(KeyCode.C)) CreateAsteroid(AsteroidType.Big);

		if (!_timerWaveTimeCount.CheckStop())
		{
			if (GameConfig.GamePaused) return;
			if (!(_counterSpawnEnemy.CheckTime() >= _spawnEnemyCounter)) return;

			CreateAsteroid(AsteroidType.Small);
			_counterSpawnEnemy.Reset();
		}
		else
		{
			GameConfig.GamePaused = true;
			_counterSpawnEnemy.Stop();
		}
	}

	public void NextWave()
	{
		//if (_timerWaveTimeCount.CheckTime() > 0) return;
		GameConfig.currentWaveNumber++;
		_spawnEnemyCounter = WaveTime / (AsteroidCount + 1);
		_timerWaveTimeCount.Start(WaveTime);
		_counterSpawnEnemy.Start();
		GameConfig.GamePaused = false;
	}

	public float GetWaveTimeLeft() => GameConfig.GamePaused ? 0 : _timerWaveTimeCount.CheckTime();

	public void CreateAsteroid(AsteroidType asteroidType)
	{
		var pointStart = MathFunctions.GetPointInsideCircle(_pointSpawnEnemyEntity, GameConfig.RadiusSpawnEnemyEntity);
		var planet = planetController.GetPlanetByType(PlanetType.Earth);
		var pointEnd = MathFunctions.GetPointInsideCircle(planet.GetPosition(),
			planet.radiusPlanet * GameConfig.RadiusDeSpawnRelativeToPlanetPositionAndRadius);
		pointEnd = MathFunctions.LineExtension(pointStart, pointEnd, 10f);
		float minSpeed = 10 + 10 * GameConfig.currentWaveNumber;
		float maxSpeed = 20 + 10 * GameConfig.currentWaveNumber;
		var newSpeed = Random.Range(minSpeed, maxSpeed);
		var pathPointsList = new List<Vector2> { pointStart, pointEnd };
		asteroidController.CreateAsteroid(asteroidType, pathPointsList, true, newSpeed);
	}
}
