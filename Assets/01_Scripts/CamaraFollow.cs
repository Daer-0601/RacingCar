using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
	public Transform target;
	public Vector3 offset = new Vector3(0, 0, -10);
	public float smoothness = 0.1f;

	void LateUpdate()
	{
		if (target == null) return;

		Vector3 targetPosition = target.position + offset;
		transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness);
	}
}