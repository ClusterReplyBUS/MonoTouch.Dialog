using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using System.IO;

namespace MonoTouch.Dialog
{
	public class ReadonlyElement : StringElement, IElementSizing
	{
		public ReadonlyElement (string caption, string value):base(caption,value)
		{
		}

		public override MonoTouch.UIKit.UITableViewCell GetCell (MonoTouch.UIKit.UITableView tv)
		{
			var cell = base.GetCell (tv);

			if (cell.DetailTextLabel != null) {
				cell.DetailTextLabel.Lines = 0;
				cell.DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			}
			if (cell.TextLabel != null) {
				cell.TextLabel.Lines = 0;
				cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			}
//				cell.DetailTextLabel.LineBreakMode = MonoTouch.UIKit.UILineBreakMode.WordWrap;
//				if (cell != null && cell.DetailTextLabel != null && !string.IsNullOrWhiteSpace (cell.DetailTextLabel.Text)) {
//					int lineCount = 0;
//					using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
//						while (r.ReadLine()!=null)
//							lineCount++;
//					}
//					cell.DetailTextLabel.Lines = 0;
//					cell.TextLabel.Lines = lineCount;
//				}
//			}
			return cell;
		}
		
		public virtual float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = GetCell (tableView);
//			int lineCount = 0;
//			if (cell != null && cell.DetailTextLabel != null) {
////				lineCount = cell.DetailTextLabel.Lines;
////				using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
////					string line;
////					while ((line = r.ReadLine())!=null) {
////						lineCount += (int)(line.Length / 50);
////					}
////				}
//				using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
//					while (r.ReadLine()!=null)
//						lineCount++;
//				}
//			}
//			float lineHeight;
//			SizeF size = new SizeF (280, float.MaxValue);
//			using (var font = UIFont.FromName ("Helvetica", 17f))
//				lineHeight = tableView.StringSize (Value, font, size, UILineBreakMode.WordWrap).Height + 3;
//			
//			return Math.Max (lineHeight * lineCount + 20, cell.Frame.Height);

			float textHeight = 0, titleHeight = 0;
			using (var font = UIFont.FromName ("Helvetica", 17f)) {
				SizeF size = new SizeF (280, float.MaxValue);

				textHeight = tableView.StringSize (string.IsNullOrWhiteSpace(Value) ? "x" : Value, font, size, UILineBreakMode.WordWrap).Height + 3;
				titleHeight = tableView.StringSize (string.IsNullOrWhiteSpace(Caption) ? "x" : Caption, font, size, UILineBreakMode.WordWrap).Height + 3;
			}	
			return Math.Max (textHeight + 10, Math.Max(titleHeight + 10, cell.Frame.Height + 10));
		}
	}
	
}

