using System.Collections.Generic;
using Types;
using UnityEngine;

namespace Rockets
{
	public class RocketSpawner : MonoBehaviour
	{
		private static RocketSpawner _instance;
		public static RocketSpawner Instance
		{
			get
			{
				if (_instance == null) _instance = FindObjectOfType<RocketSpawner>();
				return _instance;
			}
		}

		private readonly List<Entity> _rocketsList = new List<Entity>();

		private readonly Dictionary<RocketType, BaseRocket> _prefabRocketList =
			new Dictionary<RocketType, BaseRocket>();

		private void Awake()
		{
			_prefabRocketList.Add(RocketType.RocketModelA,
				Resources.Load<BaseRocket>("Prefabs/Rockets/RocketModelA"));
		}

		public BaseRocket CreateRocket(RocketType rocketType, List<Vector2> newPointList, bool isEnemy = false)
		{
			if (newPointList.Count < 2) return null;

			if (!_prefabRocketList.TryGetValue(rocketType, out var prefabRocket)) return null;
			var rocket = Instantiate(prefabRocket, transform);
			rocket.Initialize(newPointList, isEnemy);
			_rocketsList.Add(rocket);
			return rocket;
		}

		public List<Entity> GetRocketsList() => _rocketsList;

		public void DestroyRocket(Entity entity)
		{
			_rocketsList.Remove(entity);
			Destroy(entity.gameObject);
		}
	}
}
