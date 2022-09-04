using System;
using System.Collections.Generic;
using DG.Tweening;
using HpBar;
using Interfaces;
using ScriptableObject.Asteroids;
using Ui;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
	public class BaseAsteroid : Entity, IDamageable, IRewardable
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private Transform asteroidTransform;
		[SerializeField] private HpBarController hpBarController;

		[SerializeField] private int reward;
		[SerializeField] private float hp;

		public float Health { get; set; }
		public long Reward { get; set; }

		public float speed;
		public float speedRotate;
		public Asteroid asteroid;
		public int currentPointIndex;
		public List<Vector2> pathPointsList;
		private bool _isDestroyed;

		public static Action<Vector2, int> createRandomFragmentsAsteroid;
		public static Action<BaseAsteroid> destroyAsteroidAction;

		private Sequence _sequence;
		private Vector3 _defaultTransformScale;
		private Color _defaultSpriteRendererColor;

		private HpBar.HpBar _hpBar;

		public void Initialize(List<Vector2> newPathPointsList, bool newIsEnemy = true, float newSpeed = 0)
		{
			title = name = $"{asteroid.title} #{Random.Range(0, 100000)}";
			transform.position = newPathPointsList[0];
			speed = newSpeed > 0 ? newSpeed / 1000 : Random.Range(asteroid.speedMin, asteroid.speedMax) / 1000;
			speedRotate = newSpeed * (Random.Range(0, 2) == 0 ? 1f : -1f);
			Health = asteroid.health;
			damage = asteroid.damage;
			pathPointsList = newPathPointsList;
			isEnemy = newIsEnemy;
			entityType = EntityType.Asteroid;

			Reward = reward = asteroid.reward * GameConfig.currentWaveNumber;

			_defaultTransformScale = transform.localScale;
			_defaultSpriteRendererColor = spriteRenderer.color;

			_hpBar = hpBarController.Initialize(asteroid.health, asteroid.health);
		}

		private void FixedUpdate()
		{
			asteroidTransform.Rotate(Vector3.forward * (Time.deltaTime * speedRotate));
			Move();
		}

		private void Move()
		{
			if (currentPointIndex >= pathPointsList.Count || pathPointsList.Count == 0)
			{
				Destroy(ImpactType.LiveTimeLimit);
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
			transform.localScale = _defaultTransformScale;
			spriteRenderer.color = _defaultSpriteRendererColor;
			_sequence?.Kill();
			_sequence = DOTween.Sequence();
			_sequence.Append(spriteRenderer.DOColor(new Color(255, 0, 0, 1), 0.05f)
				.SetEase(Ease.InOutExpo));
			_sequence.Append(transform.DOShakeScale(0.1f,
				new Vector2(_defaultTransformScale.x * 0.3f, _defaultTransformScale.y * 0.3f)));
			_sequence.Append(spriteRenderer.DOColor(_defaultSpriteRendererColor, 0.05f)
				.SetEase(Ease.InOutExpo));
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			if (!col.transform.TryGetComponent(out Entity hit1)) return;
			if (hit1.isEnemy == isEnemy) return;
			//if (hit1.entityType != EntityType.Rocket) return;
			if (!col.transform.TryGetComponent(out IDamageable hit)) return;
			hit.ApplyDamage(damage, ImpactType.Collision);
			_hpBar.SetValue(Health);
		}

		public void ApplyDamage(float damageValue, ImpactType impactType)
		{
			if (_isDestroyed) return;
			Health -= damageValue;
			if (Health <= 0)
			{
				_isDestroyed = true;
				Destroy(impactType);
				return;
			}

			hp = Health;
			_hpBar.SetValue(Health);
			PlayEffectContactEntity();
			createRandomFragmentsAsteroid?.Invoke(GetPosition(), 0);
		}

		private void Destroy(ImpactType impactType = ImpactType.LiveTimeLimit)
		{
			if (impactType != ImpactType.Collision && impactType != ImpactType.LiveTimeLimit)
				GetReward(Reward);
			_sequence?.Kill();
			destroyAsteroidAction?.Invoke(this);
		}
	}
}
