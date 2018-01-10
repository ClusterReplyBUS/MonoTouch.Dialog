using System;
using MonoTouch.Dialog;
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
	public class TaggedRootElement : RootElement
	{
		public object Tag { get; set; }
        private UIColor _defaultColor;

		public bool IsMandatory { get; set; }

		public TaggedRadioElement SelectedChild {
			get;
			set;
		}
		
		public TaggedRootElement (string caption) : base(caption)
		{
			this.IsMandatory = false;
		}
		
		public TaggedRootElement (string caption, Group group, object tag) : base(caption, group)
		{
			this.Tag = tag;
			this.IsMandatory = false;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = base.GetCell (tv);
            cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			if (this.IsMandatory)
				cell.TextLabel.Text += "*";
//				cell.TextLabel.TextColor = UIColor.Purple;
//			else
//				cell.TextLabel.TextColor = UIColor.Black;
            if (SelectedChild != null && !SelectedChild.IsBlank)
            {
                if (_defaultColor == null)
                    _defaultColor = cell.BackgroundColor;
                cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
            }
            else
            {
                cell.BackgroundColor = _defaultColor ?? UIColor.White;
            }
			return cell;
		}
	}
}

