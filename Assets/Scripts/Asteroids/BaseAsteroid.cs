using System.Collections.Generic;
using AsteroidFragments;
using DG.Tweening;
using HpBar;
using Interfaces;
using ScriptableObject.Asteroids;
using Types;
using UnityEngine;

namespace Asteroids
{
	public class BaseAsteroid : Entity, IDamageable, IRewardable
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private Transform _asteroidTransform;
		[SerializeField] private HpBarController _hpBarController;

		[SerializeField] private int _reward;
		[SerializeField] private float _hp;

		public float Health { get; set; }
		public long Reward { get; set; }

		[SerializeField] public int asteroidFragments;
		public float speed;
		public float speedRotate;
		public Asteroid asteroid;
		public int currentPointIndex;
		public List<Vector2> pathPointsList;
		private bool _isDestroyed;

		private Sequence _sequence;
		private Vector3 _defaultTransformScale;
		private Color _defaultSpriteRendererColor;

		private HpBarSimple _hpBarSimple;

		public void Initialize(List<Vector2> newPathPointsList, bool newIsEnemy = true, int waveNumber = 1)
		{
			title = name = $"{asteroid.title} #{Random.Range(0, 100000)}";
			transform.position = newPathPointsList[0];

			asteroidFragments = Random.Range(asteroid.asteroidFragmentsMin, asteroid.asteroidFragmentsMax) + waveNumber;

			var tempSpeed = Random.Range(asteroid.speedMin, asteroid.speedMax) / 1000;
			speed = tempSpeed + tempSpeed * 0.2f * waveNumber;
			speedRotate = waveNumber * (Random.Range(0, 2) == 0 ? 1f : -1f);

			var health = Health = asteroid.health + asteroid.health * 0.2f * waveNumber;

			damage = asteroid.damage;
			pathPointsList = newPathPointsList;
			isEnemy = newIsEnemy;
			entityType = EntityType.Asteroid;

			Reward = _reward = asteroid.reward * GameConfig.currentWaveNumber;

			_defaultTransformScale = transform.localScale;
			_defaultSpriteRendererColor = _spriteRenderer.color;

			_hpBarSimple = _hpBarController.Initialize(health, health);
		}

		private void FixedUpdate()
		{
			_asteroidTransform.Rotate(Vector3.forward * (Time.deltaTime * speedRotate));
			Move();
		}

		private void Move()
		{
			if (currentPointIndex >= pathPointsList.Count || pathPointsList.Count == 0)
			{
				DestroyAsteroid();
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
			_spriteRenderer.color = _defaultSpriteRendererColor;
			_sequence?.Kill();
			_sequence = DOTween.Sequence();
			_sequence.Append(_spriteRenderer.DOColor(new Color(255, 0, 0, 1), 0.05f)
				.SetEase(Ease.InOutExpo));
			_sequence.Append(transform.DOShakeScale(0.1f,
				new Vector2(_defaultTransformScale.x * 0.3f, _defaultTransformScale.y * 0.3f)));
			_sequence.Append(_spriteRenderer.DOColor(_defaultSpriteRendererColor, 0.05f)
				.SetEase(Ease.InOutExpo));
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			
			if (!col.transform.TryGetComponent(out Entity hit1)) return;
			if (hit1.isEnemy == isEnemy) return;
			//if (hit1.entityType != EntityType.Rocket) return;
			if (!col.transform.TryGetComponent(out IDamageable hit)) return;
			hit.ApplyDamage(damage, ImpactType.Collision);
			_hpBarSimple.SetValue(Health);
		}

		public void ApplyDamage(float damageValue, ImpactType impactType)
		{
			if (_isDestroyed) return;

			var countAsteroidFragments =
				Mathf.CeilToInt((damageValue >= Health ? 1 : damageValue / Health) * asteroidFragments);
			asteroidFragments -= countAsteroidFragments;
			if (asteroidFragments <= 0) asteroidFragments = 0;
			AsteroidFragmentsSpawner.Instance.CreateRandomAsteroidFragments(GetPosition(), countAsteroidFragments);

			Health -= damageValue;
			if (Health <= 0)
			{
				_isDestroyed = true;
				DestroyAsteroid(impactType);
				return;
			}

			_hp = Health;
			_hpBarSimple.SetValue(Health);
			PlayEffectContactEntity();
		}

		private void DestroyAsteroid(ImpactType impactType = ImpactType.LiveTimeLimit)
		{
			if (impactType != ImpactType.Collision && impactType != ImpactType.LiveTimeLimit)
				GetReward(Reward);
			_sequence?.Kill();
			AsteroidSpawner.Instance.DestroyAsteroid(this);
		}
	}
}
