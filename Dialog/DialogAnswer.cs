using System;

namespace ExtraTools.UI.Dialog
{
	public struct DialogAnswer
	{
		public readonly string Text;
		public readonly Action Callback;

		public DialogAnswer(string text, Action callback = null)
		{
			Text = text;
			Callback = callback;
		}
	}
}