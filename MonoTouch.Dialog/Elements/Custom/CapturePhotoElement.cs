using System;
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
	public class CapturePhotoElement : Element, IElementSizing
	{
		public bool IsReadOnly { get; set; }

		public bool Mandatory { get; set; }

		public UIImage Value { get; set; }

		public string Base64Value
		{
			get
			{
				if (Value != null)
				{
					return Convert.ToBase64String(this.Value.AsJPEG().ToArray());
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				if (!String.IsNullOrWhiteSpace(value))
					UIImage.LoadFromData(NSData.FromArray(Convert.FromBase64String(value)));
				else
					this.Value = null;
			}
		}

		protected bool _showSelector = false;
		protected string _selectorCancelLabel = "Cancel";
		protected string _selectorTakePhotoLabel = "Take photo";
		protected string _selectorPickImageLabel = "Pick image";


		float newHeight = 1024f;

		static NSString hkey = new NSString("CapturePhotoElement");

		public CapturePhotoElement(string caption) : base(caption)
		{
		}

		public CapturePhotoElement(string caption, string base64value, bool showSelector, string selectorTakePhotoLabel, string selectorPickImageLabel) : this(caption)
		{
			this.Base64Value = base64value;
			this._showSelector = showSelector;
			if (!string.IsNullOrWhiteSpace(selectorPickImageLabel))
				this._selectorPickImageLabel = selectorPickImageLabel;
			if (!string.IsNullOrWhiteSpace(selectorTakePhotoLabel))
				this._selectorTakePhotoLabel = selectorTakePhotoLabel;
		}
		public CapturePhotoElement(string caption, string base64value) : this(caption, base64value, false, null, null)
		{
		}

		protected override NSString CellKey
		{
			get
			{
				return hkey;
			}
		}

		#region IElementSizing implementation
		public virtual nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			return 200;
		}
		#endregion

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
			if (this.Mandatory)
				cell.TextLabel.Text += '*';

			//cell.TextLabel.TextColor = UIColor.Purple;

			if (this.Value != null)
			{
				cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
				cell.ImageView.Image = this.Value;
				//cell.ImageView.Frame.X = 20;
			}
			return cell;
		}

		// We use this class to dispose the web control when it is not
		// in use, as it could be a bit of a pig, and we do not want to
		// wait for the GC to kick-in.
		/*
		class CapturePhotoViewController : UIViewController
		{
			public CapturePhotoViewController (SignatureElement container) : base ()
			{
				this.View.BackgroundColor = UIColor.FromWhiteAlpha (1f, 0.3f);
			}
			
			public override void ViewWillDisappear (bool animated)
			{
				base.ViewWillDisappear (animated);
			}

			public bool Autorotate { get; set; }
			
//			public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
//			{
//				return Autorotate;
//			}
		}
		*/

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (_showSelector)
			{
				UIActionSheet actSheet = new UIActionSheet(
					null,
					null,
					_selectorCancelLabel,
					null,
					_selectorTakePhotoLabel,
					_selectorPickImageLabel
				);
				actSheet.Clicked += (object s, UIButtonEventArgs args) =>
				{
					switch (args.ButtonIndex)
					{
						case 0:
							TakePhoto(dvc);
							break;
						case 1:
							Camera.SelectPicture(dvc, (obj) =>
							{
								var photo = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
								//Value = photo;
								Value = photo.Scale(new CGSize(this.newHeight * photo.Size.Width / photo.Size.Height, this.newHeight));
								var selected = OnSelected;
								if (selected != null)
									selected(this, EventArgs.Empty);
							});
							break;
					}
				};
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
					actSheet.ShowFromToolbar(dvc.NavigationController.Toolbar);
				else {
					var cell = tableView.CellAt(path);
					actSheet.ShowFrom(cell.Bounds, cell, true);
				}
			}
			else {
				TakePhoto(dvc);
			}

			/*
			var signatureController = new SignatureViewController (this) {
				Autorotate = dvc.Autorotate,
			};
			
//			DrawView dv = new DrawView (new CGRect (50, 350, signatureController.View.Frame.Width - 100, signatureController.View.Frame.Height - 600), null) {
//				BackgroundColor = UIColor.White,
//				AutoresizingMask = UIViewAutoresizing.All,
//			};
			SmoothedBIView dv = new SmoothedBIView(new CGRect (50, 350, signatureController.View.Frame.Width - 100, signatureController.View.Frame.Height - 600)){
				BackgroundColor = UIColor.FromWhiteAlpha(1f,1f),
				AutoresizingMask = UIViewAutoresizing.All,
			};

			try{
				UIImageView background = new UIImageView (UIImage.FromFile ("images/Signature.png"));
				background.Frame = dv.Frame;
				signatureController.View.AddSubview (background);
			}
			catch{
				Console.WriteLine ("Unable to load images/Signature.png for background image");
			}

			CGRect frame = new CGRect (20, 20, signatureController.View.Frame.Width - 40, 300);
			UITextView disclaimerView = new UITextView (frame);
			disclaimerView.BackgroundColor = UIColor.FromWhiteAlpha (0, 0);
			disclaimerView.TextColor = UIColor.White;
			disclaimerView.TextAlignment = UITextAlignment.Center;
			disclaimerView.Text = _disclaimer;
			disclaimerView.Font = UIFont.SystemFontOfSize (16f);
			disclaimerView.Editable = false;
			disclaimerView.DataDetectorTypes = UIDataDetectorType.Link;
			
			signatureController.View.AddSubview (disclaimerView);
			signatureController.View.AddSubview (dv);
			signatureController.NavigationItem.Title = Caption;
			signatureController.NavigationItem.RightBarButtonItem = new UIBarButtonItem (string.IsNullOrEmpty(_saveLabel) ? "Save" : _saveLabel, UIBarButtonItemStyle.Done, (object sender, EventArgs e) => {
				//Value = dv.GetDrawingImage ();
				Value = dv.IncrementalImage;
				var selected = OnSelected;
				if (selected != null)
					selected (this, EventArgs.Empty);
				signatureController.NavigationController.PopViewControllerAnimated (true);
			});	
			
			dvc.ActivateController (signatureController);
*/
		}

		public event EventHandler<EventArgs> OnSelected;

		private void TakePhoto(DialogViewController dvc)
		{
			Camera.TakePicture(dvc, (obj) =>
			{
				var photo = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
				//Value = photo;
				Value = photo.Scale(new CGSize(this.newHeight * photo.Size.Width / photo.Size.Height, this.newHeight));
				var selected = OnSelected;
				if (selected != null)
					selected(this, EventArgs.Empty);
			});
		}
	}

	public static class Camera
	{
		static UIImagePickerController picker;
		static Action<NSDictionary> _callback;

		static void Init()
		{
			if (picker != null)
				return;

			picker = new UIImagePickerController();
			picker.Delegate = new CameraDelegate();
		}

		class CameraDelegate : UIImagePickerControllerDelegate
		{
			public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
			{
				var cb = _callback;
				_callback = null;

				picker.DismissViewController(true, null);
				//picker.PopViewControllerAnimated (true);
				cb(info);
			}
		}

		public static void TakePicture(UIViewController parent, Action<NSDictionary> callback)
		{
			Init();
			picker.SourceType = UIImagePickerControllerSourceType.Camera;
			_callback = callback;
			//parent.PresentModalViewController (picker, true);
			//parent.NavigationController.PushViewController (picker, true);
			((DialogViewController)parent).ActivateController(picker);
		}

		public static void SelectPicture(UIViewController parent, Action<NSDictionary> callback)
		{
			Init();
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			_callback = callback;
			((DialogViewController)parent).ActivateController(picker);
		}
	}
}

