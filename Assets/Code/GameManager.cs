﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActualPlayer
{
	public int id;
	public Vector3 original_startposition;
	public int deaths = 0;
	public Player m_player;
}

public class GameManager : MonoBehaviour
{
	[SerializeField]
	Player m_playerPrefab;

	[SerializeField]
	float kill_height = -3f;

	ActualPlayer m_actualPlayer1;
	ActualPlayer m_actualPlayer2;
	InputManager m_input;
	CameraMovement m_cameraMovement;

	float prevDist = 0f;
	float distDelta = 0f;
	public float m_slowmoDuration= 1f;
	public float m_slowmoCooldown = 3f;
	bool m_inSlowmo = false;

	void Start ()
	{
		m_cameraMovement = Camera.main.GetComponent<CameraMovement>();
		m_input = Game.Instance.Input;

		m_actualPlayer1 = new ActualPlayer (){id=0, original_startposition=Vector3.left * 2.5f + Vector3.up * 1f};
		m_actualPlayer2 = new ActualPlayer (){id=1, original_startposition=Vector3.right * 3f + Vector3.up * 1f};

		SpawnPlayer(m_actualPlayer1, Vector3.zero);
		SpawnPlayer(m_actualPlayer2, Vector3.zero);

//		Debug.Log(m_input.GetController(0).GetJoystick(0).x);
	}

	void SpawnPlayer (ActualPlayer actualPlayer, Vector3 startposition_offset) {
		actualPlayer.m_player = (Player)Instantiate(m_playerPrefab, actualPlayer.original_startposition+startposition_offset, Quaternion.identity);
		actualPlayer.m_player.Init(m_input.GetController(actualPlayer.id));
		m_cameraMovement.AddTarget(actualPlayer.m_player.gameObject);
	}
	
	void Update ()
	{
		if (m_actualPlayer1.m_player != null && m_actualPlayer1.m_player.transform.position.y < kill_height)
		{
			m_actualPlayer1.deaths += 1;  // or is it better with m_actualPlayer2.score += 1 ?
			Destroy(m_actualPlayer1.m_player.gameObject);
			SpawnPlayer(m_actualPlayer1, Vector3.up * 3f);
		}
		if (m_actualPlayer2.m_player != null && m_actualPlayer2.m_player.transform.position.y < kill_height)
		{
			m_actualPlayer2.deaths += 1;
			Destroy(m_actualPlayer2.m_player.gameObject);
			SpawnPlayer(m_actualPlayer2, Vector3.up * 3f);
		}

		float dist1 = m_actualPlayer1.m_player.ClosestDistanceToHit(m_actualPlayer2.m_player);
		float dist2 = m_actualPlayer1.m_player.ClosestDistanceToHit(m_actualPlayer2.m_player);
		float dist = Mathf.Min(dist1, dist2);
		//Debug.Log(dist);
		if (dist < 0.6f && dist < prevDist && !m_inSlowmo)
		{
			StartCoroutine(Slowmo());
		}

		distDelta = dist - prevDist;
		prevDist = dist;

		if (Input.GetKeyDown(KeyCode.T))
		{
			m_cameraMovement.m_action = !m_cameraMovement.m_action;
		}
	}

	IEnumerator Slowmo()
	{
		m_inSlowmo = true;
		m_cameraMovement.m_action = true;

		//	while (distDelta < 0f)
		//	yield return null;

		float pos = Time.unscaledTime + m_slowmoDuration;
		while (Time.unscaledTime < pos)
		{
			yield return null;
		}

		m_cameraMovement.m_action = false;
		yield return new WaitForSeconds(m_slowmoCooldown);
		m_inSlowmo = false;
	}
}
