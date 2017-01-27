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
					this.Value = UIImage.LoadFromData(NSData.FromArray(Convert.FromBase64String(value)));
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

		public CapturePhotoElement(string caption, string base64value, bool showSelector, string selectorTakePhotoLabel, string selectorPickImageLabel, bool isReadOnly) : this(caption)
		{
			this.Base64Value = base64value;
			this._showSelector = showSelector;
			if (!string.IsNullOrWhiteSpace(selectorPickImageLabel))
				this._selectorPickImageLabel = selectorPickImageLabel;
			if (!string.IsNullOrWhiteSpace(selectorTakePhotoLabel))
				this._selectorTakePhotoLabel = selectorTakePhotoLabel;
			this.IsReadOnly = isReadOnly;
		}
		public CapturePhotoElement(string caption, string base64value) : this(caption, base64value, false, null, null, false)
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
			if (!IsReadOnly)
			{
				if (this.Value != null)
				{
					cell.BackgroundColor = UIColor.FromRGB(1f, 1f, 0.8f);
					cell.ImageView.Image = this.Value;
					//cell.ImageView.Frame.X = 20;
				}
				else
				{ 
					cell.BackgroundColor = UIColor.White;
					cell.ImageView.Image = this.Value;
				}
			}
			else
			{
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				cell.Selected = false;
				cell.ImageView.Image = this.Value;
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
			if (IsReadOnly)
			{

				return;
			}
			else
			{
				var PhotoVC = new PhotoViewController(_selectorPickImageLabel, _selectorTakePhotoLabel)
				{
					Title = Caption
				};
				dvc.ActivateController(PhotoVC);

				PhotoVC.SendResponse += (s, e) =>
				 {
					 //if (e.Value != null)
						 Value = e.Value;
				
					 //OnSendResponse(e.Value);
				 };
			}
		}

		//public event EventHandler<CapturePhotoEventArgs> SendResponse;
		//private void OnSendResponse(UIImage image)
		//{
		//	if (SendResponse != null)
		//	{
		//		SendResponse(this, new CapturePhotoEventArgs { Value = image });
		//	}
		//}

		//public class CapturePhotoEventArgs : EventArgs
		//{

		//	public UIImage Value { get; set; }

		//}


	}
}
