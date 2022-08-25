using DG.Tweening;
using Interfaces;
using ScriptableObject.Planets;
using UnityEngine;

namespace Planets
{
	public abstract class BasePlanet : Entity, IDamageable
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		public Planet planet;
		public float radiusOrbitSpaceFragments;
		public float radiusOrbitSputniks;
		public float radiusOrbitCaptureGravity;
		public float speedRotate;
		public float radiusPlanet;

		private Sequence _sequence;
		private Vector3 _defaultTransformScale;
		private Color _defaultSpriteRendererColor;

		public void Initialize()
		{
			title = gameObject.name = planet.title;
			entityType = EntityType.Planet;
			radiusOrbitSpaceFragments = planet.radiusOrbitSpaceFragments;
			radiusOrbitSputniks = planet.radiusOrbitSputniks;
			radiusOrbitCaptureGravity = planet.radiusOrbitCaptureGravity;
			radiusPlanet = planet.radiusPlanet;
			speedRotate = planet.speedRotate;
			damage = 1000000f;

			var drawLine = gameObject.AddComponent<LinesManager>();
			drawLine.DrawCircle(gameObject.transform, gameObject.transform.position, radiusOrbitSpaceFragments, 0.02f,
				Color.gray);
			drawLine.DrawCircle(gameObject.transform, gameObject.transform.position, radiusOrbitSputniks, 0.02f,
				Color.blue);
			drawLine.DrawCircle(gameObject.transform, gameObject.transform.position, radiusOrbitCaptureGravity, 0.02f,
				Color.yellow);

			_defaultTransformScale = transform.localScale;
			_defaultSpriteRendererColor = spriteRenderer.color;
		}

		private void Update()
		{
			gameObject.transform.Rotate(Vector3.back * (Time.deltaTime * speedRotate));
		}

		private void PlayEffectContactEntity()
		{
			_sequence?.Kill();
			transform.localScale = _defaultTransformScale;
			spriteRenderer.color = _defaultSpriteRendererColor;

			_sequence = DOTween.Sequence();
			_sequence.Append(spriteRenderer.DOColor(new Color(255, 0, 0, 1), 0.05f)
				.SetEase(Ease.InOutExpo)).SetAutoKill(true);
			_sequence.Append(spriteRenderer.DOColor(_defaultSpriteRendererColor, 0.05f)
				.SetEase(Ease.InOutExpo)).SetAutoKill(true);
			_sequence.Append(transform.DOShakeScale(0.5f, new Vector2(0.05f, 0.05f))).SetAutoKill(true);

		}

		public void ApplyDamage(Entity entity)
		{
			if (entity.IsEnemy == IsEnemy) return;

			if (entity.entityType == EntityType.Asteroid ||
			    entity.entityType == EntityType.AsteroidFragment ||
			    entity.entityType == EntityType.Rocket)
			{
				print($"\"{title}\" take damage ({entity.damage}) from \"{entity.title}\"");

				PlayEffectContactEntity();
			}
		}
	}
}
