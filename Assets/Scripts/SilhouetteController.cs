using System.Collections.Generic;
using UnityEngine;

namespace Architect {
	public class SilhouetteController : MonoBehaviour {

		public enum State {
			Idle,
			Travelling,
			Sleeping
		}

		public enum TravellingState {
			InRoom,
			InLink,
		}

		public RoomNetwork roomnet;
		public float speed = 0.1f;

		private Animator animator;

		[SerializeField] private State state = State.Idle;
		[SerializeField] private TravellingState travellingState = TravellingState.InRoom;
		private List<RoomLink> path;
		private Room currentRoom = null;
		private Room targetRoom = null;
		private Vector3 targetPosition;
		private Traverser currentTraverser;

		private void Start() {
			currentRoom = roomnet.GetRoom(transform.position);
			roomnet.LastLitChangedEvent += LastLitRoomChanged;
			animator = GetComponentInChildren<Animator>();
		}

		private void Update() {
			UpdateState();
		}

		private void OnDrawGizmos() {
			if (targetRoom != null) {
				Gizmos.DrawCube(targetRoom.transform.position, Vector3.one * 0.05f);
			}
		}

		private void LastLitRoomChanged(Room lastLit) {
			//Debug.Log("Lastlit Changed!");
			switch (state) {
				case State.Travelling:
				case State.Idle:
					targetRoom = lastLit;
					break;
			}
		}

		private void UpdateState() {
			Vector3 prevPos = roomnet.WorldToRelativePos(transform.position);
			switch (state) {
				case State.Idle:
					if (!currentRoom.isConnectedToStart) {
						state = State.Sleeping;
						animator.SetBool("isSleeping", true);
					} else if (targetRoom != null && targetRoom != currentRoom) {
						state = State.Travelling;
					}
					break;
				case State.Travelling:
					UpdateTravel();
					break;
				case State.Sleeping:
					if (currentRoom.isConnectedToStart) {
						Room connectedEnd = roomnet.GetLinkedEndRoom();
						if (connectedEnd != null) { //Has direct link to end
							targetRoom = connectedEnd;
						} else {
							targetRoom = roomnet.fallbackRoom;
						}
						state = State.Travelling;
						animator.SetBool("isSleeping", false);
					}
					break;
			}
			Vector3 deltaPos = roomnet.WorldToRelativePos(transform.position) - prevPos;
			animator.SetFloat("Velocity", deltaPos.magnitude / Time.deltaTime);
		}

		private void UpdateTravel() {
			switch (travellingState) {
				case TravellingState.InRoom:
					Room room = roomnet.GetRoom(transform.position);
					if (room != null) {
						currentRoom = room;
						if (!currentRoom.isConnectedToStart) {
							state = State.Sleeping;
							animator.SetBool("isSleeping", true);
							return;
						}
						path = roomnet.FindPath(currentRoom, targetRoom);
						if (path == null) {
							state = State.Idle;
							return;
						}
						currentTraverser = currentRoom.traverser;
						if (targetRoom == currentRoom) {
							currentRoom.traverser.SetTarget(roomnet.WorldToRelativePos(currentRoom.center));
							if (currentTraverser.Traverse(this)) {
								state = State.Idle;
							}
						} else {
							currentRoom.traverser.SetTarget(path[0]);
							if (currentTraverser.Traverse(this)) {

								travellingState = TravellingState.InLink;
							}
						}
					}
					break;

				case TravellingState.InLink:
					currentTraverser = path[0].traverser;
					path[0].traverser.SetTarget(currentRoom);

					if (currentTraverser.Traverse(this)) {
						travellingState = TravellingState.InRoom;
					}
					break;
			}
		}

		public void Recenter() {
			if (currentRoom == null) return;
			transform.position = currentRoom.center;
			if (state == State.Travelling && travellingState == TravellingState.InLink) {
				travellingState = TravellingState.InRoom;
			}
			if (transform.parent != roomnet.transform.parent) {
				transform.SetParent(roomnet.transform.parent, true);
			}
		}

	}
}
