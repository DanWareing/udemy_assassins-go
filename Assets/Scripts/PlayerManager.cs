﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerDeath))]
public class PlayerManager : TurnManager {

	public PlayerMover playerMover;
	public PlayerInput playerInput;
	Board m_board;
	public UnityEvent deathEvent;

	protected override void Awake() {
		base.Awake();
		playerMover = GetComponent<PlayerMover>();
		playerInput = GetComponent<PlayerInput>();
		playerInput.InputEnabled = true;
		m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
	}

	void Update () {

		if (playerMover.isMoving || m_gameManager.CurrentTurn != Turn.Player) {
			return;
		} 
		playerInput.GetKeyInput();

		if (playerInput.V == 0) {
			if (playerInput.H < 0) {
				playerMover.MoveLeft();
			} else if (playerInput.H > 0) {
				playerMover.MoveRight();
			}
		} else if (playerInput.H == 0) {
			if (playerInput.V < 0) {
				playerMover.MoveBackward();
			} else if (playerInput.V > 0) {
				playerMover.MoveForward();
			}
		}
	}

	public void Die() {
		if (deathEvent != null) {
			deathEvent.Invoke();
		}
	}

	void CaptureEnemies() {
		if (m_board != null) {
			List<EnemyManager> enemies = m_board.FindEnemiesAt(m_board.PlayerNode);
			Debug.Log(enemies.Count);
			if (enemies.Count != 0) {
				foreach (EnemyManager enemy in enemies) {
					if (enemy != null) enemy.Die();
				}
			}
		}
	}

	public override void FinishTurn() {
		CaptureEnemies();
		base.FinishTurn();
	}
}
