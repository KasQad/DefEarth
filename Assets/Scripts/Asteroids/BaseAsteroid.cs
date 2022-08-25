using System;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using ScriptableObject.Asteroids;
using Ui;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
	public class BaseAsteroid : Entity, IDamageable
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private HpBarController hpBarController;
		public float speed;
		public float speedRotate;
		public Asteroid asteroid;
		public int currentPointIndex;
		public List<Vector2> pathPointsList;

		public static Action<Vector2> createRandomFragmentsAsteroid;
		public static Action<Entity> destroyAsteroidAction;
		private Action<float> _changeHp;

		private Sequence _sequence;
		private Vector3 _defaultTransformScale;
		private Color _defaultSpriteRendererColor;

		public void Initialize(List<Vector2> newPathPointsList, bool isEnemy = true, float newSpeed = 0)
		{
			title = name = $"{asteroid.title} #{Random.Range(0, 100000)}";
			transform.position = newPathPointsList[0];
			speed = newSpeed > 0 ? newSpeed / 1000 : Random.Range(asteroid.speedMin, asteroid.speedMax) / 1000;
			speedRotate = newSpeed * (Random.Range(0, 2) == 0 ? 1f : -1f);
			health = asteroid.health;
			damage = asteroid.damage;
			pathPointsList = newPathPointsList;
			IsEnemy = isEnemy;
			entityType = EntityType.Asteroid;

			_defaultTransformScale = transform.localScale;
			_defaultSpriteRendererColor = spriteRenderer.color;

			hpBarController?.Initialize(this, ref _changeHp, asteroid.health, asteroid.health);
		}

		private void FixedUpdate()
		{
			transform.Rotate(Vector3.forward * (Time.deltaTime * speedRotate));
			Move();
		}

		private void Move()
		{
			if (currentPointIndex >= pathPointsList.Count || pathPointsList.Count == 0)
			{
				Destroy();
				return;
			}

			var position = transform.position;
			position = Vector3.MoveTowards(position, pathPointsList[currentPointIndex], speed);
			gameObject.transform.position = position;

			if (position.Equals(pathPointsList[currentPointIndex]))
				currentPointIndex++;
		}

		private void PlayEffectContactEntity()
		{
			_sequence?.Kill();
			transform.localScale = _defaultTransformScale;
			spriteRenderer.color = _defaultSpriteRendererColor;
			_sequence = DOTween.Sequence();
			_sequence.Append(spriteRenderer.DOColor(new Color(255, 0, 0, 1), 0.05f)
				.SetEase(Ease.InOutExpo));
			_sequence.Append(transform.DOShakeScale(0.1f,
				new Vector2(_defaultTransformScale.x * 0.3f, _defaultTransformScale.y * 0.3f)));
			_sequence.Append(spriteRenderer.DOColor(_defaultSpriteRendererColor, 0.05f)
				.SetEase(Ease.InOutExpo));
		}

		public void ApplyDamage(Entity entity)
		{
			if (entity.IsEnemy == IsEnemy) return;
			createRandomFragmentsAsteroid?.Invoke(GetPosition());
			health -= entity.damage;

			_changeHp?.Invoke(health);

			PlayEffectContactEntity();

			if (health <= 0) Destroy();
		}

		private void Destroy()
		{
			_sequence?.Kill();
			destroyAsteroidAction?.Invoke(this);
		}
	}
}
