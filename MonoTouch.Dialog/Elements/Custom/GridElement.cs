using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Drawing;

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
using System.IO;

namespace MonoTouch.Dialog
{
	public class GridElement : Element//, IElementSizing
	{
		//public bool IsMandatory { get; set; }

		public object ValueGrid { get; set; }

		public List<GridHeader> Rows { get; set; }
		public List<GridHeader> Columns { get; set; }
		public GridAnswerType GridType { get; set; }
		public UserSource Source;

		static NSString hkey = new NSString("GridElement");

		private string _saveLabel;


		public GridElement(string caption, string saveLabel) : this(caption)
		{
			_saveLabel = saveLabel;
		}
		public GridElement(string caption) : base(caption)
		{
		}

		protected override NSString CellKey
		{
			get
			{
				return hkey;
			}
		}
        protected override UITableViewCellStyle CellStyle => UITableViewCellStyle.Subtitle;
        public override UITableViewCell GetCell(UITableView tv)
		{
			var cell = base.GetCell(tv);
            cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
            cell.TextLabel.Lines = 0;
			cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.TextLabel.AdjustsFontSizeToFitWidth = true;

			//s.agostini
			cell.TextLabel.Text = Caption;
			cell.DetailTextLabel.Text = "";

			if (this.IsMandatory)
				cell.TextLabel.Text += '*';

			if (this.ValueGrid != null)
			{
				cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
				//cell.ImageView.Image = this.Value;
				//cell.ImageView.Frame.X = 20;
				cell.DetailTextLabel.Text = (string)this.ValueGrid;
			}

			if (cell.DetailTextLabel != null)
			{
				cell.DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
				if (cell != null && cell.DetailTextLabel != null && !string.IsNullOrWhiteSpace(cell.DetailTextLabel.Text))
				{
					int lineCount = 0;
					using (StringReader r = new StringReader(cell.DetailTextLabel.Text))
					{
						while (r.ReadLine() != null)
							lineCount++;
					}
					cell.DetailTextLabel.Lines = 0;
				}
			}
			return cell;
		}

//		public virtual nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
//		{
//			UITableViewCell cell = GetCell(tableView);
//			UIEdgeInsets margins = cell.LayoutMargins;
//			/*int lineCount = 0;
//			if (cell != null && cell.DetailTextLabel != null) {
////				lineCount = cell.DetailTextLabel.Lines;
////				using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
////					string line;
////					while ((line = r.ReadLine())!=null) {
////						lineCount += (int)(line.Length / 50);
////					}
////				}
//				using (StringReader r = new StringReader(cell.DetailTextLabel.Text)) {
//					while (r.ReadLine()!=null)
//						lineCount++;
//				}
//			}
//            nfloat lineHeight;
//           CGSize size = new CGSize(280, float.MaxValue);
//			using (var font = UIFont.FromName ("Helvetica", 17f))
//				lineHeight =  Caption.StringSize(font, size, UILineBreakMode.WordWrap).Height + 3;

//            return (nfloat)Math.Max(lineHeight * lineCount + 20, cell.Frame.Height);*/
//			//var width = (tableView.Frame.Width - WIDTH_OFFSET);


//			float width = 0f;
//			float height = 0f;
//			float widthFont;
//			float heightFont;
//			float widthScreen = (float)UIScreen.MainScreen.Bounds.Size.Width * (float)UIScreen.MainScreen.Scale;
//			//width = widthFont / widthScreen Math.Floor
//			////		var width = 320f;
//			string reference = (Caption ?? string.Empty).Length > (cell.TextLabel.Text ?? string.Empty).Length ? Caption : cell.TextLabel.Text;
//			int count = reference.Count(f => f == '\n');
//			int captionLenght = reference.Length;
//			if (captionLenght > 0)
//			{
//				//var Wfont = reference.Substring(0, 1).ToLower();
//				NSString nsString = new NSString("A");
//				UIStringAttributes attribs=null;
//				if (UIDevice.CurrentDevice.Model.Contains("iPhone"))
//				{
//					attribs = new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(16) };
//				}
//				else
//				{
//					 attribs = new UIStringAttributes { Font = UIFont.BoldSystemFontOfSize(17) };	
//				}
//				SizeF size = (System.Drawing.SizeF)nsString.GetSizeUsingAttributes(attribs);
//				if (UIDevice.CurrentDevice.Model.Contains("iPhone"))
//				{
//					widthFont = size.Width * (float)UIScreen.MainScreen.Scale;
//				}
//				else
//				{
//					widthFont = size.Width;
//				}
//				heightFont = size.Height + 1;
//				width = (float)Math.Floor(widthScreen / widthFont);
//				height = heightFont;
//			}
//			int numLines = (int)Math.Ceiling(captionLenght / width) + count;
//			//cell.TextLabel.Lines = numLines;
//			int lineCount = 0;
//			if (cell != null && cell.DetailTextLabel != null)
//			{
//				lineCount = (int)cell.DetailTextLabel.Lines;

//				using (StringReader r = new StringReader(cell.DetailTextLabel.Text))
//				{
//					while (r.ReadLine() != null)
//						lineCount++;
//				}
//				//cell.DetailTextLabel.Lines = lineCount;
//				numLines = numLines + lineCount;
//			}
//			//var height = (nfloat)Math.Max(HeightForWidth(reference, width), 40F);
//			return (numLines * height);
//		}

