using System;
using DG.Tweening;
using HpBar;
using Interfaces;
using ScriptableObject.Planets;
using Ui;
using UnityEngine;

namespace Planets
{
	public abstract class BasePlanet : Entity, IDamageable
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private Transform planetTransform;
		[SerializeField] private HpBarController hpBarController;

		public Planet planet;

		[SerializeField] private float hp;
		public float Health { get; set; }

		public float radiusOrbitSpaceFragments;
		public float radiusOrbitSputniks;
		public float radiusOrbitCaptureGravity;
		public float speedRotate;
		public float radiusPlanet;

		private Sequence _sequence;
		private Vector3 _defaultTransformScale;
		private Color _defaultSpriteRendererColor;

		private HpBar.HpBar _hpBar;

		public void Initialize()
		{
			title = name = planet.title;
			entityType = EntityType.Planet;
			radiusOrbitSpaceFragments = planet.radiusOrbitSpaceFragments;
			radiusOrbitSputniks = planet.radiusOrbitSputniks;
			radiusOrbitCaptureGravity = planet.radiusOrbitCaptureGravity;
			radiusPlanet = planet.radiusPlanet;
			speedRotate = planet.speedRotate;
			damage = 1000000f;

			hp = Health = 5;
			//
			// if (_showLine)
			// {
			// 	var drawLine = gameObject.AddComponent<LinesManager>();
			// 	drawLine.DrawCircle(transform, transform.position, radiusOrbitSpaceFragments, 0.02f,
			// 		Color.gray);
			// 	drawLine.DrawCircle(gameObject.transform, transform.position, radiusOrbitSputniks, 0.02f,
			// 		Color.blue);
			// 	drawLine.DrawCircle(transform, transform.position, radiusOrbitCaptureGravity, 0.02f,
			// 		Color.yellow);
			// }

			_defaultTransformScale = transform.localScale;
			_defaultSpriteRendererColor = spriteRenderer.color;

			_hpBar = hpBarController.Initialize(Health, Health, new Vector3(0, -radiusPlanet, 0), 3f);
		}

		private void Update()
		{
			planetTransform.transform.Rotate(Vector3.back * (Time.deltaTime * speedRotate));
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


		private void OnCollisionEnter2D(Collision2D col)
		{
			print($"collision.contactCount: {col.transform.name}");
			if (!col.transform.TryGetComponent(out Entity entity)) return;
			if (entity.isEnemy == isEnemy) return;
			if (entity.entityType == EntityType.Asteroid ||
			    entity.entityType == EntityType.Rocket)
			{
				if (!col.transform.TryGetComponent(out IDamageable damageable)) return;
				damageable.ApplyDamage(damage, ImpactType.Collision);
			}
		}

	
		public void ApplyDamage(float damageValue, ImpactType impactType)
		{
			print($"BasePlanet: ApplyDamage()");
			PlayEffectContactEntity();
			Health -= 1;
			hp = Health;
			_hpBar.SetValue(Health);
			if (Health <= 0) EndGame();
		}

		private void EndGame()
		{
			print($"EndGame()");
		}
	}
}
