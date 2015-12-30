using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace MonoTouch.Dialog
{
	public class DateTimeInlineElement : DateTimeElement
	{
		public object Tag { get; set; }
		public bool IsMandatory { get; set; }
		private UIColor _defaultColor = UIColor.White;

		bool isPickerPresent = false;
		UIViewElement datePickerContainer;
		/* We want to hide and override the DateSelected event in the base class 
		 * since it is not being used in this implementation. */
		public new event Action DateSelected;

		public DateTimeInlineElement(string caption, DateTime date)
			: base(caption, date)
		{	}

		public override UIDatePicker CreatePicker()
		{
			var picker = new UIDatePicker(RectangleF.Empty)
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				Mode = UIDatePickerMode.Date,
				Date = DateValue
			};
			return picker;
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if(datePicker == null)
			{
				datePicker = CreatePicker();

				// Whenever the DatePicker is changed, get the DateTimeElement cell and change the text.
				datePicker.ValueChanged += (object sender, EventArgs e) =>
				{
					this.DateValue = datePicker.Date;
					var cell = tableView.CellAt(path);
					cell.DetailTextLabel.Text = FormatDate(datePicker.Date);
					if(DateSelected != null)		// Fire our changed event.
						DateSelected();
				};
			}

			TogglePicker(dvc, tableView, path);

			// Deselect the row so the row highlint tint fades away.
			tableView.DeselectRow(path, true);
		}

		private void TogglePicker(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			var sectionAndIndex = GetMySectionAndIndex(dvc);
			if(sectionAndIndex.Key != null)
			{
				Section section = sectionAndIndex.Key;
				int index = sectionAndIndex.Value;

				var cell = tableView.CellAt(path);

				if(isPickerPresent)
				{
					// Remove the picker.
					cell.DetailTextLabel.TextColor = UIColor.Gray;
					section.Remove(datePickerContainer);
					isPickerPresent = false;
				} 
				else
				{
					// Show the picker.
					cell.DetailTextLabel.TextColor = UIColor.Red;
					datePickerContainer = new UIViewElement(string.Empty, datePicker, false);
					section.Insert(index + 1, UITableViewRowAnimation.Bottom, datePickerContainer);
					isPickerPresent = true;
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
			foreach(var section in dvc.Root)
			{
				for(int i = 0; i < section.Count; i++)
				{
					if(section[i] == this)
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
		        //cell.ContentView.AutosizesSubviews = false;
		        if (this.IsMandatory)
		        {
		            //cell.TextLabel.TextColor = UIColor.Purple;
		            cell.TextLabel.Text += "*";
		        } /*else {
		            //cell.TextLabel.TextColor = UIColor.Black;
		        }*/
		        if (!string.IsNullOrEmpty(this.Value))
		        {
		            cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
		        }
		        else
		            cell.BackgroundColor = _defaultColor ?? UIColor.White;
		        return cell;
		    }
	}

}
	//public partial class TaggedDateElement : DateElement
    //{
    //    public TaggedDateElement(string caption, DateTime date) : base(caption, date) { }
    //    public object Tag { get; set; }
    //    public bool IsMandatory { get; set; }
    //    private UIColor _defaultColor = UIColor.White;

    //    public override UITableViewCell GetCell(UITableView tv)
    //    {
    //        var cell = base.GetCell(tv);
    //        //cell.ContentView.AutosizesSubviews = false;
    //        if (this.IsMandatory)
    //        {
    //            //cell.TextLabel.TextColor = UIColor.Purple;
    //            cell.TextLabel.Text += "*";
    //        } /*else {
    //            //cell.TextLabel.TextColor = UIColor.Black;
    //        }*/
    //        if (!string.IsNullOrEmpty(this.Value))
    //        {
    //            cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
    //        }
    //        else
    //            cell.BackgroundColor = _defaultColor ?? UIColor.White;
    //        return cell;
    //    }
    //}