		// We use this class to dispose the web control when it is not
		// in use, as it could be a bit of a pig, and we do not want to
		// wait for the GC to kick-in.
		class GridViewController : UIViewController
		{
			public GridViewController(GridElement container) : base()
			{
				this.View.BackgroundColor = UIColor.White;
			}

			public override void ViewWillDisappear(bool animated)
			{
				base.ViewWillDisappear(animated);
			}

			public bool Autorotate { get; set; }

			//			public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
			//			{
			//				return Autorotate;
			//			}
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			var gridController = new GridViewController(this)
			{
				Autorotate = dvc.Autorotate,
			};
			int cellSize = 0;
			if (UIDevice.CurrentDevice.Model.Contains("iPhone"))
			{
				cellSize = 60;
			}
			else
			{
				cellSize = 112;
			}

			//var cellSize = 60;

			CGRect frame = new CGRect(0, 0, ((Columns.Count + 1) * (cellSize + 15)), gridController.View.Frame.Height);

			//			CGRect frame = new CGRect (0, 0, gridController.View.Frame.Width + 
			//				(Columns.Count > 4 ? (Columns.Count -4) * (cellSize +4) : 0), gridController.View.Frame.Height );
			CGRect frame2 = gridController.View.Frame;
			//CGRect frame2 = new CGRect (20, 20, signatureController.View.Frame.Width - 40, signatureController.View.Frame.Height - 40);
			//			UITextView disclaimerView = new UITextView (frame);
			//			disclaimerView.BackgroundColor = UIColor.FromWhiteAlpha (0, 0);
			//			disclaimerView.TextColor = UIColor.White;
			//			disclaimerView.TextAlignment = UITextAlignment.Left;
			//			disclaimerView.Text = "";
			//			foreach (var col in Columns) {
			//				disclaimerView.Text += " " + col;
			//			}
			//			foreach (var row in Rows) {
			//				disclaimerView.Text += "\n" + row;
			//			}
			//			disclaimerView.Font = UIFont.SystemFontOfSize (16f);
			//			disclaimerView.Editable = false;
			//			disclaimerView.DataDetectorTypes = UIDataDetectorType.Link;
			//			signatureController.View.AddSubview (disclaimerView);

			var scroll = new UIScrollView(frame2);




			if (Source == null)
			{
				Source = new UserSource()
				{
					GridType = this.GridType,
				};
				var lcol = new List<UserElement>();
				lcol.Add(new UserElement(""));
				foreach (var col in Columns)
				{
					lcol.Add(new UserElement(col.Text));
				}
				Source.Rows.Add(lcol);
				foreach (var row in Rows)
				{
					var lrow = new List<UserElement>();
					lrow.Add(new UserElement(row.Text));
					foreach (var col in Columns)
					{
						lrow.Add(new UserElement(this.GridType, true, false)
						{
							AnswerId = row.AnswerId,
							ColumnId = col.AnswerId
						});
					}
					Source.Rows.Add(lrow);
				}
			}

			Source.FontSize = 14f;



