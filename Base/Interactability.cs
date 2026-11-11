using UnityEngine;

namespace ExtraTools.UI.Base
{
	internal static class Interactability
	{
		internal static void Set(Component view, ref CanvasGroup canvasGroup, bool interactable)
		{
			if (!canvasGroup)
			{
				if (!view)
				{
					return;
				}

				if (!view.TryGetComponent(out canvasGroup))
				{
					canvasGroup = view.gameObject.AddComponent<CanvasGroup>();

					// The manager sets every view itself, so a dialog nested under a covered screen must stay usable.
					canvasGroup.ignoreParentGroups = true;
				}
			}

			canvasGroup.interactable = interactable;
		}
	}
}
