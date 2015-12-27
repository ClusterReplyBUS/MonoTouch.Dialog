using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonoTouch.Dialog
{
    public partial class TextOnlyElement : ReadonlyElement
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
                cell.TextLabel.LineBreakMode = MonoTouch.UIKit.UILineBreakMode.WordWrap;

                cell.TextLabel.Lines = 0;
                cell.TextLabel.Frame = new RectangleF(cell.TextLabel.Frame.X, cell.TextLabel.Frame.Y, cell.TextLabel.Frame.Width, LabelSize.Height);

            }
            return cell;
        }

        public override float GetHeight(UITableView tableView, NSIndexPath indexPath)
        {
            //return base.GetHeight(tableView, indexPath);
            return LabelSize.Height + 30;
        }
    }
}
