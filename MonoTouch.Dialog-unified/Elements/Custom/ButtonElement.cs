using System;
namespace MonoTouch.Dialog
{
	public class ButtonElement:StyledStringElement
	{
		public ButtonElement(string caption,Action tapped): base(caption,tapped)
		{
			
		}
	}
}
