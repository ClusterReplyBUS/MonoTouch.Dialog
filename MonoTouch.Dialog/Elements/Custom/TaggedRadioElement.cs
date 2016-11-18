using System;
#if XAMCORE_2_0
using UIKit;
using Foundation;
using CoreGraphics;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
#endif

#if !XAMCORE_2_0
using nint = global::System.Int32;
using nuint = global::System.UInt32;
using nfloat = global::System.Single;

using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;
using CGRect = global::System.Drawing.RectangleF;
#endif

namespace MonoTouch.Dialog
{
	public class TaggedRadioElement :RadioElement, ITaggedNodeElement
	{
		
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
			dvc.NavigationController.
#if XAMCORE_2_0
                PopViewController(true);
#else
                PopViewControllerAnimated (true);
#endif
		}

		public event EventHandler<EventArgs> OnSelected;
	}
}

