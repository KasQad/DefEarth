using Types;
using UnityEngine;

namespace Modules
{
	public class BaseModule : Entity, IModular
	{
		protected bool isRunning;
		[SerializeField] private ModuleType _moduleType;

		protected internal void Initialize(ModuleType moduleType)
		{
			print($"BaseModule Initialize: {moduleType.ToString()}");
			_moduleType = moduleType;
		}

		public void Run()
		{
			isRunning = true;
			print($"_moduleType: {_moduleType.ToString()} is Started");
		}

		public void Stop()
		{
			isRunning = false;
			print($"_moduleType: {_moduleType.ToString()} is Stopped");
		}
	}

}
