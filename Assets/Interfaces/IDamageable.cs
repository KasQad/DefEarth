namespace Interfaces
{
	public interface IDamageable
	{
		public float Health { get; set; }

		public void ApplyDamage(float damageValue, ImpactType impactType);
	}
}
