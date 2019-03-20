namespace Architect {
	public class RoomLink {

		public Room room1;
		public Room room2;

		public Room getOther(Room r) {
			return (r == room1) ? room2 : room1;
		}

	}
}
