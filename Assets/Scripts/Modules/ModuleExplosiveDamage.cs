using UnityEngine;

namespace Modules
{
	public class ModuleExplosiveDamage : BaseModule
	{
		private void OnCollisionEnter2D(Collision2D col)
		{
			///print($"ModuleExplosiveDamage : OnCollisionEnter2D(): {col.transform.name}");
		}
	}
}
