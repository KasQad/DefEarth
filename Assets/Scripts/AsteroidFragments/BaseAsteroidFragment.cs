using System.Collections.Generic;
using CustomTimers;
using DG.Tweening;
using Interfaces;
using ScriptableObject.AsteroidFragments;
using Types;
using UnityEngine;

namespace AsteroidFragments
{
	public class BaseAsteroidFragment : Entity, IDamageable, IRewardable
	{
		public AsteroidFragment asteroidFragment;
		// [SerializeField] private new Collider2D collider2D;
		[SerializeField] private SpriteRenderer spriteRenderer;

		private CustomTimer _customTimer;

		[SerializeField] private float hp;

		public float Health { get; set; }
		public long Reward { get; set; }

		public float speed;
		public float speedRotate;
		[SerializeField] private bool _isDestroyed;
		public AsteroidFragmentType asteroidFragmentType;

		private List<Vector2> _pathPointsList = new List<Vector2>();
		private int _currentPointIndex;

		public PlanetType? isInOrbitPlanet;
		private float _angleOnOrbitPlanet;
		private float _radiusOrbitSpaceFragments;
		private Vector2 _positionPlanet;
		private int _directionMovingOnOrbitPlanet;

		private Sequence _sequence;
		private Vector3 _defaultTransformScale;
		private Color _defaultSpriteRendererColor;

		[SerializeField] private bool _firstInit = true;

		public PlanetType? CheckIsInOrbitPlanet() => isInOrbitPlanet;


		private void Awake()
		{
			_customTimer = gameObject.AddComponent<CustomTimer>();
		}

		private void FixedUpdate()
		{
			gameObject.transform.Rotate(Vector3.back * speedRotate);
			Move();
		}

		public void Initialize(List<Vector2> pathPointsList, bool enemy = true)
		{
			_pathPointsList.Clear();
			isInOrbitPlanet = null;
			_isDestroyed = false;
			// collider2D.enabled = false;
			_currentPointIndex = 0;


			//title = gameObject.name = $"{asteroidFragment.title} #{Random.Range(0, 100000)}";
			transform.position = pathPointsList[0];
			speed = Random.Range(asteroidFragment.speedMin, asteroidFragment.speedMax) / 1000;
			speedRotate = Random.Range(asteroidFragment.speedRotateMin, asteroidFragment.speedRotateMax);
			Health = hp = asteroidFragment.health;
			damage = asteroidFragment.damage;

			_pathPointsList = pathPointsList;
			isEnemy = enemy;
			entityType = EntityType.AsteroidFragment;

			Reward = asteroidFragment.reward;

			if (_firstInit)
			{
				_defaultTransformScale = transform.localScale;
				_defaultSpriteRendererColor = spriteRenderer.color;
			}
			else
			{
				{
					transform.localScale = _defaultTransformScale;
					spriteRenderer.color = _defaultSpriteRendererColor;
				}
			}

			_firstInit = false;
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

		public List<Vector2> GetPathPointsList() => _pathPointsList;
		
		private void Move()
		{
			if (_pathPointsList.Count == 0) return;

			if (_currentPointIndex >= _pathPointsList.Count)
			{
				if (isInOrbitPlanet != null)
				{
					MoveOnOrbit();
					return;
				}

				DestroyAsteroidFragment();
				return;
			}

			var position = transform.position;
			position = Vector3.MoveTowards(position, _pathPointsList[_currentPointIndex], speed);
			gameObject.transform.position = position;

			if (position.Equals(_pathPointsList[_currentPointIndex]))
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

		// internal void SetActiveCollider(bool mode) => collider2D.enabled = mode;

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
				DestroyAsteroidFragment(impactType);
				return;
			}

			PlayEffectCollision();
			hp = Health;
		}

		public void DestroyAsteroidFragment(ImpactType impactType = ImpactType.LiveTimeLimit, float time = 0)
		{
			_sequence?.Kill();
			if (impactType != ImpactType.Collision && impactType != ImpactType.LiveTimeLimit)
				GetReward(Reward);


			if (time > 0)
			{
				_customTimer.AddCallBack(() => { DestroyByTimer(time); });
				_customTimer.InitTimer(time);
				_customTimer.Run();

				spriteRenderer.DOFade(0, time - 0.1f).SetEase(Ease.InOutSine).SetAutoKill();
				transform.DOScale(new Vector3(0, 0, 0), time - 0.1f).SetEase(Ease.InOutSine).SetAutoKill();
			}
			else AsteroidFragmentsSpawner.Instance.DestroyAsteroidFragment(this, time);
		}

		private void DestroyByTimer(float time)
		{
			AsteroidFragmentsSpawner.Instance.DestroyAsteroidFragment(this, time);
			_customTimer.Stop();
			_customTimer.ClearCallBacks();
		}
	}
}
