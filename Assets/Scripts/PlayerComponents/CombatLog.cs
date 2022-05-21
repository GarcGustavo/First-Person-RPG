using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlayerComponents
{
	public class CombatLog : MonoBehaviour
	{
		//[SerializeField] private List<TMP_Text> _logList;
		[SerializeField] private TMP_Text _log;
		private UIManager _uiManager;

		private void Start()
		{
			_uiManager = UIManager.GetInstance();
			_uiManager.LogAction.AddListener(AddLog);
			_log.text = "Floor: 1";
		}

		private void AddLog(string next_log)
		{
			//var new_log = Instantiate(_log, transform);
			_log.text += "\n" + next_log;
			//_logList.Add(newLog);
		}
	}
}