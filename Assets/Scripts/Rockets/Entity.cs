using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Rockets
{
	public class Entity : MonoBehaviour
	{
		public bool IsEnemy { get; protected set; }

		public EntityType entityType;
		public float health;
		public string title;
		public float damage;
		private HashSet<Entity> _entitiesAimed = new HashSet<Entity>();

		private void OnCollisionEnter2D(Collision2D col) =>
			col.transform.GetComponent<IDamageable>()?.ApplyDamage(this);

		public Vector2 GetPosition() => transform.position;

	}
}
