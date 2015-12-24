using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using CoreGraphics;

using NSAction = global::System.Action;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
#endif

namespace MonoTouch.Dialog
{
	public class TaggedRadioElement :RadioElement
	{
		public object Tag { get; set; }
		
		public bool IsBlank { get; set; }

		public UIColor _originalColor = null;

		public TaggedRadioElement (string caption):base(caption)
		{
		}
		
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			base.Selected (dvc, tableView, path);
			var selected = OnSelected;
			if (selected != null)
				selected (this, EventArgs.Empty);
			dvc.NavigationController.PopViewController (true);
		}

		public event EventHandler<EventArgs> OnSelected;
	}
}

