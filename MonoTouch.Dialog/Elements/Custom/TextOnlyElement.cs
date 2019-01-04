using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
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
    public partial class TextOnlyElement : ReadonlyElement, IElementSizing
    {
        public TextOnlyElement(string caption)
            : base(caption, string.Empty)
        {

        }
        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);

            if (cell != null && cell.TextLabel != null && !string.IsNullOrWhiteSpace(cell.TextLabel.Text))
            {

                cell.TextLabel.Font = UIFont.ItalicSystemFontOfSize(17);
                //cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;

                cell.TextLabel.Lines = 0;
                var height = HeightForWidth(cell.Frame.Width);
                cell.TextLabel.Frame = new CGRect(cell.TextLabel.Frame.X, cell.TextLabel.Frame.Y, cell.TextLabel.Frame.Width, Math.Max(cell.Frame.Height, height));
				//cell.TextLabel.AdjustsFontSizeToFitWidth = true;
				
            }
            return cell;
        }
        public override bool IsMissing { get => false; set { } }
        public override nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
        {
            ////return base.GetHeight(tableView, indexPath);
            ////return LabelSize.Height + 30;
            //var cell = GetCell(tableView);
            //var height = HeightForWidth(cell.Frame.Width);
            ////return Math.Max(cell.Frame.Height, height);
            //return height+10;
			float heightBase = (float)base.GetHeight(tableView, indexPath) + 1;
			return Math.Max(70, heightBase);
        }
    }
}
