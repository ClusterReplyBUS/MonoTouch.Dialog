using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

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
		
		public override MonoTouch.UIKit.UITableViewCell GetCell (MonoTouch.UIKit.UITableView tv)
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

