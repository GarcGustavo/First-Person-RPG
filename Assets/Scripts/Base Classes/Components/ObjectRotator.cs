using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Base_Classes
{
	public class ObjectRotator : MonoBehaviour
	{
		private void Update()
		{
			 if (!DOTween.IsTweening(transform))
			 {
			 	transform.DORotate(transform.eulerAngles + new Vector3(0, 1f, 0) * 180, 1f, RotateMode.Fast)
			 		.SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
			 }
		}
	}
}