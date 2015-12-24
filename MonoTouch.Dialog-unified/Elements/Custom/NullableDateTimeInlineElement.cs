﻿using System;
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
using System.Collections.Generic;
using System.Drawing;

namespace MonoTouch.Dialog
{
	public class NullableDateElementInline : StringElement
	{
		public object Tag { get; set; }
		public bool IsMandatory { get; set; }

		static NSString skey = new NSString("NullableNSDateElementInline");
		public NSDate DateValue;
		public event Action DateSelected;
		public event Action PickerClosed;
		public event Action PickerOpened;
		private InlineDateElement _inline_date_element = null;
		private bool _picker_present = false;
		private UIColor _defaultColor = UIColor.White;

		public NullableDateElementInline(string caption, NSDate date)
			: base(caption)
		{
			DateValue = date;
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

		protected internal NSDateFormatter fmt = new NSDateFormatter()
		{
			DateStyle = NSDateFormatterStyle.Medium
		};

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (fmt != null)
				{
					fmt.Dispose();
					fmt = null;
				}
			}
		}

		public virtual string FormatDate(NSDate dt)
		{
			if (dt == null)
				return " ";

			dt = GetDateWithKind(dt);
			return fmt.ToString(dt);
		}

		protected NSDate GetDateWithKind(NSDate date)
		{
            if (date == null)
                return date;
            var dt = date.ToDateTime();

			if (dt.Kind == DateTimeKind.Unspecified)
				return DateTime.SpecifyKind(dt, DateTimeKind.Local).ToNSDate();

            return dt.ToNSDate();
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

		public void SetDate(NSDate date)
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
					_inline_date_element = new InlineDateElement(DateValue);

					_inline_date_element.DateSelected += (NSDate date) =>
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
						NSDate null_date = null;
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
			if (DateValue != null)
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

			public event Action<NSDate> DateSelected;
			public event Action ClearPressed;

			private NSDate _current_date;
			private CGSize _picker_size;
			private CGSize _cell_size;

			public InlineDateElement(NSDate current_date)
				: base("")
			{
				_current_date = current_date;
				_date_picker = new UIDatePicker();
				_date_picker.Mode = UIDatePickerMode.Date;
				_picker_size = _date_picker.SizeThatFits(CGSize.Empty);
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

				if (_current_date == null && DateSelected != null)
					DateSelected(NSDate.Now);
				else if (_current_date != null)
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
				_clear_cancel_button.Frame = new CGRect(tv.Frame.Width/2 - 20f, _cell_size.Height - 40f, 40f, 40f);
				_date_picker.Frame = new CGRect(tv.Frame.Width / 2 - _picker_size.Width / 2, _cell_size.Height / 2 - _picker_size.Height / 2, _picker_size.Width, _picker_size.Height);
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
			public nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
			{
				return _cell_size.Height;
			}
		}
	}
}

