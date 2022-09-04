using System;
using System.Collections.Generic;
using System.Linq;
using CustomTimers;
using Interfaces;
using UnityEngine;

namespace Modules
{
	public class ModuleLaserGun : BaseModule
	{
		[SerializeField] private CircleCollider2D _circleCollider2D;
		[SerializeField] private GameObject _laserLine;
		[SerializeField] private LineRenderer _lineRenderer;

		private float _pauseEffectTimeAttack = 0.1f;
		private float _damage = 10f;
		private float _rangeAttack = 6;

		private Dictionary<Entity, Action<float, ImpactType>> _objectsInRangeAttack =
			new Dictionary<Entity, Action<float, ImpactType>>();

		private CustomTimerProcess _timerBlink;
		private CustomTimer _timerAttack;

		private void Awake()
		{
			_timerAttack = gameObject.AddComponent<CustomTimer>();
			_timerBlink = gameObject.AddComponent<CustomTimerProcess>();

			_circleCollider2D.radius = _rangeAttack;
		}

		private void Start()
		{
			_timerAttack.InitTimer(_pauseEffectTimeAttack, true, false);
			_timerAttack.AddTask(CheckAttack);
			_timerAttack.Run();

			_timerBlink.InitTimerProcess((_pauseEffectTimeAttack *= .3f) / 2f, (_pauseEffectTimeAttack *= .7f) / 2f,
				true);
			// _timerBlink.Run();

			// BaseAsteroid.destroyAsteroidAction += DelFromList;
		}

		private void OnDestroy()
		{
			// BaseAsteroid.destroyAsteroidAction -= DelFromList;
			_timerAttack.DelTask(CheckAttack);
		}

		private void Update()
		{
			if (_objectsInRangeAttack.Count == 0)
			{
				_lineRenderer.positionCount = 0;
				return;
			}

			if (_timerBlink.CheckRun()) _laserLine.SetActive(_timerBlink.CheckTime());

			var entityFirst = _objectsInRangeAttack.Keys.First();
			_lineRenderer.positionCount = 0;
			if (entityFirst == null) return;
			_lineRenderer.positionCount = 2;
			Vector3 pos1 = (Vector3)entityFirst.GetPosition() - transform.parent.position;
			Vector3 pos2 = transform.parent.position - transform.position;
			_lineRenderer.SetPosition(0, pos1);
			_lineRenderer.SetPosition(1, pos2);
		}

		private void CheckAttack()
		{
			if (_objectsInRangeAttack.Count == 0) return;
			_objectsInRangeAttack.Values.First()?.Invoke(_damage, ImpactType.LaserGun);
		}

		protected void OnCollisionEnter2D(Collision2D col)
		{
			if (!col.transform.TryGetComponent(out Entity entity)) return;
			if (entity.isEnemy == isEnemy) return;
			if (entity.entityType != EntityType.Asteroid) return;
			if (!col.transform.TryGetComponent(out IDamageable damageable)) return;
			_objectsInRangeAttack.Add(entity, damageable.ApplyDamage);
			// print($"_objectsInRangeAttack.Count: {_objectsInRangeAttack.Count}");
		}

		protected void OnCollisionExit2D(Collision2D col)
		{
			if (!col.transform.TryGetComponent(out Entity entity)) return;
			if (_objectsInRangeAttack.ContainsKey(entity)) _objectsInRangeAttack.Remove(entity);
			// print($"_objectsInRangeAttack.Count: {_objectsInRangeAttack.Count}");
		}

		public override void Use()
		{
			print($"ModuleLaserDrill : override Use()");
		}
	}
}
