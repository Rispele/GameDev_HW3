using UnityEngine;

namespace Dices
{
	[RequireComponent(typeof(Renderer))]
	public class DiceHighlighter : MonoBehaviour
	{
		private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");
		private static readonly Color NotHighlighted = Color.black;
		private static readonly Color GoingToSelectHighlight = Color.darkKhaki;
		private static readonly Color SelectedHighlight = Color.khaki;

		[SerializeField] private new Renderer renderer;
		private MaterialPropertyBlock materialPropertyBlock;

		private void Awake()
		{
			materialPropertyBlock = new MaterialPropertyBlock();
		}
		
		public void SetHighlighted(bool isSelected, bool isGoingToSelect)
		{
			var color = isSelected
				? SelectedHighlight
				: isGoingToSelect
					? GoingToSelectHighlight
					: NotHighlighted;

			SetHighlighted(color);
		}

		private void SetHighlighted(Color color)
		{
			renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetColor(EmissionId, color);
			renderer.SetPropertyBlock(materialPropertyBlock);
		}
	}
}