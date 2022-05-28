using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Base_Classes
{
	public class ObjectRotator : MonoBehaviour
	{
		public bool rotating = true;
		private void Update()
		{
			if (!rotating)
				return;
			if (!DOTween.IsTweening(transform) && rotating)
			{ 
				transform.DORotate(transform.eulerAngles + new Vector3(0, 1f, 0) * 180, 1f, RotateMode.Fast)
					.SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
			}
		}

		public void StopRotation()
		{
			rotating = false;
			transform.DOKill();
			transform.rotation = Quaternion.identity;
		}
	}
}