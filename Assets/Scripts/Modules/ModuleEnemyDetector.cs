using System;
using System.Collections.Generic;
using Types;
using UnityEngine;

namespace Modules
{
	public class ModuleEnemyDetector : BaseModule
	{
		[SerializeField] private CircleCollider2D _circleCollider2D;

		[SerializeField] private float _radiusDetector;
		private List<EntityType> _targetsTypesEntity = new List<EntityType>();
		private bool _targetsIsEnemy;

		public Action<Entity> detectOnCollisionEnter2D;
		public Action<Entity> detectOnCollisionStay2D;

		private void Awake()
		{
			_circleCollider2D.radius = _radiusDetector;
		}

		public void SetConfig(float radiusDetector, List<EntityType> targetsTypesEntity, bool targetsIsEnemy)
		{
			_targetsTypesEntity = targetsTypesEntity;
			_targetsIsEnemy = targetsIsEnemy;
			_circleCollider2D.radius = _radiusDetector = radiusDetector;
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			if (!isRunning) return;
			if (_targetsTypesEntity.Count == 0) return;

			if (!col.transform.TryGetComponent(out Entity entity)) return;

			foreach (var targetEntityType in _targetsTypesEntity)
			{
				if (targetEntityType.Equals(entity.entityType) && _targetsIsEnemy == entity.isEnemy)
				{
					print($"detectOnCollisionEnter2D title: {entity.title}");
					detectOnCollisionEnter2D?.Invoke(entity);
				}
			}
		}

		private void OnCollisionStay2D(Collision2D collision)
		{
			if (!isRunning) return;
			if (_targetsTypesEntity.Count == 0) return;

			if (!collision.transform.TryGetComponent(out Entity entity)) return;
			if (entity.isEnemy == isEnemy) return;

			foreach (var targetEntityType in _targetsTypesEntity)
			{
				if (targetEntityType == entity.entityType && _targetsIsEnemy == entity.isEnemy)
				{
					// print($"detectOnCollisionStay2D title: {entity.title}");
					detectOnCollisionStay2D?.Invoke(entity);
				}
			}
		}

		protected void OnCollisionExit2D(Collision2D col)
		{
			// if (!_isRunning) return;
			//
			// if (!col.transform.TryGetComponent(out Entity entity)) return;
			// if (_objectsInRangeAttack.ContainsKey(entity)) _objectsInRangeAttack.Remove(entity);
		}

	}
}
