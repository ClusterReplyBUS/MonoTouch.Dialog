//
// MultilineEntryElement.cs: multi-line element entry for MonoTouch.Dialog
// 
// Author:
// Aleksander Heintz (alxandr@alxandr.me)
// Based on the code for the EntryElement by Miguel de Icaza
//

using System;
#if XAMCORE_2_0
using UIKit;
using CoreGraphics;
using Foundation;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
#endif
using System.Drawing;
using MonoTouch.Dialog;
#if !XAMCORE_2_0
using nint = global::System.Int32;
using nuint = global::System.UInt32;
using nfloat = global::System.Single;

using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;
using CGRect = global::System.Drawing.RectangleF;
#endif

namespace ElementPack
{
	/// <summary>
	/// An element that can be used to enter text.
	/// </summary>
	/// <remarks>
	/// This element can be used to enter text both regular and password protected entries. 
	///     
	/// The Text fields in a given section are aligned with each other.
	/// </remarks>
	public class SimpleMultilineEntryElement : Element, IElementSizing
	{
		/// <summary>
		///   The value of the EntryElement
		/// </summary>
		public string Value { 
			get {
				return val;
			}
			set {
				val = value;
				if (entry != null)
					entry.Text = value;
			}
		}
		public string Placeholder{ get; set; }

		protected string val;
		
		public bool Editable {
			get {
				return editable;
			}
			set {
				editable = value;
				if (entry != null)
					entry.Editable = editable;
			}
		}
		
		protected bool editable;

		/// <summary>
		/// The key used for reusable UITableViewCells.
		/// </summary>
		static NSString entryKey = new NSString ("MultilineEntryElement");

		protected virtual NSString EntryKey {
			get {
				return entryKey;
			}
		}

		/// <summary>
		/// The type of keyboard used for input, you can change
		/// this to use this for numeric input, email addressed,
		/// urls, phones.
		/// </summary>
		public UIKeyboardType KeyboardType {
			get {
				return keyboardType;
			}
			set {
				keyboardType = value;
				if (entry != null)
					entry.KeyboardType = value;
			}
		}
		
		public UITextAutocapitalizationType AutocapitalizationType {
			get {
				return autocapitalizationType;	
			}
			set { 
				autocapitalizationType = value;
				if (entry != null)
					entry.AutocapitalizationType = value;
			}
		}
		
		public UITextAutocorrectionType AutocorrectionType { 
			get { 
				return autocorrectionType;
			}
			set { 
				autocorrectionType = value;
				if (entry != null)
					this.autocorrectionType = value;
			}
		}
		
		private float height = 112;
		public float Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}
		
		private UIFont inputFont = UIFont.SystemFontOfSize(17);
		public UIFont Font {
			get {
				return inputFont;
			}
			set {
				inputFont = value;
				if (entry != null)
					entry.Font = value;
			}
		}

		UIKeyboardType keyboardType = UIKeyboardType.Default;
		UITextAutocapitalizationType autocapitalizationType = UITextAutocapitalizationType.Sentences;
		UITextAutocorrectionType autocorrectionType = UITextAutocorrectionType.Default;
		bool becomeResponder;
		UITextView entry;
		static UIFont font = UIFont.BoldSystemFontOfSize (17);

		public event EventHandler Changed;
		public event Func<bool> ShouldReturn;
		/// <summary>
		/// Constructs an MultilineEntryElement with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display when no value is set.
		/// </param>
		/// <param name="value">
		/// Initial value.
		/// </param>
		public SimpleMultilineEntryElement (string caption, string value) : this (caption, "", value)
		{ 
		}

		public SimpleMultilineEntryElement (string caption, string placeholder, string value): base(caption)
		{
			Placeholder = placeholder;
			Value = value;
		}
		public override string Summary ()
		{
			return Value;
		}

		// 
		// Computes the X position for the entry by aligning all the entries in the Section
		//
		CGSize ComputeEntryPosition (UITableView tv, UITableViewCell cell)
		{
			Section s = Parent as Section;
			if (s.EntryAlignment.Width != 0)
				return s.EntryAlignment;
			
			// If all EntryElements have a null Caption, align UITextField with the Caption
			// offset of normal cells (at 10px).
            CGSize max = new CGSize(-15, "M".StringSize(font).Height);
			foreach (var e in s.Elements) {
				if (e == null)
					continue;
				
				if (e.Caption != null) {
					var size = e.Caption.StringSize (font);
					if (size.Width > max.Width)
						max = size;
				}
			}
            s.EntryAlignment = new CGSize(25 + Math.Min(max.Width, 160), max.Height);
			return s.EntryAlignment;
		}

