using System.Collections.Generic;
using Asteroids;
using Interfaces;
using Rockets;
using ScriptableObject.Planets;
using UnityEngine;

namespace Planets
{
	public abstract class BasePlanet : Entity, IDamageable
	{
		public Planet planet;
		public float orbitSpaceFragments;
		public float orbitSputniks;
		public float orbitCaptureGravity;
		public float speedRotate;
		
		public void Initialize()
		{
			orbitSpaceFragments = planet.orbitSpaceFragments;
			orbitSputniks = planet.orbitSputniks;
			orbitCaptureGravity = planet.orbitCaptureGravity;
			
			var drawLine = gameObject.AddComponent<LinesManager>();
			drawLine.DrawCircle(gameObject.transform, orbitSpaceFragments, 0.02f, Color.gray);
			drawLine.DrawCircle(gameObject.transform, orbitSputniks, 0.02f, Color.blue);
			drawLine.DrawCircle(gameObject.transform, orbitCaptureGravity, 0.02f, Color.yellow);
		}

		
		private void Update()
		{
			gameObject.transform.Rotate(Vector3.back * (Time.deltaTime * speedRotate));
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			var entity = col.transform.GetComponent<Entity>();
			// print($"entity.GetType(): {entity.GetType()}");

			if (entity.Enemy == Enemy)
			{
				print($"OnCollisionEnter2D {title}: friendly object {col.transform.name}");
				return;
			}

			col.transform.GetComponent<IDamageable>()?.ApplyDamage(10000000);
		}

		public void ApplyDamage(float applyDamage)
		{
		}
	}
}
