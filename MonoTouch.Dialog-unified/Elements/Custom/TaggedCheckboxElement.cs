using System;
using Foundation;
using UIKit;


namespace MonoTouch.Dialog
{
	public partial class TaggedCheckboxElement : CheckboxElement, ITaggedNodeElement
	{
		public object Tag { get; set; }

		//public bool IsBlank { get; set; }


		//public UIColor _originalColor = null;

		public TaggedCheckboxElement(string caption)
			: base(caption)
		{
		}
		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			base.Selected(dvc, tableView, path);
			var selected = OnSelected;
			if (selected != null)
				selected(this, EventArgs.Empty);
		}
		public event EventHandler<EventArgs> OnSelected;

	}
}
