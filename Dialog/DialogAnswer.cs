using System;
using JetBrains.Annotations;

namespace ExtraTools.UI.Dialog
{
	public struct DialogAnswer
	{
		[PublicAPI] public readonly string Text;
		[PublicAPI] public readonly Action Callback;
		[PublicAPI] public readonly bool IsDefault;

		public DialogAnswer(string text, Action callback = null, bool isDefault = false)
		{
			Text = text;
			Callback = callback;
			IsDefault = isDefault;
		}
	}
}
