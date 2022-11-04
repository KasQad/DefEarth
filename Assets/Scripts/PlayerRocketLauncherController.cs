using System.Collections.Generic;
using Asteroids;
using CustomTimers;
using Planets;
using Rockets;
using Types;
using UnityEngine;

public class PlayerRocketLauncherController : MonoBehaviour
{
	private static PlayerRocketLauncherController _instance;

	public static PlayerRocketLauncherController Instance
	{
		get
		{
			if (_instance == null) _instance = FindObjectOfType<PlayerRocketLauncherController>();
			return _instance;
		}
	}

	[SerializeField] private PlanetSpawner planetSpawner;
	[SerializeField] private RocketSpawner rocketSpawner;
	[SerializeField] private AsteroidSpawner asteroidSpawner;

	private readonly Dictionary<Entity, HashSet<BaseRocket>> _entitiesAimed =
		new Dictionary<Entity, HashSet<BaseRocket>>();

	private CustomTimer _launchRocketTimer;
	private CustomTimer _updateAimingEntitiesTimer;

	private void Awake()
	{
		_launchRocketTimer = gameObject.AddComponent<CustomTimer>();
		_updateAimingEntitiesTimer = gameObject.AddComponent<CustomTimer>();
	}

	private void Start()
	{
		_launchRocketTimer.AddCallBack(() =>
		{
			UpdateAimingEntitiesList();
			LaunchRocketAtNearestFreeAsteroidFound();
		});
		_launchRocketTimer.InitTimer(.5f, true);
		_launchRocketTimer.Run();

		_updateAimingEntitiesTimer.InitTimer(.1f, true);
		_updateAimingEntitiesTimer.AddCallBack(UpdateAimingEntitiesList);
		_updateAimingEntitiesTimer.Run();
	}

	private void LaunchRocketAtNearestFreeAsteroidFound()
	{
		var planet = planetSpawner.GetPlanetByType(PlanetType.Earth);

		var asteroidsList = asteroidSpawner.GetAsteroidsList();
		if (asteroidsList.Count == 0) return;

		var whiteList = new HashSet<BaseAsteroid>();

		foreach (var baseAsteroid in asteroidsList)
		{
			if (Vector2.Distance(planet.GetPosition(), baseAsteroid.GetPosition()) >
			    GameConfig.GetCurrentObjectObservationRadius()) continue;

			var amountOfDamageAimingEntities = GetAmountOfDamageAimingEntities(baseAsteroid);
			if (amountOfDamageAimingEntities < baseAsteroid.Health) whiteList.Add(baseAsteroid);
		}

		Entity asteroid = null;
		if (whiteList.Count > 0) asteroid = FindNearestAsteroidToPlayerEntity(planet, whiteList);
		if (asteroid == null) return;

		LaunchRocketToEntity(asteroid, planet);
	}

	private Entity FindNearestAsteroidToPlayerEntity(Entity planet, HashSet<BaseAsteroid> entityList = null)
	{
		HashSet<BaseAsteroid> asteroidsList;
		if (entityList != null) asteroidsList = entityList;
		else asteroidsList = asteroidSpawner.GetAsteroidsList();

		if (asteroidsList.Count == 0) return null;

		Entity baseAsteroid = null;
		var minDistBetweenAsteroidAndPlanet = 0f;

		foreach (var asteroid in asteroidsList)
		{
			var distanceBetweenAsteroidAndPlanet = Vector2.Distance(asteroid.GetPosition(), planet.GetPosition());
			if (distanceBetweenAsteroidAndPlanet < minDistBetweenAsteroidAndPlanet ||
			    minDistBetweenAsteroidAndPlanet == 0f)
			{
				minDistBetweenAsteroidAndPlanet = distanceBetweenAsteroidAndPlanet;
				baseAsteroid = asteroid;
			}
		}

		return baseAsteroid;
	}

	private void LaunchRocketToEntity(Entity entity, BasePlanet planet)
	{
		if (entity == null) return;
		var randomAnglePointStartOnPlanet = Random.Range(0f, 360);
		var pointStart = MathFunctions.GetXYCoordsOnBorderCircleByAngle(
			planet.GetPosition(), planet.radiusPlanet, randomAnglePointStartOnPlanet);

		var pointMiddle = MathFunctions.GetXYCoordsOnBorderCircleByAngle(
			planet.GetPosition(), planet.radiusOrbitSpaceFragments, randomAnglePointStartOnPlanet);

		var pointEnd = entity.GetPosition();
		var pathList = new List<Vector2> { pointStart, pointMiddle, pointEnd };
		var rocket = rocketSpawner.CreateRocket(RocketType.RocketModelA, pathList);
		AddRocketToHashSet(entity, rocket);
	}

	private void UpdateAimingEntitiesList()
	{
		if (_entitiesAimed.Count == 0) return;
		foreach (var entityAimedList in _entitiesAimed)
		{
			if (entityAimedList.Value.Count == 0) continue;
			foreach (var baseRocket in entityAimedList.Value)
			{
				if (baseRocket == null) continue;
				baseRocket.SetTarget(entityAimedList.Key);
			}
		}
	}

	public void AddEntityToEntitiesAimedDictionary(Entity entity)
	{
		_entitiesAimed.Add(entity, new HashSet<BaseRocket>());
	}

	public void DelEntityFromEntitiesAimedDictionary(Entity entity)
	{
		if (_entitiesAimed.ContainsKey(entity)) _entitiesAimed.Remove(entity);
	}

	private void AddRocketToHashSet(Entity entity, BaseRocket baseRocket)
	{
		if (_entitiesAimed.ContainsKey(entity)) _entitiesAimed[entity].Add(baseRocket);
	}

	private void DelRocketFromHashSet(Entity entity, BaseRocket baseRocket)
	{
		if (_entitiesAimed.ContainsKey(entity)) _entitiesAimed[entity].Remove(baseRocket);
	}

	public void FindAndDelEmptyRocketFromHashSet(BaseRocket baseRocket)
	{
		if (_entitiesAimed.Count == 0) return;
		foreach (var entityAimedList in _entitiesAimed)
		{
			if (entityAimedList.Value.Contains(baseRocket))
			{
				entityAimedList.Value.Remove(baseRocket);
				return;
			}
		}
	}

	private HashSet<BaseRocket> GetAimingEntitiesHashSet(Entity entity) =>
		_entitiesAimed.TryGetValue(entity, out var aimingEntitiesHashSet) ? aimingEntitiesHashSet : null;

	private float GetAmountOfDamageAimingEntities(Entity entity)
	{
		var aimingEntitiesHashSet = GetAimingEntitiesHashSet(entity);
		if (aimingEntitiesHashSet.Count == 0) return 0;
		var rez = 0f;
		foreach (var aimingEntity in aimingEntitiesHashSet)
			if (aimingEntity.entityType == EntityType.Rocket)
				rez += aimingEntity.damage;
		return rez;
	}
}
