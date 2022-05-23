using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Base_Classes
{
	public class Tentacle : MonoBehaviour
	{
		public int _length;
		public LineRenderer _lineRenderer;
		public Vector3[] _segmentPoses;
		public Transform _targetDir;
		public float _targetDistance;
		public float _smoothSpeed;
		public float _trailSpeed;
		public float _wiggleSpeed;
		public float _wiggleMagnitude;
		public Transform _wiggleDir;
		
		[SerializeField] private GridUnit _unit;
		private Vector3[] _segmentV;
		private void Awake()
		{
			_lineRenderer.positionCount = _length;
			_segmentPoses = new Vector3[_length];
			_segmentV = new Vector3[_length];
		}

		private void Update()
		{
			MoveTentacles();
		}

		private void MoveTentacles()
		{
			_wiggleDir.localRotation = Quaternion.Euler(0, 0,
				Mathf.Sin(Time.time * _wiggleSpeed) * _wiggleMagnitude);
			_segmentPoses[0] = _unit.transform.position;
			for (int i = 1; i < _segmentPoses.Length; i++)
			{
				//Trail reaches parent head
				// _segmentPoses[i] = Vector3.SmoothDamp(_segmentPoses[i],
				// 	_segmentPoses[i-1] + _targetDir.right * _targetDistance,
				// 	ref _segmentV[i], _smoothSpeed + i / _trailSpeed);
				
				//Trail is static when object stops moving (retains mass)
				var target_pos = _segmentPoses[i - 1] 
				                 + (_segmentPoses[i] - _segmentPoses[i - 1]).normalized * _targetDistance;
				
				_segmentPoses[i] = Vector3.SmoothDamp(_segmentPoses[i],
					target_pos,ref _segmentV[i],_smoothSpeed + i / _trailSpeed);
				
				
			}
			_lineRenderer.SetPositions(_segmentPoses);
		}
	}
}