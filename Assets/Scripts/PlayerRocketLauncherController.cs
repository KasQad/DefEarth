using System.Collections.Generic;
using Asteroids;
using Planets;
using Rockets;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerRocketLauncherController : MonoBehaviour
{
	[SerializeField] private PlanetController planetController;
	[SerializeField] private RocketController rocketController;
	[SerializeField] private AsteroidController asteroidController;

	private readonly Dictionary<Entity, HashSet<BaseRocket>> _entitiesAimed =
		new Dictionary<Entity, HashSet<BaseRocket>>();

	private void Start()
	{
		AsteroidController.addEntityToHashSetAction += AddEntityToEntitiesAimedDictionary;
		AsteroidController.delEntityFromHashSetAction += DelEntityFromEntitiesAimedDictionary;
		BaseRocket.destroyRocket += FindAndDelEmptyRocketFromHashSet;
	}

	private void FixedUpdate()
	{
		LaunchRocketAtNearestFreeAsteroidFound();
		UpdateAimingEntitiesHashSet();
	}

	public void LaunchRocketAtNearestFreeAsteroidFound()
	{
		var planet = planetController.GetPlanetByType(PlanetType.Earth);

		var asteroidsList = asteroidController.GetAsteroidsList();
		if (asteroidsList.Count == 0) return;

		var whiteList = new HashSet<Entity>();

		foreach (var entity in asteroidsList)
		{
			if (Vector2.Distance(planet.GetPosition(), entity.GetPosition()) >
			    GameConfig.GetCurrentObjectObservationRadius()) continue;

			var amountOfDamageAimingEntities = GetAmountOfDamageAimingEntities(entity);
			if (amountOfDamageAimingEntities < entity.health) whiteList.Add(entity);
		}

		Entity asteroid = null;
		if (whiteList.Count > 0) asteroid = FindNearestAsteroidToPlayerEntity(planet, whiteList);
		if (asteroid == null) return;

		LaunchRocketToEntity(asteroid, planet);
	}

	private Entity FindNearestAsteroidToPlayerEntity(Entity planet, HashSet<Entity> entityList = null)
	{
		HashSet<Entity> asteroidsList;
		if (entityList != null) asteroidsList = entityList;
		else asteroidsList = asteroidController.GetAsteroidsList();

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
		var rocket = rocketController.CreateRocket(RocketType.RocketModel4, pathList);
		AddRocketToHashSet(entity, rocket);
	}

	private void UpdateAimingEntitiesHashSet()
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

	private void AddEntityToEntitiesAimedDictionary(Entity entity)
	{
		_entitiesAimed.Add(entity, new HashSet<BaseRocket>());
	}

	private void DelEntityFromEntitiesAimedDictionary(Entity entity)
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

	private void FindAndDelEmptyRocketFromHashSet(BaseRocket baseRocket)
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
