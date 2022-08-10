using Interfaces;
using UnityEngine;

namespace Rockets
{
	public class Entity : MonoBehaviour
	{
		public bool IsEnemy { get; protected set; }

		public EntityType entityType;  
		public string title;
		public float damage;
		public Entity target;

		private void OnCollisionEnter2D(Collision2D col)
		{
			col.transform.GetComponent<IDamageable>()?.ApplyDamage(this);
		}
		
	}
}
