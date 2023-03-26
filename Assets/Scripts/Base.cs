using TMPro;
using UnityEngine;

namespace Assets.Scripts {
	public class Base : MonoBehaviour {
		internal BaseType baseType;
		internal bool enemySide = false;
		private MeshRenderer main;
		private TextMeshProUGUI identification;

		public void Initiate(string identification, Vector3 position, BaseType baseType) {
			transform.position = new Vector3(position.x, position.y, position.z);
			this.baseType = baseType;

			main = transform.Find("Main").GetComponent<MeshRenderer>();
			main.material.mainTexture = UnitManager.Instance.GetBaseTexture(baseType);

			this.identification = transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>();
			this.identification.text = identification;
		}

		private Vector3 offset;

		private void OnMouseDown() {
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = Camera.main.nearClipPlane;
			offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
		}

		private void OnMouseDrag() {
			//Movement allowed only for admins.
			if (GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin) {
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.z = Camera.main.nearClipPlane;
				transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
			}
		}

		internal void ChangeAffiliation(bool enemy) {
			this.enemySide = enemy;
		}

		internal void ChangeType(BaseType type) {
			baseType = type;
			main.material.mainTexture = UnitManager.Instance.GetBaseTexture(baseType);
		}

		internal void ChangeIdentification(string identification) {
			this.identification.text = identification;
		}
	}
}