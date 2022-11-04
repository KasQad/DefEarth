using System.Collections.Generic;
using Asteroids;
using CustomTimers;
using Planets;
using Types;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveEnemyController : MonoBehaviour
{
	[SerializeField] private PlanetSpawner planetSpawner;
	[SerializeField] private AsteroidSpawner asteroidSpawner;
	[SerializeField] private new Camera camera;
	[SerializeField] private LinesManager linesManager;

	private Vector2 _pointStartSpawnEnemyEntity;
	private Vector2 _pointEndSpawnEnemyEntity;

	private const float WaveTime = 5;
	private const int EnemyCount = 3;
	private float _spawnEnemyCounter;

	private CustomTimer _counterSpawnEnemy;
	private CustomTimer _timerWaveTimeCount;

	private void Awake()
	{
		_counterSpawnEnemy = gameObject.AddComponent<CustomTimer>();
		_timerWaveTimeCount = gameObject.AddComponent<CustomTimer>();

		var cameraScreenToWorldPoint = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

		_pointStartSpawnEnemyEntity = new Vector2(cameraScreenToWorldPoint.x + 2f, cameraScreenToWorldPoint.y);
		_pointEndSpawnEnemyEntity = new Vector2(cameraScreenToWorldPoint.x + 2f, -cameraScreenToWorldPoint.y / 2);

		linesManager.DrawLine(transform, new List<Vector2> { _pointStartSpawnEnemyEntity, _pointEndSpawnEnemyEntity },
			0.03f, Color.yellow);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z)) CreateAsteroid(AsteroidType.Small);
		if (Input.GetKeyDown(KeyCode.X)) CreateAsteroid(AsteroidType.Middle);
		if (Input.GetKeyDown(KeyCode.C)) CreateAsteroid(AsteroidType.Big);
		if (Input.GetKeyDown(KeyCode.W)) NextWave();

		if (!_timerWaveTimeCount.CheckTime())
		{
			if (GameConfig.GamePaused) return;

			if (!_counterSpawnEnemy.CheckTime()) return;
			CreateAsteroid(AsteroidType.Small);
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
		_spawnEnemyCounter = WaveTime / (EnemyCount + 1);

		print($"WaveTime: {WaveTime}");
		_timerWaveTimeCount.InitTimer(WaveTime);
		_timerWaveTimeCount.Run();

		print($"_timerWaveTimeCount.GetLeftTime(): {_timerWaveTimeCount.GetLeftTime()}");

		_counterSpawnEnemy.InitTimer(_spawnEnemyCounter, true);
		_counterSpawnEnemy.Run();

		GameConfig.GamePaused = false;
	}

	public float GetWaveTimeLeft()
	{
		if (!GameConfig.GamePaused) return _timerWaveTimeCount.GetLeftTime();
		return 0;
	}

	public void CreateAsteroid(AsteroidType asteroidType)
	{
		var pointStart =
			MathFunctions.RandomPointBetween2Points(_pointStartSpawnEnemyEntity, _pointEndSpawnEnemyEntity);

		var planet = planetSpawner.GetPlanetByType(PlanetType.Earth);
		var pointEnd = MathFunctions.GetPointInsideCircle(planet.GetPosition(),
			planet.radiusPlanet * GameConfig.RadiusDeSpawnRelativeToPlanetPositionAndRadius);
		pointEnd = MathFunctions.LineExtension(pointStart, pointEnd, 10f);
		var pathPointsList = new List<Vector2> { pointStart, pointEnd };
		asteroidSpawner.CreateAsteroid(asteroidType, pathPointsList, true, GameConfig.currentWaveNumber);
	}
}
