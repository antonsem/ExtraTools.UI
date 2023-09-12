using System;

namespace ExtraTools.UI.Widget
{
	public class WidgetTask
	{
		public readonly float ShowTime;
		public readonly string Text;
		public readonly Action Callback;
		public readonly WidgetBase Base;

		internal WidgetTask(WidgetBase widgetBase, string text, float showTime, Action callback)
		{
			Base = widgetBase;
			Text = text;
			ShowTime = showTime;
			Callback = callback;
		}
	}
}