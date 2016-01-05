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
using System.Drawing;
using System.IO;

namespace MonoTouch.Dialog
{
    public class ReadonlyElement : StringElement, IElementSizing
    {
        public ReadonlyElement(string caption, string value)
            : base(caption, value)
        {
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            if (cell.DetailTextLabel != null)
                cell.DetailTextLabel.Lines = 0;
            if (cell.TextLabel != null)
                cell.TextLabel.Lines = 0;
            return cell;
        }
        /*
        public virtual nfloat GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = GetCell (tableView);
            int lineCount = 0;
            if (cell != null && cell.DetailTextLabel != null) {
//				lineCount = cell.DetailTextLabel.Lines;
//				using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
//					string line;
//					while ((line = r.ReadLine())!=null) {
//						lineCount += (int)(line.Length / 50);
//					}
//				}
                using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
                    while (r.ReadLine()!=null)
                        lineCount++;
                }
            }
            nfloat lineHeight;
            CGSize size = new CGSize (280, float.MaxValue);
            using (var font = UIFont.FromName ("Helvetica", 17f))
                lineHeight = Caption.StringSize (font, size, UILineBreakMode.WordWrap).Height + 3;
			
            return (nfloat) Math.Max (lineHeight * lineCount + 20, cell.Frame.Height);
        }*/

        public virtual nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
        {
//			var width = (tableView.Frame.Width- WIDTH_OFFSET )/2;
			var width = 500f;
			string reference = (Caption ?? string.Empty).Length > (Value ?? string.Empty).Length ? Caption : Value;
			return (nfloat)Math.Max(HeightForWidth(reference, width), 40F);

        }
    }

}

