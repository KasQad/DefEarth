using System.Collections.Generic;
using Asteroids;
using Planets;
using UnityEngine;
using UnityEngine.UI;
public class WaveEnemyController : MonoBehaviour
{
	[SerializeField] private PlanetController planetController;
	[SerializeField] private AsteroidController asteroidController;
	[SerializeField] private new Camera camera;
	[SerializeField] private Button buttonSmall;
	[SerializeField] private Button buttonMiddle;
	[SerializeField] private Button buttonBig;

	private Vector2 _pointSpawnEnemyEntity;
	[SerializeField] private float radiusSpawnEnemyEntity = 2f;

	[Range(0f, 2f)]
	[SerializeField] private float difficultySpawnEnemy = 0.5f;

	private void Awake()
	{
		buttonSmall.onClick.AddListener(() => CreateAsteroid(AsteroidType.Small));
		buttonMiddle.onClick.AddListener(() => CreateAsteroid(AsteroidType.Middle));
		buttonBig.onClick.AddListener(() => CreateAsteroid(AsteroidType.Big));
		
		_pointSpawnEnemyEntity = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
		_pointSpawnEnemyEntity = new Vector2(
			_pointSpawnEnemyEntity.x + radiusSpawnEnemyEntity,
			_pointSpawnEnemyEntity.y + radiusSpawnEnemyEntity - 4f);
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Z)) CreateAsteroid(AsteroidType.Small);
		if(Input.GetKeyDown(KeyCode.X)) CreateAsteroid(AsteroidType.Middle);
		if(Input.GetKeyDown(KeyCode.C)) CreateAsteroid(AsteroidType.Big);
	}

	private void CreateAsteroid(AsteroidType asteroidType)
	{
		var pointStart = MathFunctions.GetPointInsideCircle(_pointSpawnEnemyEntity, radiusSpawnEnemyEntity);
		var planet = planetController.GetPlanetByType(PlanetType.Earth);
		var pointEnd = MathFunctions.GetPointInsideCircle(planet.GetPosition(),
			planet.radiusPlanet + planet.radiusPlanet * difficultySpawnEnemy);
		pointEnd = MathFunctions.LineExtension(pointStart, pointEnd, 10f);

		var pathPointsList = new List<Vector2> { pointStart, pointEnd };
		asteroidController.CreateAsteroid(asteroidType, pathPointsList, true);
	}
}
