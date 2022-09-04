using UnityEngine;

namespace Modules
{
	public class BaseModule : Entity, IModular
	{
		private bool _onStarted;
		[SerializeField] private ModuleType _moduleType;
		
		public void Initialize(ModuleType moduleType, bool newIsEnemy)
		{
			print($"BaseModule Initialize: {moduleType.ToString()}");
			_moduleType = moduleType;
		}
	
		public void Run()
		{
			_onStarted = true;
			print($"_moduleType: {_moduleType.ToString()} is Started");
		}

		public void Stop()
		{
			_onStarted = false;
			print($"_moduleType: {_moduleType.ToString()} is Stopped");
		}

		public virtual void Use()
		{
			if (!_onStarted) return;
			print($"_moduleType: {_moduleType.ToString()} is Using");
		}
	}
}
