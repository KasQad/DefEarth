using System;
using System.Collections;
using System.Collections.Generic;
using Asteroids;
using Planets;
using Rockets;
using UnityEngine;
using Random = UnityEngine.Random;
// ReSharper disable All

public class PlayerRocketLauncherController : MonoBehaviour
{
	[SerializeField] private PlanetController planetController;
	[SerializeField] private RocketController rocketController;
	[SerializeField] private AsteroidController asteroidController;
	[SerializeField] private LinesManager linesManager;

	private readonly Dictionary<Entity, HashSet<BaseRocket>> _entitiesAimed = new Dictionary<Entity, HashSet<BaseRocket>>();

	public static Action<Entity> addEntityToHashSetAction;
	public static Action<Entity> delEntityFromHashSetAction;


	private void Start()
	{
		StartCoroutine(StartRockets());
		addEntityToHashSetAction += AddEntityToEntitiesAimedDictionary;
		delEntityFromHashSetAction += DelEntityFromEntitiesAimedDictionary;
	}

	IEnumerator StartRockets()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.10f);
			FindNearestFreeAsteroidToPlayerEntity();
			UpdateAimingEntitiesHashSet();
		}
	}

	private void FindNearestFreeAsteroidToPlayerEntity()
	{
		var planet = planetController.GetPlanetByType(PlanetType.Earth);

		var asteroidsList = asteroidController.GetAsteroidsList();
		if (asteroidsList.Count==0) return;

		Entity asteroid=null;
			
		var whiteList = new HashSet<Entity>();

		foreach (var entity in asteroidsList)
		{
			var amountOfDamageAimingEntities = GetAmountOfDamageAimingEntities(entity);
			if (amountOfDamageAimingEntities <= entity.health) whiteList.Add(entity);
		}
		
		if (whiteList.Count>0)
			asteroid = FindNearestAsteroidToPlayerEntity(planet, whiteList);
		
		if (asteroid == null) return;

		linesManager.DrawLine(transform, new List<Vector2> { planet.GetPosition(), asteroid.GetPosition() }, 0.01f,
			Color.blue);

		LunchRocketToEntity(asteroid, planet);
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

	private void LunchRocketToEntity(Entity entity, BasePlanet planet)
	{
		if (entity == null) return;
		var randomAnglePointStartOnPlanet = Random.Range(0f, 360);
		var pointStart = MathFunctions.GetXYCoordsOnBorderCircleByAngle(
			planet.GetPosition(), planet.radiusPlanet, randomAnglePointStartOnPlanet);
		var pointEnd = entity.GetPosition();
		var pathList = new List<Vector2> { pointStart, pointEnd };
		var rocket = rocketController.CreateRocket(RocketType.RocketModel3, pathList);
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

	private HashSet<BaseRocket> GetAimingEntitiesHashSet(Entity entity) =>
		_entitiesAimed.TryGetValue(entity, out var aimingEntitiesList) ? aimingEntitiesList : null;

	private float GetAmountOfDamageAimingEntities(Entity entity)
	{
		var aimingEntitiesList = GetAimingEntitiesHashSet(entity);
		if (aimingEntitiesList.Count == 0) return 0;
		var rez = 0f;
		foreach (var aimingEntity in aimingEntitiesList)
		{
			if (aimingEntity.entityType != EntityType.Rocket) continue;
			rez += aimingEntity.damage;
		}
		return rez;
	}
}
