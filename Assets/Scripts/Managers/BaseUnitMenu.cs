using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnitMenu : MonoBehaviour {
	#region General Attributes
	private Base managedBase;
	internal Base ManagedBase { get { return managedBase; } set { managedBase = value; } }
	private UILabelTextAppender baseTitle;
	private GameObject unitListUI;
	private Button closeButtonUI;
	public GameObject unitBarTemplate;
	#endregion


	private void Awake() {
		baseTitle = transform.GetChild(1).GetChild(1).GetComponent<UILabelTextAppender>();
		unitListUI = transform.GetChild(2).gameObject;
		closeButtonUI = transform.GetChild(1).GetChild(4).GetComponent<Button>();

		closeButtonUI.onClick.RemoveAllListeners();
		closeButtonUI.onClick.AddListener(() => {
			for (int i = 0; i < unitListUI.transform.childCount; i++) {
				Destroy(unitListUI.transform.GetChild(i).gameObject);
			}
			managedBase = null;
			gameObject.SetActive(false);
		});
	}

	private void OnEnable() {
		baseTitle.UpdateText(managedBase.name);
		AddUnits();
	}

	#region Modifying Units in Base
	/// <summary>
	/// Method creates UI elements of units inside a base.
	/// </summary>
	/// <param name="equipment"></param>
	private void AddUnits() {
		foreach (Unit unit in ManagedBase.unitList) {
			GameObject unitBar = Instantiate(unitBarTemplate, unitListUI.transform);

			unitBar.transform.GetChild(0).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"[{unit,-3}]";

			Button spawnButton = unitBar.transform.GetChild(1).GetComponent<Button>();
			Button sellButton = unitBar.transform.GetChild(2).GetComponent<Button>();
			Button deleteButton = unitBar.transform.GetChild(3).GetComponent<Button>();

			if (!ApplicationController.isAdmin) Destroy(deleteButton.gameObject); //Hide buttons if not admin.

			spawnButton.onClick.RemoveAllListeners();
			deleteButton.onClick.RemoveAllListeners();
			sellButton.onClick.RemoveAllListeners();

			spawnButton.onClick.AddListener(() => {
				unit.SetVisibility(true);
				managedBase.unitList.Remove(unit);
				Destroy(unitBar);
			});

			deleteButton.onClick.AddListener(() => {
				UnitManager.Instance.Despawn(unit.gameObject, false);
				managedBase.unitList.Remove(unit);
				Destroy(unitBar);
			});

			sellButton.onClick.AddListener(() => {
				UnitManager.Instance.Despawn(unit.gameObject, true);
				managedBase.unitList.Remove(unit);
				Destroy(unitBar);
			});
		}
	}




	#endregion
}