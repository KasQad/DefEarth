using UnityEngine;

namespace HpBar
{
	public class HpBarController : MonoBehaviour
	{
		private HpBarSimple _hpBarSimple;
		private readonly Vector3 _offsetHpBar = new Vector3(0, -0.8f, 0);

		internal HpBarSimple Initialize(float valueCurrent, float valueMax, Vector3 offsetHpBar = default,
			float scale = 1f)
		{
			_hpBarSimple = Instantiate(Resources.Load<HpBarSimple>("Prefabs/Ui/HpBarSimple"), transform);
			_hpBarSimple.transform.position += offsetHpBar == default ? _offsetHpBar : offsetHpBar;
			_hpBarSimple.transform.localScale *= scale;
			_hpBarSimple.SetValue(valueCurrent, valueMax);
			return _hpBarSimple;
		}
	}
}
