using UnityEngine;

internal interface IMovable {
	int ID { get; set; }
	bool SideB { get; set; }
	bool isGhost { get; set; }
	Vector3 StartPosition { get; set; }
}