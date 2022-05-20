using System;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Scriptable_Objects
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skill", order = 3)]
	public class SkillData : ScriptableObject
	{
		public GameObject display;
		public ParticleSystem vfx;
		public AudioClip sfx;
		
		public string skillName;
		public string description;
		
		public int dmg;
		public int mp;
		public int range;
		public element elementType;
		public enum element
		{
			Physical,
			Psy,
			Magic,
			Bullet
		}
	}
}