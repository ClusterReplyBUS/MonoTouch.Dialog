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
	public class SelectableMultilineEntryElement : ReadonlyElement
	{
		///s.agostini
		//public bool IsMandatory { get; set; }
		public bool IsReadonly { get; set; }
		///

		private string _saveLabel;

		public SelectableMultilineEntryElement(string caption, string value, string saveLabel)
			: base(caption, value)
		{
			_saveLabel = saveLabel;
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (IsReadonly)
			{
				base.Selected(dvc, tableView, path);
				return;
			}

			var controller = new UIViewController();

			UITextView disclaimerView = new UITextView(controller.View.Frame);
			disclaimerView.BackgroundColor = UIColor.FromWhiteAlpha(0, 0);
			disclaimerView.TextColor = UIColor.Black;
			disclaimerView.TextAlignment = UITextAlignment.Left;
			if (!string.IsNullOrWhiteSpace(Value))
			{
				disclaimerView.Text = Value;
				//disclaimerView.TextColor = UIColor.FromRGB(1f, 1f, 0.8f);
			}
			else
				disclaimerView.Text = string.Empty;

			disclaimerView.Font = UIFont.SystemFontOfSize(16f);
			disclaimerView.Editable = true;

			controller.View.AddSubview(disclaimerView);
			controller.NavigationItem.Title = Caption;
			controller.NavigationItem.RightBarButtonItem = new UIBarButtonItem(string.IsNullOrEmpty(_saveLabel) ? "Save" : _saveLabel, UIBarButtonItemStyle.Done, (object sender, EventArgs e) =>
			{
				if (OnSave != null)
					OnSave(this, EventArgs.Empty);
				controller.NavigationController.
#if XAMCORE_2_0
				PopViewController(true);
#else
                PopViewControllerAnimated (true);
#endif
				Value = disclaimerView.Text;
			});

			dvc.ActivateController(controller);
		}
		public event EventHandler<EventArgs> OnSave;

		public override UITableViewCell GetCell(UITableView tv)
		{
			var cell = base.GetCell(tv);
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			//cell.TextLabel.AdjustsFontSizeToFitWidth = true;
			
			if (!string.IsNullOrWhiteSpace(Value) && Value.Length > 0)
			{
				cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
			}
			if (this.IsMandatory)
				cell.TextLabel.Text += "*";
			return cell;
		}

		public override nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			var heightBase = base.GetHeight(tableView, indexPath) + 1;
			var nh = HeightForWidth(tableView.Frame.Width - 30f) + 30;
			return nh > heightBase ? nh : heightBase;
		}

		//public override nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
		//{
		//	return 90f;
		//	//return base.GetHeight(tableView, indexPath);
		//}
	}
}

