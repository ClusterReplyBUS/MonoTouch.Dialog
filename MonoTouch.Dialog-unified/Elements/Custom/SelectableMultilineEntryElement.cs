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
	public class SelectableMultilineEntryElement : ReadonlyElement
	{
		///s.agostini
		public bool IsMandatory{ get; set; }
		public bool IsReadonly { get; set; }
		///

		private string _saveLabel;

		public SelectableMultilineEntryElement (string caption, string value, string saveLabel) : base (caption, value)
		{
			_saveLabel = saveLabel;
		}

		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (IsReadonly) {
				base.Selected (dvc, tableView, path);
				return;
			}

			var controller = new UIViewController ();

			UITextView disclaimerView = new UITextView (controller.View.Frame);
//			disclaimerView.BackgroundColor = UIColor.FromWhiteAlpha (0, 0);
//			disclaimerView.TextColor = UIColor.White;
//			disclaimerView.TextAlignment = UITextAlignment.Left;
			if (!string.IsNullOrWhiteSpace (Value))
				disclaimerView.Text = Value;
			else
				disclaimerView.Text = string.Empty;
			
			disclaimerView.Font = UIFont.SystemFontOfSize (16f);
			disclaimerView.Editable = true;

			controller.View.AddSubview (disclaimerView);
			controller.NavigationItem.Title = Caption;
			controller.NavigationItem.RightBarButtonItem = new UIBarButtonItem (string.IsNullOrEmpty (_saveLabel) ? "Save" : _saveLabel, UIBarButtonItemStyle.Done, (object sender, EventArgs e) => {
				if (OnSave != null)
					OnSave (this, EventArgs.Empty);
				controller.NavigationController.PopViewController (true);
				Value = disclaimerView.Text;
			});	

			dvc.ActivateController (controller);
		}
		public event EventHandler<EventArgs> OnSave;

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = base.GetCell (tv);
			if (this.IsMandatory)
				cell.TextLabel.Text += "*";
			return cell;
		}
	}
}

