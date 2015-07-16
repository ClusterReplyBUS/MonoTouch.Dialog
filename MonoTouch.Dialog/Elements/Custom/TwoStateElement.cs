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
	public class TwoStateElement : Element
	{
		public object Tag{ get; set; }

		public bool IsMandatory{ get; set; }

		TwoStateChoice val;
		List<TwoStateChoice> choices;

		public virtual TwoStateChoice Value {
			get {
				return val;
			}
			set {
				
				bool emit = (val == null && value != null) || (val != null && value == null) || val.Id != value.Id;
				val = value;
				if (emit && ValueChanged != null)
					ValueChanged (this, EventArgs.Empty);
			}
		}

		public event EventHandler ValueChanged;

		public TwoStateElement (string caption, TwoStateChoice firstChoice, TwoStateChoice secondChoice, bool firstIsDefault) : base (caption)
		{
			this.choices = new List<TwoStateChoice> (){ firstChoice, secondChoice };
			val = firstIsDefault ? firstChoice : secondChoice;
		}

		static NSString bkey = new NSString ("TwoStateElement");
		UISegmentedControl sc;
		protected override NSString CellKey {
			get {
				return bkey;
			}
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			sc = new UISegmentedControl () {
				BackgroundColor = UIColor.Clear,
				Tag = 1,
			};
			sc.Selected = true;

			sc.InsertSegment (choices [0].Text, 0, false);
			sc.InsertSegment (choices [1].Text, 1, false);
			sc.Frame = new RectangleF (570f, 8f, 150f, 26f);

			sc.SelectedSegment = choices.FindIndex (e => e.Id == val.Id);
			sc.AddTarget (delegate {
				Value = choices [sc.SelectedSegment];
			}, UIControlEvent.ValueChanged);

			var cell = tv.DequeueReusableCell (CellKey);
//			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, CellKey);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				cell.AddSubview (sc);
//			} 
//			else
//				RemoveTag (cell, 1);
			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize (17);
			//cell.Frame.Height = 44;
			cell.TextLabel.Text = Caption;
			if (this.IsMandatory)
				cell.TextLabel.Text += "*";
			

			return cell;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (sc != null) {
					sc.Dispose ();
					sc = null;
				}
			}
		}
	}

	public class TwoStateChoice
	{
		public Guid Id{ get; set; }

		public string Text { get; set; }
	}
}

