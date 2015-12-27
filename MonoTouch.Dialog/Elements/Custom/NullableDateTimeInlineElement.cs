using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Drawing;

namespace MonoTouch.Dialog
{
	public class NullableDateElementInline : StringElement
	{
		public object Tag { get; set; }
		public bool IsMandatory { get; set; }

		static NSString skey = new NSString("NullableDateTimeElementInline");
		public DateTime? DateValue;
		public event Action DateSelected;
		public event Action PickerClosed;
		public event Action PickerOpened;
		private InlineDateElement _inline_date_element = null;
		private bool _picker_present = false;
		private UIColor _defaultColor = UIColor.White;
        private UIDatePickerMode _mode = UIDatePickerMode.Date;

        public NullableDateElementInline(string caption, DateTime? date) : this(caption, date, UIDatePickerMode.Date) { }
		public NullableDateElementInline(string caption, DateTime? date, UIDatePickerMode mode)
			: base(caption)
		{
			DateValue = date;
            _mode = mode;
            Value = FormatDate(date);
		}

		/// <summary>
		/// Returns true iff the picker is currently open
		/// </summary>
		/// <returns></returns>
		public bool IsPickerOpen()
		{
			return _picker_present;
		}

        //protected internal NSDateFormatter DateFormatter
        //{
        //    get
        //    {
        //        return new NSDateFormatter()
        //        {
        //            DateStyle = _mode == UIDatePickerMode.DateAndTime ? NSDateFormatterStyle.Medium
        //        };
        //    }
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    if (disposing)
        //    {
        //        if (DateFormatter != null)
        //        {
        //            DateFormatter.Dispose();
        //            //DateFormatter = null;
        //        }
        //    }
        //}

		public virtual string FormatDate(DateTime? dt)
		{
			if (!dt.HasValue)
				return " ";

			dt = GetDateWithKind(dt).Value.ToLocalTime();
            //var s = DateFormatter.ToString(dt);
            //if (_mode ==UIDatePickerMode.DateAndTime)
            //{
            //    s+=" " + dt.Value.ToShortTimeString();
            //}
            //return s;
            return _mode == UIDatePickerMode.DateAndTime ? dt.Value.ToShortDateString() + " " + dt.Value.ToShortTimeString() : dt.Value.ToShortDateString();
		}

		protected DateTime? GetDateWithKind(DateTime? dt)
		{
			if (!dt.HasValue)
				return dt;

			if (dt.Value.Kind == DateTimeKind.Unspecified)
				return DateTime.SpecifyKind(dt.Value, DateTimeKind.Local);

			return dt;
		}

		public void ClosePickerIfOpen(DialogViewController dvc)
		{
			if (_picker_present)
			{
				var index_path = this.IndexPath;
				var table_view = this.GetContainerTableView();

				Selected(dvc, table_view, index_path);
			}
		}

		public void SetDate(DateTime? date)
		{
			this.DateValue = date;
			Value = FormatDate(date);
			var r = this.GetImmediateRootElement();
			r.Reload(this, UITableViewRowAnimation.None);
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			TogglePicker(dvc, tableView, path);

			// Deselect the row so the row highlint tint fades away.
			tableView.DeselectRow(path, true);
		}

		/// <summary>
		/// Shows or hides the nullable picker
		/// </summary>
		/// <param name="dvc"></param>
		/// <param name="tableView"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private void TogglePicker(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			var sectionAndIndex = GetMySectionAndIndex(dvc);
			if (sectionAndIndex.Key != null)
			{
				Section section = sectionAndIndex.Key;
				int index = sectionAndIndex.Value;

				var cell = tableView.CellAt(path);

				if (_picker_present)
				{
					// Remove the picker.
					cell.DetailTextLabel.TextColor = UIColor.Gray;
					section.Remove(_inline_date_element);
					_picker_present = false;
					if (PickerClosed != null)
						PickerClosed();
				}
				else
				{
					// Show the picker.
					cell.DetailTextLabel.TextColor = UIColor.Red;
					_inline_date_element = new InlineDateElement(DateValue, _mode);

					_inline_date_element.DateSelected += (DateTime? date) =>
					{
						this.DateValue = date;
						cell.DetailTextLabel.Text = FormatDate(date);
						Value = cell.DetailTextLabel.Text;
						cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
						if (DateSelected != null)       // Fire our changed event.
							DateSelected();
					};

					_inline_date_element.ClearPressed += () =>
					{
						DateTime? null_date = null;
						DateValue = null_date;
						cell.DetailTextLabel.Text = " ";
						Value = cell.DetailTextLabel.Text;
						cell.DetailTextLabel.TextColor = UIColor.Gray;
						section.Remove(_inline_date_element);
						_picker_present = false;
						if (PickerClosed != null)
							PickerClosed();
						cell.BackgroundColor = _defaultColor ?? UIColor.White;
					};

					section.Insert(index + 1, UITableViewRowAnimation.Bottom, _inline_date_element);
					_picker_present = true;
					tableView.ScrollToRow(_inline_date_element.IndexPath, UITableViewScrollPosition.None, true);

					if (PickerOpened != null)
						PickerOpened();
				}
			}
		}

