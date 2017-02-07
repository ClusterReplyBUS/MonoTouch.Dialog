using System;
using Foundation;
using UIKit;

namespace MonoTouch.Dialog
{
	public class DialogCell : UITableViewCell
	{
		public event EventHandler SubviewsLayoutted;
		public DialogCell(UITableViewCellStyle style, NSString reuseIdentifier) 
			: base(style, reuseIdentifier)
		{
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			if (SubviewsLayoutted != null)
				SubviewsLayoutted(this, null);
		}
	}
}
