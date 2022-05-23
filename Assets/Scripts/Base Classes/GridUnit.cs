using System.Collections;
using DG.Tweening;
using Scriptable_Objects;
using UnityEngine;

namespace Base_Classes
{
	public abstract class GridUnit : MonoBehaviour
	{
		//Initial Values
		//[SerializeField] private UnitData _unitData;

		//Status Info
		public string _unitName = "unit";
		public string _unitDesc = "description";
		public SpriteRenderer _spriteRenderer;

		//Movement
		public GameManager.Direction _currentDirection = GameManager.Direction.North;
		public Vector3Int _initialCell;
		public Vector3Int _currentCell;
		public Vector3 _centerOffset;
		public abstract void InitializeUnit(GridCell cell);
	}
}