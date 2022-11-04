using System;
using System.Collections.Generic;
using System.Linq;
using Asteroids;
using CustomTimers;
using Interfaces;
using Types;
using UnityEngine;

namespace Modules
{
	public class ModuleLaserGun : BaseModule
	{
		// [SerializeField] private CircleCollider2D _circleCollider2D;
		[SerializeField] private GameObject _laserLine;
		[SerializeField] private LineRenderer _lineRenderer;

		private float _pauseEffectTimeAttack = 0.1f;
		private float _damage = 10f;
		private float _rangeAttack = 10f;

		private Dictionary<Entity, Action<float, ImpactType>> _objectsInRangeAttack =
			new Dictionary<Entity, Action<float, ImpactType>>();

		private CustomTimerProcess _timerBlink;
		private CustomTimer _timerAttack;
		private CustomTimer _timerFind;

		private void Awake()
		{
			_timerAttack = gameObject.AddComponent<CustomTimer>();
			_timerBlink = gameObject.AddComponent<CustomTimerProcess>();
			_timerFind = gameObject.AddComponent<CustomTimer>();
		}

		private void Start()
		{
			_timerFind.InitTimer(0.1f, true, true);
			_timerFind.AddCallBack(CheckFind);
			_timerFind.Run();

			_timerAttack.InitTimer(_pauseEffectTimeAttack, true, false);
			_timerAttack.AddCallBack(CheckAttack);
			_timerAttack.Run();

			_timerBlink.InitTimerProcess((_pauseEffectTimeAttack *= .7f) / 2f, (_pauseEffectTimeAttack *= .3f) / 2f,
				true);
			//_timerBlink.Run();
		}

		private void OnDestroy()
		{
			_timerAttack.DelCallBack(CheckAttack);
		}

		private void Update()
		{
			AimingLaserAtTarget();
		}

		private void CheckAttack()
		{
			if (_objectsInRangeAttack.Count == 0) return;
			
			var entityFirst = _objectsInRangeAttack.Keys.First();
			if (entityFirst == null) return;
			_objectsInRangeAttack.Values.First()?.Invoke(_damage, ImpactType.LaserDrill);
		}

		private void AimingLaserAtTarget()
		{
			if (!ChExists() || _objectsInRangeAttack.Count == 0)
			{
				_lineRenderer.positionCount = 0;
				return;
			}

			// if (_timerBlink.CheckRun()) _laserLine.SetActive(_timerBlink.CheckTime());

			var entityFirst = _objectsInRangeAttack.Keys.First();
			_lineRenderer.positionCount = 0;
			if (entityFirst == null) return;
			_lineRenderer.positionCount = 2;
			Vector3 pos1 = (Vector3)entityFirst.GetPosition() - transform.parent.position;
			Vector3 pos2 = transform.parent.position - transform.position;
			_lineRenderer.SetPosition(0, pos1);
			_lineRenderer.SetPosition(1, pos2);
		}

		private void CheckFind()
		{
			DelAsteroidFromList();
			FindAsteroidAndAddToList();
		}

		private void DelAsteroidFromList()
		{
			if (_objectsInRangeAttack.Count == 0) return;

			var entityFirst = _objectsInRangeAttack.Keys.First();
			if (entityFirst == null)
			{
				_objectsInRangeAttack.Clear();
				_lineRenderer.positionCount = 0;
				return;
			}

			var a = entityFirst.GetPosition();
			var b = GetPosition();
			if (Vector2.Distance(a, b) > _rangeAttack)
			{
				_objectsInRangeAttack.Remove(entityFirst);
				_lineRenderer.positionCount = 0;
			}
		}

		private bool ChExists()
		{
			if (_objectsInRangeAttack.Count == 0) return false;
			var entityFirst = _objectsInRangeAttack.Keys.First();
			if (entityFirst == null) return false;
			return entityFirst.IsActive();
		}


		private void FindAsteroidAndAddToList()
		{
			if (_objectsInRangeAttack.Count != 0) return;

			var asteroidList = AsteroidSpawner.Instance.GetAsteroidsList();
			if (asteroidList.Count == 0) return;

			foreach (var asteroid in asteroidList)
			{
				var a = asteroid.GetPosition();
				var b = GetPosition();
				if (Vector2.Distance(a, b) <= _rangeAttack)
				{
					if (!asteroid.transform.TryGetComponent(out IDamageable damageable)) continue;
					_objectsInRangeAttack.Add(asteroid, damageable.ApplyDamage);
					return;
				}
			}
		}

	}
}
