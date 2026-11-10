using System;
using JetBrains.Annotations;

namespace ExtraTools.UI.Dialog
{
	public struct DialogAnswer
	{
		[PublicAPI] public readonly string Text;
		[PublicAPI] public readonly Action Callback;

		public DialogAnswer(string text, Action callback = null)
		{
			Text = text;
			Callback = callback;
		}
	}
}
