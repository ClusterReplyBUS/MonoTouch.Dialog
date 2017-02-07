using System;
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

namespace MonoTouch.Dialog
{
	//	public class SignatureElement : UIViewElement
	//	{
	//		private const string CELL_IDENTIFIER = "SignatureCell";
	//		private DrawView _drawView;
	//		
	//		public SignatureElement (UIViewController parent) : base(string.Empty, new DrawView(new CGRect(5f,5f,1000,200), parent), false)
	//		{
	//			base.Flags = UIViewElement.CellFlags.DisableSelection;
	//			_drawView = (DrawView)base.View;
	//		}
	//		
	//		public override UITableViewCell GetCell (UITableView tv)
	//		{
	//			var cell = base.GetCell (tv);
	//			cell.ContentView.Frame.Height = _drawView.Frame.Height + 50;
	//			_drawView.Frame.Width = cell.Frame.Width - 10;
	//			
	//			return cell;
	//		}
	//	}
	public class SignatureElement : Element
	{
		private string _disclaimer;
		//public bool IsMandatory { get; set; }
		public string SignatureBase64
		{
			get
			{
				if (Value == null)
				{
					return null;
				}
				else
				{
					return Convert.ToBase64String(Value.AsJPEG(0.2f).ToArray());
				}
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					Value = null;
				else {
					NSData image = new NSData(value, NSDataBase64DecodingOptions.None);
					Value = UIImage.LoadFromData(image);
				}
			}
		}

		public UIImage Value { get; set; }

		static NSString hkey = new NSString("SignatureElement");

		private string _saveLabel;

		public SignatureElement(string caption, string disclaimer, string saveLabel)
				: this(caption, disclaimer)
		{
			_saveLabel = saveLabel;
		}
		public SignatureElement(string caption, string disclaimer)
				: base(caption)
		{
			_disclaimer = disclaimer;
		}

		protected override NSString CellKey
		{
			get
			{
				return hkey;
			}
		}


		public override UITableViewCell GetCell(UITableView tv)
		{
			var cell = tv.DequeueReusableCell(CellKey);
			if (cell == null)
			{
				cell = new UITableViewCell(UITableViewCellStyle.Default, CellKey);
				cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
			}
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			//s.agostini
			cell.TextLabel.Text = Caption;
			if (this.IsMandatory)
				cell.TextLabel.Text += '*';

			//cell.TextLabel.TextColor = UIColor.Purple;

			if (this.Value != null)
			{
				if (!this.Value.Size.IsEmpty)
				{
					cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
					cell.ImageView.Image = this.Value;
					//cell.ImageView.Frame.X = 20;
				}
			}
			return cell;
		}

		// We use this class to dispose the web control when it is not
		// in use, as it could be a bit of a pig, and we do not want to
		// wait for the GC to kick-in.
		class SignatureViewController : UIViewController
		{
			public SignatureViewController(SignatureElement container)
				: base()
			{

				this.View.BackgroundColor = UIColor.FromWhiteAlpha(1f, 0.3f);

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
			var signatureController = new SignatureViewController(this)
			{
				Autorotate = dvc.Autorotate,
			};

			//			DrawView dv = new DrawView (new CGRect (50, 350, signatureController.View.Frame.Width - 100, signatureController.View.Frame.Height - 600), null) {
			//				BackgroundColor = UIColor.White,
			//				AutoresizingMask = UIViewAutoresizing.All,
			//			};
			var rect = new CGRect(20, signatureController.View.Frame.Height / 3, signatureController.View.Frame.Width - 40, (signatureController.View.Frame.Height / 3 * 2) - 10);
			SmoothedBIView dv = new SmoothedBIView(rect)
			{
				BackgroundColor = UIColor.FromWhiteAlpha(1f, 0f),
				AutoresizingMask = UIViewAutoresizing.All,
			};

			try
			{
				UIImageView background = new UIImageView(UIImage.FromFile("images/Signature.png"));
				var newWidth = signatureController.View.Frame.Width - 40;
				var newHeight = signatureController.View.Frame.Height * background.Frame.Height / background.Frame.Width;
				background.Frame = new CGRect(20, signatureController.View.Frame.Height - 40 - newHeight, newWidth, newHeight);
				signatureController.View.AddSubview(background);
			}
			catch
			{
				Console.WriteLine("Unable to load images/Signature.png for background image");
			}

			var frame = new
#if XAMCORE_2_0
				CGRect
#else
                CGRect 
#endif
				(20, 20, signatureController.View.Frame.Width - 40, signatureController.View.Frame.Height / 4);
			UITextView disclaimerView = new UITextView(frame);
			//disclaimerView.BackgroundColor = UIColor.Gray;
			disclaimerView.BackgroundColor = UIColor.FromWhiteAlpha(0, 0);
			disclaimerView.TextColor = UIColor.Black;
			disclaimerView.TextAlignment = UITextAlignment.Center;
			disclaimerView.Text = _disclaimer;
			disclaimerView.Font = UIFont.SystemFontOfSize(16f);
			disclaimerView.Editable = false;
			disclaimerView.DataDetectorTypes = UIDataDetectorType.Link;

			disclaimerView.Text = _disclaimer;
			signatureController.View.BackgroundColor = UIColor.LightGray;
			signatureController.View.AddSubview(dv);

			signatureController.View.AddSubview(disclaimerView);
			signatureController.NavigationItem.Title = Caption;
			signatureController.NavigationItem.RightBarButtonItem = new UIBarButtonItem(string.IsNullOrEmpty(_saveLabel) ? "Save" : _saveLabel, UIBarButtonItemStyle.Done, (object sender, EventArgs e) =>
			{
			//Value = dv.GetDrawingImage ();
			Value = dv.IncrementalImage;
				var selected = OnSelected;
				if (selected != null)
					selected(this, EventArgs.Empty);

				signatureController.NavigationController.
#if XAMCORE_2_0
					PopViewController(true);
#else
                    PopViewControllerAnimated(true);
#endif
		});

			dvc.ActivateController(signatureController);

		}


		public event EventHandler<EventArgs> OnSelected;
	}
}

