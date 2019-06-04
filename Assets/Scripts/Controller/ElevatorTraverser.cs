using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Architect {
	public class ElevatorTraverser : LinkTraverser {

		private enum ElevatorState {
			Up,
			Down,
			Between
		}

		private Elevator elevator;
		private Transform elevatorCenter;
		private LinearMapping elevatorMapping;
		private ElevatorState elevatorState;

		public ElevatorTraverser(RoomLink link, Elevator elevator, LinearMapping mapping, RoomNetwork roomnet) {
			this.link = link;
			this.elevator = elevator;
			elevatorCenter = elevator.movingRoot.Find("Center");
			elevatorMapping = mapping;
			this.roomnet = roomnet;
		}

		public override bool Traverse(SilhouetteController controller) {
			// Elevator State update
			if (elevatorMapping.value < 0.05f) {
				elevatorState = ElevatorState.Down;
			} else if (elevatorMapping.value > 0.95f) {
				elevatorState = ElevatorState.Up;
			} else {
				elevatorState = ElevatorState.Between;
			}

			// Move Character
			Vector3 targetOppositeEntry = link.GetEntry(link.GetOther(targetRoom));
			bool characterInside = controller.transform.parent == elevator.movingRoot;
			bool characterTowardsTargetLevel = Vector3.Distance(controller.transform.position, roomnet.RelativeToWorldPos(target)) < Vector3.Distance(controller.transform.position, roomnet.RelativeToWorldPos(targetOppositeEntry));
			bool elevatorTowardsTargetLevel = Vector3.Distance(elevator.movingRoot.position, roomnet.RelativeToWorldPos(target)) < Vector3.Distance(elevator.movingRoot.position, roomnet.RelativeToWorldPos(targetOppositeEntry));
			bool elevatorAtTargetLevel = elevatorTowardsTargetLevel && (elevatorState == ElevatorState.Up || elevatorState == ElevatorState.Down);
			bool elevatorAtTargetOppositeLevel = !elevatorTowardsTargetLevel && (elevatorState == ElevatorState.Up || elevatorState == ElevatorState.Down);
			Vector3 toWaypoint;
			Vector3 direction;
			if (characterInside) {
				if (characterTowardsTargetLevel && elevatorAtTargetLevel) {
					//Debug.Log("Inside - To Target");
					toWaypoint = elevator.movingRoot.InverseTransformPoint(roomnet.RelativeToWorldPos(target)) - controller.transform.localPosition;
				} else {
					//Debug.Log("Inside - To Elevator Center");
					toWaypoint = elevatorCenter.localPosition - controller.transform.localPosition;
				}
			} else {
				if (characterTowardsTargetLevel) {
					//Debug.Log("Outside - To Target");
					toWaypoint = target - roomnet.WorldToRelativePos(controller.transform.position);
				} else if (elevatorAtTargetOppositeLevel) {
					//Debug.Log("Outside - To Elevator Center");
					toWaypoint = roomnet.WorldToRelativePos(elevatorCenter.position) - roomnet.WorldToRelativePos(controller.transform.position);
				} else { // Wait for elevator
					//Debug.Log("Outside - Waiting");
					toWaypoint = Vector3.zero;
				}
			}
			if (toWaypoint != Vector3.zero) {
				direction = toWaypoint.normalized;
				if (toWaypoint.magnitude > 0.01f) {
					controller.transform.localPosition += direction * controller.speed * Time.deltaTime;
					controller.transform.localRotation = Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.forward, direction.Flat3().normalized, Vector3.up), 0f);
				}
			}

			// Check at target or entering elevator
			if (controller.transform.parent != elevator.movingRoot && elevator.insideBox.IsInside(controller.transform.position)) {
				controller.transform.SetParent(elevator.movingRoot, true);
			} else if (controller.transform.parent == elevator.movingRoot && !elevator.insideBox.IsInside(controller.transform.position)) {
				controller.transform.SetParent(roomnet.transform.parent, true); // TODO ugly and unsure
			}
			if (Vector3.Distance(target, roomnet.WorldToRelativePos(controller.transform.position)) < 0.01f) {
				return true;
			}
			return false;
		}

	}
}