			var layout = new UICollectionViewFlowLayout();
			layout.SectionInset = new UIEdgeInsets(1, 1, 1, 1);
			layout.ItemSize = new CGSize((float)cellSize - 1, (float)cellSize - 1);
			var collectionViewUser = new UICollectionView(frame, layout);
			collectionViewUser.BackgroundColor = UIColor.White;

			collectionViewUser.RegisterClassForCell(typeof(UserCell), UserCell.CellID);
			collectionViewUser.ShowsHorizontalScrollIndicator = true;
			collectionViewUser.ShowsVerticalScrollIndicator = true;
			collectionViewUser.ScrollEnabled = true;
			collectionViewUser.DirectionalLockEnabled = false;
			collectionViewUser.Source = Source;

			collectionViewUser.ReloadData();


			scroll.AddSubview(collectionViewUser);
			scroll.ContentSize = frame.Size;
			if (scroll.ContentSize.Width < gridController.View.Frame.Width)
				scroll.ContentOffset = new CGPoint(scroll.ContentSize.Width / 2 - scroll.Bounds.Size.Width / 2 + 10, 0f);
			gridController.View.AddSubview(scroll);


			gridController.NavigationItem.Title = Caption;
			gridController.NavigationItem.RightBarButtonItem = new UIBarButtonItem(string.IsNullOrEmpty(_saveLabel) ? "Save" : _saveLabel, UIBarButtonItemStyle.Done, (object sender, EventArgs e) =>
			{

				string text = "";
				foreach (var row in Source.Rows)
				{
					foreach (var el in row)
					{
						if (el.Checked == true)
						{
							if (text == "")
								text = this.Rows.SingleOrDefault(r => r.AnswerId == el.AnswerId).Text + "-" +
									this.Columns.SingleOrDefault(r => r.AnswerId == el.ColumnId).Text +
									((GridType == GridAnswerType.Text || GridType == GridAnswerType.Number) ? ":" + el.Caption : "");
							else
								text += "\n" + this.Rows.SingleOrDefault(r => r.AnswerId == el.AnswerId).Text + "-" +
									this.Columns.SingleOrDefault(r => r.AnswerId == el.ColumnId).Text +
									((GridType == GridAnswerType.Text || GridType == GridAnswerType.Number) ? ":" + el.Caption : "");
						}
					}
				}


				ValueGrid = text;
				var selected = OnSelected;
				if (selected != null)
					selected(this, EventArgs.Empty);
				gridController.NavigationController
#if XAMCORE_2_0
					.PopViewController(true);
#else
                    .PopViewControllerAnimated (true);  
#endif
			});

			dvc.ActivateController(gridController);

		}

		public event EventHandler<EventArgs> OnSelected;






		public class UserSource : UICollectionViewSource
		{
			public UserSource()
			{
				Rows = new List<List<UserElement>>();
			}

			public List<List<UserElement>> Rows { get; private set; }
			public GridAnswerType GridType { get; set; }
			public Single FontSize { get; set; }

#if !XAMCORE_2_0
			public override Int32 NumberOfSections(UICollectionView collectionView)
			{
				return Rows.Count;
			}

			public override Int32 GetItemsCount(UICollectionView collectionView, Int32 section)
			{
				return Rows[section].Count;
			}
#endif
			public override nint NumberOfSections(UICollectionView collectionView)
			{
				return Rows.Count;
			}

			public override nint GetItemsCount(UICollectionView collectionView, nint section)
			{
				return Rows[(int)section].Count;
			}


			public override Boolean ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
			{
				return true;
			}

			//			public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
			//			{
			//				var cell = (UserCell) collectionView.CellForItem(indexPath);
			//			}

			public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
			{
				var cell = (UserCell)collectionView.CellForItem(indexPath);

				UserElement row = Rows[indexPath.Section][indexPath.Row];
				if (row.Tappable)
				{
					if (GridType == GridAnswerType.Checkbox)
					{
						row.Checked = row.Checked ? false : true;
						row.Caption = row.Checked ? "☑" : "☐";
					}
					else if (GridType == GridAnswerType.Text)
					{
						row.Checked = true;
						row.Caption = "risposta";
					}
					cell.UpdateRow(row, FontSize);
				}
				//row.Tapped.Invoke();
			}

			public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
			{
				var cell = (UserCell)collectionView.DequeueReusableCell(UserCell.CellID, indexPath);

				UserElement row = Rows[indexPath.Section][indexPath.Row];

				cell.UpdateRow(row, FontSize);

				return cell;
			}
		}

