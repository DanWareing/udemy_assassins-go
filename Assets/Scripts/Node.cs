﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	Vector2 m_coordinate;
	public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); } }
	List<Node> m_neighbourNodes = new List<Node>();
	public List<Node> NeighbourNodes { get { return m_neighbourNodes; } }
	List<Node> m_linkedNodes = new List<Node>();
	public List<Node> LinkedNodes { get { return m_linkedNodes; } }
	Board m_board;
	public GameObject geometry;
	public float scaleTime = 0.3f;
	public iTween.EaseType easeType = iTween.EaseType.easeInExpo;
	public bool autoRun = false;
	public float delay = 1f;
	bool m_isInitialized = false;
	public GameObject linkPrefab;

	void Awake() {
		m_board = Object.FindObjectOfType<Board>();
		m_coordinate = new Vector2(transform.position.x, transform.position.z);
	}
	void Start () {
		if (geometry != null) {
			geometry.transform.localScale = Vector3.zero;
			if (autoRun) {
				InitNode();
			}
		}

		if (m_board != null) {
			m_neighbourNodes = FindNeighbours(m_board.AllNodes);
		}
	}

	public void ShowGeometry() {
		if (geometry != null) {
			iTween.ScaleTo(geometry, iTween.Hash(
				"time", scaleTime,
				"scale", Vector3.one,
				"easetype", easeType,
				"delay", delay 
			));
		}
	}
	
	public List<Node> FindNeighbours(List<Node> nodes) {
		List<Node> nList = new List<Node>();
		foreach (Vector2 dir in Board.directions) {
			Node foundNeighbour = nodes.Find(
				n => n.Coordinate == Coordinate + dir
			);
			if (foundNeighbour != null && !nList.Contains(foundNeighbour)) {
				nList.Add(foundNeighbour);
			}
		}
		return nList;
	}

	public void InitNode() {
		if (!m_isInitialized) {
			ShowGeometry();
			InitNeighbours();
		}
		m_isInitialized = true;
	}

	void InitNeighbours() {
		StartCoroutine(InitNeighboursRoutine());
	}

	IEnumerator InitNeighboursRoutine() {
		yield return new WaitForSeconds(delay);
		foreach (Node n in m_neighbourNodes) {
			if (!m_linkedNodes.Contains(n)) {
				LinkNode(n);
				n.InitNode();
			}
		}
	}

	void LinkNode(Node targetNode) {
		if (linkPrefab != null) {
			GameObject linkInstance = Instantiate(
				linkPrefab, transform.position, Quaternion.identity
			);
			linkInstance.transform.parent = transform;

			Link link = linkInstance.GetComponent<Link>();
			if (link != null) {
				link.DrawLink(transform.position, targetNode.transform.position);
			}

			if (!m_linkedNodes.Contains(targetNode)) {
				m_linkedNodes.Add(targetNode);
			}
			if (!targetNode.LinkedNodes.Contains(this)) {
				targetNode.LinkedNodes.Add(this);
			}
		}
	}
}
