using Dices;
using UnityEngine;

public class HoverDebug : MonoBehaviour
{
	private Camera cam;

	private void Awake()
	{
		cam = GetComponent<Camera>();
		if (cam == null)
		{
			Debug.LogError("Скрипт не на камере!");
		}
	}

	private void Update()
	{
		var ray = cam.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

		if (Physics.Raycast(ray, out var hit, 100f))
		{
			Debug.Log("Попал в: "
			          + hit.collider.gameObject.name
			          + " | слой: "
			          + LayerMask.LayerToName(hit.collider.gameObject.layer)
			          + " | HoverHighlight: "
			          + (hit.collider.GetComponentInParent<Dice>() != null));
		}
		else
		{
			Debug.Log("Луч ни во что не попал");
		}
	}
}