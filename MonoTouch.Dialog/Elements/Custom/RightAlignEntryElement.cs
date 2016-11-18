using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using MonoTouch.Dialog.Utilities;

namespace MonoTouch.Dialog
{
	public class RightAlignEntryElement : EntryElement, IElementSizing
	{
		#region PRIVATE MEMBERS
		private string _placeholder;
        private UIColor _defaultColor = UIColor.White;
        //private UIColor _defaultColor = UIColor.FromRGB(247, 247, 247);
        private NSString _cellKey = new NSString("RightAlignEntryElement");
		#endregion
		
		#region PUBLIC PROPERTIES

		public bool IsMandatory { get; set; }

		//public bool IsPassword { get; set; }
		#endregion
		
		#region CTOR
		public RightAlignEntryElement (string caption, string placeholder, string value):this(caption, placeholder, value, false)
		{
			base.Changed += HandleBaseChanged;
		}

		public RightAlignEntryElement (string caption, string placeholder, string value, bool isPassword):base(caption, placeholder, value, isPassword)
		{
			this._placeholder = placeholder;
			//this.IsPassword = isPassword;
			this.IsMandatory = false;
            this.TextAlignment = UITextAlignment.Right;
            this.ClearButtonMode = UITextFieldViewMode.WhileEditing;
            
			base.Changed += HandleBaseChanged;
		}
		#endregion
		
		#region PRIVATE METHODS
		private void HandleBaseChanged (object sender, EventArgs e)
		{
			var cell = this.GetActiveCell ();
			if (cell != null) {
				if (!string.IsNullOrEmpty (this.Value)) {
					cell.BackgroundColor = UIColor.FromRGB (1f, 1f, 0.8f);
				} else
					cell.BackgroundColor = _defaultColor;
			}
		}

		#endregion
		
		#region OVERRIDE METHODS
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = base.GetCell (tv);
			//cell.ContentView.AutosizesSubviews = false;
			if (this.IsMandatory) {
				//cell.TextLabel.TextColor = UIColor.Purple;
				cell.TextLabel.Text += "*";
			} /*else {
				//cell.TextLabel.TextColor = UIColor.Black;
			}*/
			if (!string.IsNullOrEmpty (this.Value)) {
				cell.BackgroundColor = UIColor.FromRGB (1f, 1f, 0.8f);
			} else
				cell.BackgroundColor = _defaultColor ?? UIColor.White;
			return cell;
		}

		public virtual nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			var width = (tableView.Frame.Width- WIDTH_OFFSET )/2;
			string reference = (Caption ?? string.Empty).Length > (Value ?? string.Empty).Length ? Caption : Value;
			return (nfloat)Math.Max(HeightForWidth(reference, width), 40F);

		}
		#endregion
	}
	
}