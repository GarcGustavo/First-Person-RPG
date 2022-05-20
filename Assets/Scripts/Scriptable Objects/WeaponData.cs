using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

namespace Scriptable_Objects
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Weapon", order = 4)]
	public class WeaponData : ScriptableObject
	{
		public string weaponName;
		public string description;
		public int dmg;
		public Element elementType;
		public enum Element
		{
			Physical,
			Psy,
			Magic,
			Bullet
		}
	}
}