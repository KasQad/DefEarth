using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
	public class RocketSpawner : MonoBehaviour
	{
		private readonly List<Entity> _rocketsList = new List<Entity>();

		private readonly Dictionary<RocketType, BaseRocket> _prefabRocketList =
			new Dictionary<RocketType, BaseRocket>();

		private void Awake()
		{
			_prefabRocketList.Add(RocketType.RocketModel1,
				Resources.Load<BaseRocket>("Prefabs/Rockets/Rocket1"));
			_prefabRocketList.Add(RocketType.RocketModel2,
				Resources.Load<BaseRocket>("Prefabs/Rockets/Rocket2"));
			_prefabRocketList.Add(RocketType.RocketModel3,
				Resources.Load<BaseRocket>("Prefabs/Rockets/Rocket3"));
			_prefabRocketList.Add(RocketType.RocketModel4,
				Resources.Load<BaseRocket>("Prefabs/Rockets/Rocket4"));
		}

		private void Start()
		{
			BaseRocket.destroyRocket += DestroyRocket;
		}

		private void OnDestroy()
		{			
			BaseRocket.destroyRocket -= DestroyRocket;
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

		private void DestroyRocket(Entity entity)
		{
			_rocketsList.Remove(entity);
			Destroy(entity.gameObject);
		}
	}
}
