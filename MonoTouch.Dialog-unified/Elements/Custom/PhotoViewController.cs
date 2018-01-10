using System;
using Foundation;
using UIKit;

namespace MonoTouch.Dialog
{
	public partial class PhotoViewController : UIViewController
	{
		UIBarButtonItem btnClear;
		private string _BtnSelectPhoto;
		private string _BtnTakePicture;
		private string _deleteButtonLabel;
		

		public PhotoViewController(string BtnSelectPhoto, string BtnTakePicture,string DeleteButtonLabel) : base("PhotoViewController", null)
		{
			this._BtnSelectPhoto = BtnSelectPhoto;
			this._BtnTakePicture = BtnTakePicture;
			this._deleteButtonLabel = DeleteButtonLabel;

		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			btnClear = new UIBarButtonItem(_deleteButtonLabel, UIBarButtonItemStyle.Done,(object sender, EventArgs e) =>
				   {
					   OnSendResponse(null);
					   BeginInvokeOnMainThread(() => NavigationController.PopViewController(true));
				
				   });
			this.NavigationItem.SetRightBarButtonItem(btnClear,false);
		
			SelectPhotoBtn.SetTitle(_BtnSelectPhoto, UIControlState.Normal);

			SelectPhotoBtn.TouchUpInside += (sender, e) =>
			  {
				  Camera.SelectPicture(this, (obj) =>
				  {
					  var photo = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
					  OnSendResponse(photo);
					  BeginInvokeOnMainThread(() => NavigationController.PopViewController(true));

				  });
			  };

			takePicture.SetTitle(_BtnTakePicture, UIControlState.Normal);
			takePicture.TouchUpInside += (sender, e) =>
			 {
				 TakePhoto(this);
			 };

			//NavigationItem.RightBarButtonItem = new UIBarButtonItem();

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public event EventHandler<CapturePhotoEventArgs> SendResponse;
		private void OnSendResponse(UIImage image)
		{
			//if (SendResponse != null)
			//{
			SendResponse(this, new CapturePhotoEventArgs { Value = image });
			//}
		}

		public class CapturePhotoEventArgs : EventArgs
		{

			public UIImage Value { get; set; }

		}

		private void TakePhoto(UIViewController uvc)
		{
			Camera.TakePicture(uvc, (obj) =>
			{
				var photo = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
		//Value = photo;
		//Value = photo.Scale(new CGSize(this.newHeight * photo.Size.Width / photo.Size.Height, this.newHeight));
		OnSendResponse(photo);
				BeginInvokeOnMainThread(() => NavigationController.PopViewController(true));
			});
		}
	}



	public static class Camera
	{
		static UIImagePickerController picker;
		static Action<NSDictionary> _callback;
		public static UIViewController _parent = null;

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
				cb(info);
			}
		}

		public static void TakePicture(UIViewController parent, Action<NSDictionary> callback)
		{
			Init();
			_parent = parent;
			picker.SourceType = UIImagePickerControllerSourceType.Camera;
			_callback = callback;
			//parent.PresentModalViewController (picker, true);
			//parent.NavigationController.PushViewController (picker, true);
			//((DialogViewController)parent).ActivateController(picker);
			parent.PresentViewController(picker, true, null);


		}

		public static void SelectPicture(UIViewController parent, Action<NSDictionary> callback)
		{
			Init();
			_parent = parent;
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			_callback = callback;
			//((DialogViewController)parent).ActivateController(picker);
			parent.PresentViewController(picker, true, null);
		}



	}
}

