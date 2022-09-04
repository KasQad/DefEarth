using UnityEngine;

namespace HpBar
{
	public class HpBarController : MonoBehaviour
	{
		private HpBar _hpBar;
		private readonly Vector3 _offsetHpBar = new Vector3(0, -0.8f, 0);

		internal HpBar Initialize(float valueCurrent, float valueMax, Vector3 offsetHpBar = default,
			float scale = 1f)
		{
			_hpBar = Instantiate(Resources.Load<HpBar>("Prefabs/Ui/HpBar"), transform);
			_hpBar.transform.position += offsetHpBar == default ? _offsetHpBar : offsetHpBar;
			_hpBar.transform.localScale *= scale;
			_hpBar.SetValue(valueCurrent, valueMax);
			return _hpBar;
		}
	}
}
