using System;
using System.Collections.Generic;
using Foundation;
using UIKit;


namespace MonoTouch.Dialog
{
public class TaggedRootElement<ElementType> : RootElement
		where ElementType : ITaggedNodeElement
	{
		private string _backButtonLabel;
		private Dictionary<object, ElementType> _selectedChilds;
		private UIColor _defaultColor;

		//public bool IsMandatory { get; set; }

		public ElementType SelectedChild
		{
			get
			{
				if (SelectedChildren.ContainsKey("single"))
					return (ElementType)SelectedChildren["single"];
				return default(ElementType);
			}
			set
			{
				if (SelectedChildren.ContainsKey("single"))
					SelectedChildren["single"] = value;
				else
					SelectedChildren.Add("single", value);
			}
		}
		public Dictionary<object, ElementType> SelectedChildren
		{
			get
			{
				if (_selectedChilds == null)
					_selectedChilds = new Dictionary<object, ElementType>();
				return _selectedChilds;
			}
		}

		public TaggedRootElement(string caption, string backButtonLabel) : base(caption)
		{
			this.IsMandatory = false;
			this._backButtonLabel = backButtonLabel;
		}

		public TaggedRootElement(string caption, Group group, object tag, string backButtonLabel) : base(caption, group)
		{
			this.Tag = tag;
			this.IsMandatory = false;
			this._backButtonLabel = backButtonLabel;
		}

		public override UIKit.UITableViewCell GetCell(UIKit.UITableView tv)
		{
			var cell = base.GetCell(tv);
			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			if (this.IsMandatory)
				cell.TextLabel.Text += "*";

            if (cell.DetailTextLabel != null)
				cell.DetailTextLabel.Lines = 0;
			if (cell.TextLabel != null)
				cell.TextLabel.Lines = 0;

			if (SelectedChildren != null && SelectedChildren.Count > 0)
			{
				if ((SelectedChildren.ContainsKey("single") && !(SelectedChildren["single"] as TaggedRadioElement).IsBlank) || !SelectedChildren.ContainsKey("single"))
				{
					if (_defaultColor == null)
						_defaultColor = cell.BackgroundColor;
					cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
					if (!SelectedChildren.ContainsKey("single"))
						cell.DetailTextLabel.Text = SelectedChildren.Count.ToString();
				}
				else
				{
					cell.DetailTextLabel.Text = string.Empty;
					cell.BackgroundColor = _defaultColor ?? UIColor.White;
				}
			}
			else
			{
				cell.DetailTextLabel.Text = string.Empty;
				cell.BackgroundColor = _defaultColor ?? UIColor.White;
			}
			return cell;
		}
		#if __IOS__
		public override void Selected(DialogViewController dvc, UITableView tableView, Foundation.NSIndexPath path)
		{
			base.Selected(dvc, tableView, path);
			dvc.NavigationItem.Title = _backButtonLabel;
		}
        #endif
        public override nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
        {
            var heightBase = base.GetHeight(tableView, indexPath) + 1;
            var nh = HeightForWidth(tableView.Frame.Width - 30f) + 30;
            return nh > heightBase ? nh : heightBase;
        }
	

	}
}