		protected virtual UITextView CreateTextField (CGRect frame)
		{
			return new UITextView (frame) {
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin,
				Text = Value ?? "",
				Tag = 1,
				BackgroundColor = UIColor.Clear,
				Editable = editable
			};
		}
		
		static NSString cellkey = new NSString ("MultilineEntryElement");
		
		protected override NSString CellKey {
			get {
				return cellkey;
			}
		}
        protected override UITableViewCellStyle CellStyle => UITableViewCellStyle.Default;
        public override UITableViewCell GetCell (UITableView tv)
		{
            //var cell = tv.DequeueReusableCell (CellKey);
            var cell = base.GetCell(tv);

   //         if (cell == null) {
			//	cell = new UITableViewCell (UITableViewCellStyle.Default, CellKey);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			//} else 
				RemoveTag (cell, 1);
			
			if (entry == null) {
                CGSize size = ComputeEntryPosition(tv, cell);
				nfloat yOffset = (cell.ContentView.Bounds.Height - size.Height) / 2 - 1;
				nfloat width = cell.ContentView.Bounds.Width - size.Width;
				
				entry = CreateTextField (new CGRect (size.Width, yOffset, width, size.Height + (height - 44)));
				entry.Font = inputFont;
				
				entry.Changed += delegate {
					FetchValue ();
				};
				entry.Ended += delegate {
					FetchValue ();
				};
				entry.Started += delegate {
					entry.ReturnKeyType = UIReturnKeyType.Default;

					tv.ScrollToRow (IndexPath, UITableViewScrollPosition.Middle, true);
				};
			}
			if (becomeResponder) {
				entry.BecomeFirstResponder ();
				becomeResponder = false;
			}
			entry.KeyboardType = KeyboardType;
			
			entry.AutocapitalizationType = AutocapitalizationType;
			entry.AutocorrectionType = AutocorrectionType;
			
			cell.TextLabel.Text = Caption;
			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			cell.ContentView.AddSubview (entry);
			return cell;
		}
		
		/// <summary>
		///  Copies the value from the UITextField in the EntryElement to the
		//   Value property and raises the Changed event if necessary.
		/// </summary>
		public void FetchValue ()
		{
			if (entry == null)
				return;

			var newValue = entry.Text;
			if (newValue == Value)
				return;
		
			var currentPos = entry.SelectedRange.Location;	
			Value = newValue;
			
			if (Changed != null)
				Changed (this, EventArgs.Empty);

			if (currentPos > 0)
				entry.SelectedRange = new NSRange(currentPos, 0);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (entry != null) {
					entry.Dispose ();
					entry = null;
				}
			}
		}

		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			BecomeFirstResponder (true);
			tableView.DeselectRow (indexPath, true);
		}
		
		public override bool Matches (string text)
		{
			return (Value != null ? Value.IndexOf (text, StringComparison.CurrentCultureIgnoreCase) != -1 : false) || base.Matches (text);
		}
		
		/// <summary>
		/// Makes this cell the first responder (get the focus)
		/// </summary>
		/// <param name="animated">
		/// Whether scrolling to the location of this cell should be animated
		/// </param>
		public void BecomeFirstResponder (bool animated)
		{
			becomeResponder = true;
			var tv = GetContainerTableView ();
			if (tv == null)
				return;
			tv.ScrollToRow (IndexPath, UITableViewScrollPosition.Middle, animated);
			if (entry != null) {
				entry.BecomeFirstResponder ();
				becomeResponder = false;
			}
		}

		public void ResignFirstResponder (bool animated)
		{
			becomeResponder = false;
			var tv = GetContainerTableView ();
			if (tv == null)
				return;
			tv.ScrollToRow (IndexPath, UITableViewScrollPosition.Middle, animated);
			if (entry != null)
				entry.ResignFirstResponder ();
		}

		public override void Deselected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
//			viewController.View.EndEditing (true);
			base.Deselected (dvc, tableView, path);
			var tap = new UITapGestureRecognizer ();
			tap.AddTarget (() => dvc.View.EndEditing (true));
			dvc.View.AddGestureRecognizer (tap);
			tap.CancelsTouchesInView = false;
		}

		#region IElementSizing implementation
		public virtual nfloat GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return height;
		}
		#endregion
	}
}

