using System.Collections.Generic;
using Base_Classes;
using Scriptable_Objects;
using UnityEngine;

namespace Managers
{
	public class InventoryManager : MonoBehaviour
	{
		private static InventoryManager _instance;
		public WeaponData weapon;
		public List<ItemData> items;
    
		public static InventoryManager GetInstance()
		{
			return _instance;
		}
		void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			} 
			else 
			{
				Destroy(this);
			}

			items = new List<ItemData>();
		}
	}
}