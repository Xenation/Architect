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

		[SerializeField] private State state = State.Idle;
		[SerializeField] private TravellingState travellingState = TravellingState.InRoom;
		private List<RoomLink> path;
		private Room currentRoom = null;
		private Room targetRoom = null;
		private Vector3 targetPosition;

		private void Start() {
			currentRoom = roomnet.GetRoomHover(transform.position);
			roomnet.LastLitChangedEvent += LastLitRoomChanged;
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
			switch (state) {
				case State.Idle:
					if (!currentRoom.isConnectedToStart) {
						state = State.Sleeping;
					} else if (targetRoom != null && targetRoom != currentRoom) {
						state = State.Travelling;
					}
					break;
				case State.Travelling:
					UpdateTravel();
					break;
				case State.Sleeping:
					RoomLink linkToLit = currentRoom.GetClosestOpenLinkToConnected();
					if (linkToLit != null) {
						targetRoom = linkToLit.GetOther(currentRoom);
						state = State.Travelling;
					}
					break;
			}
		}

		private void UpdateTravel() {
			Vector3 toTarget = Vector3.zero;

			switch (travellingState) {
				case TravellingState.InRoom:
					Room room = roomnet.GetRoomHover(transform.position);
					if (room != null) {
						currentRoom = room;
					}

					//targetRoom = roomnet.GetRoomHover(debugTarget.position);

					if (targetRoom != null && currentRoom != null) {
						path = roomnet.FindPath(currentRoom, targetRoom);
					}

					if (path != null) {
						if (targetRoom == currentRoom) { // In Destination Room
							targetPosition = currentRoom.transform.position;
						} else {
							targetPosition = path[0].GetEntry(currentRoom);
						}
						toTarget = targetPosition - transform.position;
						if (toTarget.magnitude < 0.01f) {
							if (targetRoom == currentRoom) {
								state = State.Idle;
							} else {
								path[0].NotifyCharacterEnter();
								travellingState = TravellingState.InLink;
							}
						}
					}
					break;

				case TravellingState.InLink:
					targetPosition = path[0].GetEntry(path[0].GetOther(currentRoom));
					toTarget = targetPosition - transform.position;

					if (toTarget.magnitude < 0.01f) {
						path[0].NotifyCharacterExit(); // TODO left here
						travellingState = TravellingState.InRoom;
					}
					break;
			}

			Vector3 direction = toTarget.normalized;
			transform.position += direction * speed * Time.deltaTime;
			transform.rotation = Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.forward, direction, Vector3.up), 0f);
		}

	}
}
