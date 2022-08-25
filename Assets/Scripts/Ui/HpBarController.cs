using System;
using UnityEngine;

namespace Ui
{
	public class HpBarController : MonoBehaviour
	{
		private HpBar _hpBar;

		public void Initialize(Entity entity, ref Action<float> changeHp, float value, float maxValues)
		{
			_hpBar = Instantiate(Resources.Load<HpBar>("Prefabs/Ui/HpBar"), entity.transform);
			_hpBar.Initialize(ref changeHp, value, maxValues);
		}
	}
}
