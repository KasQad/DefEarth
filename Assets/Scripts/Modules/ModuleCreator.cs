using System.Collections.Generic;
using UnityEngine;

namespace Modules
{
	public class ModuleCreator : MonoBehaviour
	{

		private readonly Dictionary<ModuleType, BaseModule> _prefabModulesList =
			new Dictionary<ModuleType, BaseModule>();

		private void Awake()
		{
			_prefabModulesList.Add(ModuleType.LaserDrill,
				Resources.Load<BaseModule>("Prefabs/Modules/LaserDrill"));
			
			_prefabModulesList.Add(ModuleType.LaserGun,
				Resources.Load<BaseModule>("Prefabs/Modules/LaserGun"));
			
			_prefabModulesList.Add(ModuleType.ExplosiveDamage,
				Resources.Load<BaseModule>("Prefabs/Modules/ModuleExplosiveDamage"));
		}

		public BaseModule CreateModule(ModuleType moduleType, Transform transformParent)
		{
			if (!_prefabModulesList.TryGetValue(moduleType, out var baseModule)) return null;
			var module = Instantiate(baseModule, transform);
			module.Initialize(moduleType, transformParent);
			return module;
		}

	}
}