		/// <summary>
		/// Locates this instance of this Element within a given DialogViewController.
		/// </summary>
		/// <returns>The Section instance and the index within that Section of this instance.</returns>
		/// <param name="dvc">Dvc.</param>
		private KeyValuePair<Section, int> GetMySectionAndIndex(DialogViewController dvc)
		{
			foreach (var section in dvc.Root)
			{
				for (int i = 0; i < section.Count; i++)
				{
					if (section[i] == this)
					{
						return new KeyValuePair<Section, int>(section, i);
					}
				}
			}
			return new KeyValuePair<Section, int>();
		}

		public override UITableViewCell GetCell(UITableView tv)
		{
			var cell = base.GetCell(tv);
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.DetailTextLabel.Font = UIFont.SystemFontOfSize(17);

			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			if (this.IsMandatory)
			{
				//cell.TextLabel.TextColor = UIColor.Purple;
				cell.TextLabel.Text += "*";
			} /*else {
		            //cell.TextLabel.TextColor = UIColor.Black;
		        }*/
			if (DateValue.HasValue)
			{
				cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
			}
			else
				cell.BackgroundColor = _defaultColor ?? UIColor.White;
			return cell;
		}

		/// <summary>
		/// Class that has the UIDatePicker and a button for clearing/cancelling
		/// </summary>
		public class InlineDateElement : Element, IElementSizing
		{
			public UIDatePicker _date_picker;
			private UIButton _clear_cancel_button;

			static NSString skey = new NSString("InlineDateElement");

			public event Action<DateTime?> DateSelected;
			public event Action ClearPressed;

			private DateTime? _current_date;
			private SizeF _picker_size;
			private SizeF _cell_size;

            public InlineDateElement(DateTime? current_date) : this(current_date, UIDatePickerMode.Date) { }
			public InlineDateElement(DateTime? current_date, UIDatePickerMode mode)
				: base("")
			{
				_current_date = current_date;
				_date_picker = new UIDatePicker();
				_date_picker.Mode = mode;
				_picker_size = _date_picker.SizeThatFits(SizeF.Empty);
				_cell_size = _picker_size;
				_cell_size.Height += 30f; // Add a little bit for the clear button
			}

			/// <summary>
			/// Returns the cell, with some additions
			/// </summary>
			/// <param name="tv"></param>
			/// <returns></returns>
			public override UITableViewCell GetCell(UITableView tv)
			{
				//Debug.Assert(_date_picker != null);

				var cell = base.GetCell(tv);

				if (!_current_date.HasValue && DateSelected != null)
					DateSelected(DateTime.Now);
				else if (_current_date.HasValue)
					_date_picker.Date = _current_date;

				_date_picker.ValueChanged += (object sender, EventArgs e) =>
				{
					if (DateSelected != null)
						DateSelected(_date_picker.Date);
				};

				if (_clear_cancel_button == null)
				{
					_clear_cancel_button = UIButton.FromType(UIButtonType.RoundedRect);
					_clear_cancel_button.SetTitle("Clear", UIControlState.Normal);                 
				}
				_clear_cancel_button.Frame = new RectangleF(tv.Frame.Width/2 - 20f, _cell_size.Height - 40f, 40f, 40f);
				_date_picker.Frame = new RectangleF(tv.Frame.Width / 2 - _picker_size.Width / 2, _cell_size.Height / 2 - _picker_size.Height / 2, _picker_size.Width, _picker_size.Height);
				_clear_cancel_button.TouchUpInside += (object sender, EventArgs e) =>
				{
					// Clear button pressed. 
					if (ClearPressed != null)
						ClearPressed();
				};

				cell.AddSubview(_date_picker);

				cell.AddSubview(_clear_cancel_button);

				return cell;
			}

			/// <summary>
			/// Returns the height of the cell
			/// </summary>
			/// <param name="tableView"></param>
			/// <param name="indexPath"></param>
			/// <returns></returns>
			public float GetHeight(UITableView tableView, NSIndexPath indexPath)
			{
				return _cell_size.Height;
			}
		}
	}
}

