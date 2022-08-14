using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
	public class RocketController : MonoBehaviour
	{
		[SerializeField] private GameObject rocketContainer;
		[SerializeField] private List<Transform> listKeyPoint;

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
		}

		private void Start()
		{
			BaseRocket.destroyRocket += DestroyRocket;
		}

		private void Update()
		{
			List<Vector2> pointList = new List<Vector2>();
			foreach (var point in listKeyPoint)
				pointList.Add(point.transform.position);

			if(Input.GetKeyDown(KeyCode.R)) CreateRocket(RocketType.RocketModel1, pointList);
			if(Input.GetKeyDown(KeyCode.T)) CreateRocket(RocketType.RocketModel2, pointList);
			if(Input.GetKeyDown(KeyCode.Y)) CreateRocket(RocketType.RocketModel3, pointList);
		}

		public BaseRocket CreateRocket(RocketType rocketType, List<Vector2> newPointList, bool enemy = false)
		{
			if(newPointList.Count < 2) return null;

			if(!_prefabRocketList.TryGetValue(rocketType, out var prefabRocket)) return null;

			var rocket = Instantiate(prefabRocket, rocketContainer.transform);
			MathFunctions.RotateTo(rocket.transform, newPointList[1]);
			rocket.Initialize(newPointList, enemy);
			_rocketsList.Add(rocket);
			return rocket;
		}

		public List<Entity> GetRocketsList() => _rocketsList;
		
		
		private void DestroyRocket(Entity entity)
		{
			_rocketsList.Remove(entity);
			Destroy(entity.gameObject);
			// print($"DestroyRocket: {entity.title}");
		}
	}
}
