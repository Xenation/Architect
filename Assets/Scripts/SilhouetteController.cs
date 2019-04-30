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
		private Traverser currentTraverser;

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
			switch (travellingState) {
				case TravellingState.InRoom:
					Room room = roomnet.GetRoomHover(transform.position);
					if (room != null) {
						currentRoom = room;
						path = roomnet.FindPath(currentRoom, targetRoom);
						currentTraverser = currentRoom.traverser;
						if (targetRoom == currentRoom) {
							currentRoom.traverser.SetTarget(roomnet.WorldToRelativePos(currentRoom.transform.position));
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

	}
}
