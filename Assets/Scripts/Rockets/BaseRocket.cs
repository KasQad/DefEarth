using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObject.Rockets;
using Types;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rockets
{
	public class BaseRocket : Entity, IDamageable
	{
		public float Health { get; set; }
		
		public Rocket rocket;
		public float speed;
		public float angularSpeed;
		public float accumulativeBrakingSpeed;

		public List<Vector2> pathPointsList;
		private int _currentPointIndex = 1;

		public void Initialize(List<Vector2> newPathPointsList, bool enemy = false)
		{
			title = name = $"{rocket.title} #{Random.Range(0, 100000)}";
			transform.position = newPathPointsList[0];
			MathFunctions.LookAt2D(transform, newPathPointsList[1]);
			speed = rocket.speed / 1000;
			angularSpeed = rocket.angularSpeed;
			Health = rocket.health;
			damage = rocket.damage;
			pathPointsList = newPathPointsList;
			isEnemy = enemy;
			entityType = EntityType.Rocket;
		}

		private void FixedUpdate()
		{
			Move();
		}

		private void Move()
		{
			if (_currentPointIndex >= pathPointsList.Count || pathPointsList.Count == 0)
			{
				DestroyRocket();
				return;
			}
			LookAtTarget(gameObject.transform, pathPointsList[_currentPointIndex], angularSpeed);
			transform.Translate(Vector3.up * (speed - speed * accumulativeBrakingSpeed / 100));
			var distance = Vector2.Distance(transform.position, pathPointsList[_currentPointIndex]);
			if (distance < 0.2f) _currentPointIndex++;
		}

		private void LookAtTarget(Transform objectTransform, Vector3 targetPosition, float newAngularSpeed)
		{
			var signedAngle = Vector2.SignedAngle(objectTransform.up, targetPosition - objectTransform.position);

			if (Mathf.Abs(signedAngle) > 15f)
			{
				var angles = objectTransform.eulerAngles;
				angles.z += signedAngle > 0 ? newAngularSpeed : -newAngularSpeed;
				objectTransform.eulerAngles = angles;
			}

			if (Mathf.Abs(signedAngle) > 20f)
			{
				if (accumulativeBrakingSpeed < 70) accumulativeBrakingSpeed += 0.1f;
			}
			else if (accumulativeBrakingSpeed > 0) accumulativeBrakingSpeed -= 1f;

			if (accumulativeBrakingSpeed > 70) accumulativeBrakingSpeed = 70;
			if (accumulativeBrakingSpeed < 0) accumulativeBrakingSpeed = 0;
		}


		public void SetTarget(Entity entity)
		{
			pathPointsList[pathPointsList.Count - 1] = entity.GetPosition();
		}

		internal void OnCollisionEnter2D(Collision2D col)
		{
			if (!col.transform.TryGetComponent(out Entity entity)) return;
			if (entity.isEnemy == isEnemy) return;
			
			if (entity.entityType != EntityType.Asteroid &&
			    entity.entityType != EntityType.Warship) return;

			if (!col.transform.TryGetComponent(out IDamageable damageable)) return;
			damageable.ApplyDamage(damage, ImpactType.Explosive);
		}
		
		private void DestroyRocket()
		{
			RocketSpawner.Instance.DestroyRocket(this);
			PlayerRocketLauncherController.Instance.FindAndDelEmptyRocketFromHashSet(this);
		}


		public void ApplyDamage(float damageValue, ImpactType impactType)
		{
			print($"BaseRocket: ApplyDamage() : newDamage: {damageValue}");
			Health -= damageValue;
			if (Health <= 0) DestroyRocket();
		}
	}
}
