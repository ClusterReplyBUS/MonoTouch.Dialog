using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;

namespace MonoTouch.Dialog
{
	public class RightAlignEntryElement : EntryElement
	{
		#region PRIVATE MEMBERS
		private string _placeholder;
        private UIColor _defaultColor = UIColor.White;
        //private UIColor _defaultColor = UIColor.FromRGB(247, 247, 247);
        private NSString _cellKey = new NSString("RightAlignEntryElement");
		#endregion
		
		#region PUBLIC PROPERTIES
		public object Tag{ get; set; }

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
		/*
		protected override UITextField CreateTextField (RectangleF frame)
		{
			//RectangleF newFrame = new RectangleF (152f, 10.5f, 168f, 21f);
			return new UITextField (new RectangleF (152f, 10.5f, 520f, 21f)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin,
				Placeholder = _placeholder ?? "",
				SecureTextEntry = IsPassword,
				Text = Value ?? "",
				Tag = 1,
			//BackgroundColor = UIColor.LightTextColor,
				TextAlignment = UITextAlignment.Right,
				ClearButtonMode = UITextFieldViewMode.WhileEditing,
			};
		}
		*/
		#endregion
	}
	
}