		public class UserElement
		{
			//			public UserElement(String caption, NSAction tapped)
			//			{
			//				Caption = caption;
			//				Tapped = tapped;
			//			}

			public UserElement(string caption)
			{
				Caption = caption;
				Type = GridAnswerType.None;
				Tappable = false;
				Checked = false;
			}

			//			public UserElement(GridAnswerType type)
			//			{
			//				Type = type;
			//				if (type == GridAnswerType.Checkbox)
			//					Caption = "☐";
			//				else
			//					Caption = "";
			//			}

			public UserElement(GridAnswerType type, bool tappable, bool check)
			{
				Type = type;
				if (type == GridAnswerType.Checkbox)
				{
					if (check)
						Caption = "☑";
					else
						Caption = "☐";
				}
				else
					Caption = "";
				Tappable = tappable;
				Checked = check;
			}

			public UserElement(GridAnswerType type, bool tappable, bool check, string caption)
			{
				Type = type;
				if (type == GridAnswerType.Checkbox)
				{
					if (check)
						Caption = "☑";
					else
						Caption = "☐";
				}
				else
					Caption = caption;
				Tappable = tappable;
				Checked = check;
			}

			public String Caption { get; set; }
			public bool Checked { get; set; }
			public bool Tappable { get; set; }
			public GridAnswerType Type { get; set; }

			public Guid AnswerId { get; set; }
			public Guid ColumnId { get; set; }

			//			public NSAction Tapped { get; set; }
		}


		public class GridHeader
		{
			public Guid AnswerId { get; set; }
			public string Text { get; set; }
		}

		public enum GridAnswerType
		{
			None = 0,
			Checkbox,
			Text,
			Number
		}


		public class UserCell : UICollectionViewCell
		{
			public static NSString CellID = new NSString("UserSource");

			[Export("initWithFrame:")]
			public UserCell(CGRect frame) : base(frame)
			{
				BackgroundView = new UIView { BackgroundColor = UIColor.Black };

				SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Blue };

				ContentView.Layer.BorderColor = UIColor.Black.CGColor;
				ContentView.Layer.BorderWidth = 0.0f;
				ContentView.BackgroundColor = UIColor.White;
				//ContentView.Transform = CGAffineTransform.MakeScale (0.9f, 0.9f);

				LabelView = new UILabel();
				LabelView.BackgroundColor = UIColor.Clear;
				LabelView.TextColor = UIColor.DarkGray;
				LabelView.TextAlignment = UITextAlignment.Center;
				LabelView.LineBreakMode = UILineBreakMode.WordWrap;
				LabelView.Lines = 0;

				TextBox = new UITextField();

				OtherView = new UIView();
				OtherView.BackgroundColor = UIColor.LightGray;

				ContentView.AddSubview(LabelView);
				ContentView.AddSubview(TextBox);
				ContentView.AddSubview(OtherView);
			}

			public UILabel LabelView { get; private set; }

			public UITextField TextBox { get; private set; }

			public UIView OtherView { get; private set; }

			public void UpdateRow(UserElement element, Single fontSize)
			{
				LabelView.Text = element.Caption;

				LabelView.Font = UIFont.FromName("HelveticaNeue-Bold", fontSize);

				LabelView.Frame = new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height);

				if (element.Type == GridAnswerType.Text || element.Type == GridAnswerType.Number)
				{
					LabelView.Frame = new CGRect(0, 0, 0, 0);
					TextBox.Frame = new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height);
					//TextBox.BorderStyle = UITextBorderStyle.Line;

					TextBox.TextAlignment = UITextAlignment.Center;
					TextBox.Text = element.Caption;
					TextBox.EditingChanged += (object sender, EventArgs e) =>
					{
						element.Caption = ((UITextField)sender).Text;
						element.Checked = true;
					};
					if (element.Type == GridAnswerType.Number)
						TextBox.KeyboardType = UIKeyboardType.NumberPad;

					OtherView.Frame = new CGRect(0, ContentView.Frame.Height - 30, ContentView.Frame.Width, 1);
				}
			}
		}

		//		private void elementTapped(String title)
		//		{
		//			new UIAlertView("Tapped", title, null, "OK", null).Show();
		//		}
	}
}

