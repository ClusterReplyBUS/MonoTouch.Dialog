using System;
using MonoTouch.Dialog;
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
using System.Drawing;
using System.IO;

namespace MonoTouch.Dialog
{
	public class ReadonlyElement : StringElement, IElementSizing
	{
		public ReadonlyElement (string caption, string value):base(caption,value)
		{
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = base.GetCell (tv);
			if (cell.DetailTextLabel != null) {
				cell.DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
				if (cell != null && cell.DetailTextLabel != null && !string.IsNullOrWhiteSpace (cell.DetailTextLabel.Text)) {
					int lineCount = 0;
					using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
						while (r.ReadLine()!=null)
							lineCount++;
					}
					cell.DetailTextLabel.Lines = 0;
					cell.TextLabel.Lines = lineCount;
				}
			}
			return cell;
		}
		
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
			CGSize size = new CGSize (280, nfloat.MaxValue);
			using (var font = UIFont.FromName ("Helvetica", 17f))
                ///lineHeight = tableView.StringSize(Caption, font, size, UILineBreakMode.WordWrap).Height + 3;
                lineHeight = Caption.StringSize(font, size, UILineBreakMode.WordWrap).Height + 3f;

            return DMath.Max((lineHeight * lineCount + 20), (cell.Frame.Height));
		}
	}
	
}

