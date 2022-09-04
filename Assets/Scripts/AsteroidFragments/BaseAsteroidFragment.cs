using System;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using ScriptableObject.AsteroidFragments;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteroidFragments
{
	public class BaseAsteroidFragment : Entity, IDamageable, IRewardable
	{
		public AsteroidFragment asteroidFragment;
		[SerializeField] private new Collider2D collider2D;
		[SerializeField] private SpriteRenderer spriteRenderer;

		[SerializeField] private float hp;

		public float Health { get; set; }
		public long Reward { get; set; }

		public float speed;
		public float speedRotate;
		private bool _isDestroyed;

		public List<Vector2> pathPointsList = new List<Vector2>();
		private int _currentPointIndex;

		public PlanetType? isInOrbitPlanet;
		private float _angleOnOrbitPlanet;
		private float _radiusOrbitSpaceFragments;
		private Vector2 _positionPlanet;
		private int _directionMovingOnOrbitPlanet;

		private Sequence _sequence;
		private Vector3 _defaultTransformScale;
		private Color _defaultSpriteRendererColor;

		public static Action<BaseAsteroidFragment, float> destroyAsteroidFragment;

		public PlanetType? CheckIsInOrbitPlanet() => isInOrbitPlanet;

		private void FixedUpdate()
		{
			gameObject.transform.Rotate(Vector3.back * speedRotate);
			Move();
		}

		public void Initialize(List<Vector2> newPathPointsList, bool enemy = true)
		{
			title = gameObject.name = $"{asteroidFragment.title} #{Random.Range(0, 100000)}";
			transform.position = newPathPointsList[0];
			speed = Random.Range(asteroidFragment.speedMin, asteroidFragment.speedMax) / 1000;
			speedRotate = Random.Range(asteroidFragment.speedRotateMin, asteroidFragment.speedRotateMax);
			Health = hp = asteroidFragment.health;
			damage = asteroidFragment.damage;
			pathPointsList = newPathPointsList;
			isEnemy = enemy;
			entityType = EntityType.AsteroidFragment;

			Reward = asteroidFragment.reward;

			_defaultTransformScale = transform.localScale;
			_defaultSpriteRendererColor = spriteRenderer.color;
		}

		public void InitializeMovingOnOrbitPlanet(PlanetType planetType, Vector2 positionPlanet,
			float radiusOrbitSpaceFragments, float angleOnOrbitPlanet, int directionMovingOnOrbitPlanet)
		{
			isInOrbitPlanet = planetType;
			_angleOnOrbitPlanet = angleOnOrbitPlanet;
			_positionPlanet = positionPlanet;
			_radiusOrbitSpaceFragments = radiusOrbitSpaceFragments;
			_directionMovingOnOrbitPlanet = directionMovingOnOrbitPlanet;
		}

		private void Move()
		{
			if (pathPointsList.Count == 0) return;
			if (_currentPointIndex >= pathPointsList.Count)
			{
				if (isInOrbitPlanet != null)
				{
					MoveOnOrbit();
					return;
				}

				Destroy();
				return;
			}

			var position = transform.position;
			position = Vector3.MoveTowards(position, pathPointsList[_currentPointIndex], speed);
			gameObject.transform.position = position;

			if (position.Equals(pathPointsList[_currentPointIndex]))
				_currentPointIndex++;
		}

		private void MoveOnOrbit()
		{
			var stepMove = speed * 15 * _directionMovingOnOrbitPlanet;
			var radians = Mathf.PI * (_angleOnOrbitPlanet + stepMove) / 180.0f;
			var x = _positionPlanet.x + _radiusOrbitSpaceFragments * Mathf.Cos(radians);
			var y = _positionPlanet.y + _radiusOrbitSpaceFragments * Mathf.Sin(radians);
			transform.position = new Vector2(x, y);
			_angleOnOrbitPlanet += stepMove;
			if (_angleOnOrbitPlanet > 360) _angleOnOrbitPlanet = 0;
		}

		internal void SetActiveCollider(bool mode) => collider2D.enabled = mode;

		private void PlayEffectCollision()
		{
			_sequence?.Kill();
			_sequence = DOTween.Sequence();
			transform.localScale = _defaultTransformScale;
			spriteRenderer.color = _defaultSpriteRendererColor;

			_sequence.Append(spriteRenderer.DOColor(new Color(255, 0, 0, 1), 0.05f)
				.SetEase(Ease.InOutExpo)).SetLoops(2, LoopType.Yoyo).SetSpeedBased();

			_sequence.Append(transform.DOScale(
				new Vector2(transform.localScale.x * 0.8f, transform.localScale.y * 0.8f),
				0.05f)).SetLoops(2, LoopType.Yoyo).SetSpeedBased();
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

			PlayEffectCollision();
			hp = Health;
		}

		public void Destroy(ImpactType impactType = ImpactType.LiveTimeLimit, float time = 0)
		{
			_sequence?.Kill();
			if (impactType != ImpactType.Collision && impactType != ImpactType.LiveTimeLimit)
				GetReward(Reward);

			destroyAsteroidFragment?.Invoke(this, time);

			if (!(time > 0)) return;
			spriteRenderer.DOFade(0, time - 0.1f).SetEase(Ease.InOutSine).SetAutoKill();
			transform.DOScale(new Vector3(0, 0, 0), time - 0.1f).SetEase(Ease.InOutSine).SetAutoKill();
		}
	}
}
