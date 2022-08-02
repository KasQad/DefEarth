using System.Collections;
using System.Collections.Generic;
using Asteroids;
using ScriptableObject.Rockets;
using UnityEngine;

namespace Rockets
{
	public class RocketController : MonoBehaviour
	{
		[SerializeField] private GameObject rocketContainer;

		[SerializeField] private List<Transform> listKeyPoint;

		private readonly List<Entity> _rocketsList = new List<Entity>();

		private Bezier _bezier;

		private readonly Dictionary<Rocket.Type, BaseRocket> _prefabRocketList =
			new Dictionary<Rocket.Type, BaseRocket>();

		private void Awake()
		{
			_prefabRocketList.Add(Rocket.Type.RocketModel1,
				Resources.Load<BaseRocket>("Prefabs/Rockets/Rocket1"));
			_prefabRocketList.Add(Rocket.Type.RocketModel2,
				Resources.Load<BaseRocket>("Prefabs/Rockets/Rocket2"));
			_prefabRocketList.Add(Rocket.Type.RocketModel3,
				Resources.Load<BaseRocket>("Prefabs/Rockets/Rocket3"));

			_bezier = gameObject.GetComponent<Bezier>();
		}

		private void Start()
		{
			Entity.DestroyEntity += DestroyRocket;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R)) CreateRocket(Rocket.Type.RocketModel1, listKeyPoint);
			if (Input.GetKeyDown(KeyCode.T)) CreateRocket(Rocket.Type.RocketModel2, listKeyPoint);
			if (Input.GetKeyDown(KeyCode.Y)) CreateRocket(Rocket.Type.RocketModel3, listKeyPoint);
		}

		private void CreateRocket(Rocket.Type rocketType, List<Transform> newKeyPointList, bool enemy = false)
		{
			List<Vector2> newPathPointsList = _bezier.CreatePathPointsListByBezierMethod(newKeyPointList);

			if (newKeyPointList.Count < 2) return;

			if (!_prefabRocketList.TryGetValue(rocketType, out var prefabRocket)) return;

			var rocket = Instantiate(prefabRocket, rocketContainer.transform);
			rocket.Initialize(newPathPointsList, enemy);
			_rocketsList.Add(rocket);
		}

		private void DestroyRocket(Entity entity)
		{
			_rocketsList.Remove(entity);
			Destroy(entity.gameObject);
			// print($"_rocketsList.Count: {_rocketsList.Count}");
		}

	}
}
