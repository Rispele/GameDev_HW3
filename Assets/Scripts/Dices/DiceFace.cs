using UnityEngine;

namespace Dices
{
	public class DiceFace : MonoBehaviour
	{
		[SerializeField] private int faceNumber;
		
		public int FaceNumber => faceNumber;
		public bool IsCollisionEntered { get; private set; }

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Table"))
			{
				return;
			}

			IsCollisionEntered = true;
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Table"))
			{
				return;
			}

			IsCollisionEntered = false;
		}
	}
}