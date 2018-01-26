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
	public class TwoStateElement : Element
	{

		//public bool IsMandatory { get; set; }

		TwoStateChoice val;
		List<TwoStateChoice> choices;

		public virtual TwoStateChoice Value
		{
			get
			{
				return val;
			}
			set
			{
				bool emit = (val == null && value != null) || (val != null && value == null) || val.Id != value.Id;
				val = value;
				if (emit && ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		public event EventHandler ValueChanged;

		public TwoStateElement(string caption, TwoStateChoice firstChoice, TwoStateChoice secondChoice, bool firstIsDefault) : base(caption)
		{
			this.choices = new List<TwoStateChoice>() { firstChoice, secondChoice };
			val = firstIsDefault ? firstChoice : secondChoice;
		}

		static NSString bkey = new NSString("TwoStateElement");
		UISegmentedControl sc;
		protected override NSString CellKey
		{
			get
			{
				return bkey;
			}
		}
		public override UITableViewCell GetCell(UITableView tv)
		{
			var cell = (DialogCell)tv.DequeueReusableCell(CellKey);
			if (cell == null)
			{
				cell = new DialogCell(UITableViewCellStyle.Value1, CellKey);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;

				if (cell.TextLabel != null)
				{
					cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
					cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
					cell.TextLabel.Text = Caption;
					cell.TextLabel.Lines = 0;
					if (this.IsMandatory)
						cell.TextLabel.Text += "*";
				}

				sc = new UISegmentedControl()
				{
					BackgroundColor = UIColor.Clear,
					Tag = 1,
				};
				sc.Selected = true;

				sc.InsertSegment(choices[0].Text, 0, false);
				sc.InsertSegment(choices[1].Text, 1, false);
				sc.SelectedSegment = choices.FindIndex(e => e.Id == val.Id);
				sc.AddTarget(delegate
				{
					Value = choices[(int)sc.SelectedSegment];
				}, UIControlEvent.ValueChanged);

				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
					sc.Frame = new CGRect(570f, 8f, 150f, 26f);
				else
					sc.Frame = new CGRect(210f, 15f, 100f, 26f);

				cell.AddSubview(sc);
				cell.SubviewsLayoutted += (sender, e) =>
				{
					var nw = sc.Frame.X - 10f - cell.TextLabel.Frame.X;
					var nh = HeightForWidth(nw);
					cell.TextLabel.Frame = new CGRect(cell.TextLabel.Frame.X, (cell.Frame.Height - nh) / 2, nw, nh);
					sc.Frame = new CGRect(sc.Frame.X, (cell.Frame.Height - sc.Frame.Height) / 2, sc.Frame.Width, sc.Frame.Height);
				};
			}
			return cell;

		}

		public override nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			var heightBase = base.GetHeight(tableView, indexPath) + 1;
			var nh = HeightForWidth(sc.Frame.X - 30f) + 30;
			return nh > heightBase ? nh : heightBase;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (sc != null)
				{
					sc.Dispose();
					sc = null;
				}
			}
		}
	}

	public class TwoStateChoice
	{
		public Guid Id { get; set; }

		public string Text { get; set; }
	}
}

