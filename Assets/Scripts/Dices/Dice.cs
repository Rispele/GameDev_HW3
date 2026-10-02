using System;
using System.Linq;
using UnityEngine;

namespace Dices
{
	public class Dice : MonoBehaviour
	{
		private static readonly int[] OppositeFaces =
		{
			6,
			5,
			4,
			3,
			2,
			1
		};

		private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");
		private static readonly Color HighlightEmission = Color.darkKhaki;

		private DiceFace[] diceFaces;
		private MaterialPropertyBlock materialPropertyBlock;
		private new Renderer renderer;

		public bool IsChosen { get; private set; }
		public bool AllowChoosing { get; set; }

		public event Action<Dice> Clicked;

		public int? GetCurrentScore()
		{
			var triggeredFace = diceFaces.FirstOrDefault(t => t.IsCollisionEntered);

			return triggeredFace ? GetOppositeFace(triggeredFace.FaceNumber) : null;
		}

		public void Reset()
		{
			IsChosen = false;
			SetHighlighted(IsChosen);
		}

		private void Awake()
		{
			renderer = GetComponent<Renderer>();
			materialPropertyBlock = new MaterialPropertyBlock();
			diceFaces = GetComponentsInChildren<DiceFace>();
		}

		private void OnMouseEnter()
		{
			if (IsChosen)
			{
				return;
			}

			SetHighlighted(true);
		}

		private void OnMouseDown()
		{
			if (!AllowChoosing)
			{
				return;
			}

			IsChosen = !IsChosen;
			SetHighlighted(IsChosen);

			Clicked?.Invoke(this);
		}

		private void OnMouseExit()
		{
			if (IsChosen)
			{
				return;
			}

			SetHighlighted(false);
		}

		private void SetHighlighted(bool on)
		{
			renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetColor(EmissionId, on ? HighlightEmission : Color.black);
			renderer.SetPropertyBlock(materialPropertyBlock);
		}

		private static int GetOppositeFace(int number)
		{
			return OppositeFaces[number - 1];
		}
	}
}