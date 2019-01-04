using System;
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

namespace MonoTouch.Dialog
{
	public partial class StaticImageElement : Element,IElementSizing
	{

		#region PRIVATE MEMBERS
		UIImage _value;
		UIImage _scaled;
		int _height;
		int _width;
		#endregion
		
		#region PUBLIC PROPERTIES
		public UIImage Value {
			get {
				if (_value == null)
					_value = MakeEmpty ();
				return _value;
			}
		}

		public UIImage Scaled {
			get {
				if (_scaled == null)
					_scaled = Value.Scale (new CGSize (_width, _height));
				return _scaled;
			}
		}
		#endregion
		
		#region PRIVATE STATIC MEMBERS
		static NSString ikey = new NSString ("StaticImageElement");
		#endregion
		
		#region CONSTS
		// radius for rounding
		const int rad = 10;
		#endregion
		
		#region PRIVATE METHODS
		UIImage MakeEmpty ()
		{
			using (var cs = CGColorSpace.CreateDeviceRGB ()) {
				using (var bit = new CGBitmapContext (IntPtr.Zero, _width, _height, 8, 0, cs, CGImageAlphaInfo.PremultipliedFirst)) {
					bit.SetStrokeColor (1, 0, 0, 0.5f);
					bit.FillRect (new CGRect (0, 0, _width, _height));
					
					return UIImage.FromImage (bit.ToImage ());
				}
			}
		}
		
		#endregion
		
		#region CTOR
		public StaticImageElement (string caption, UIImage image, int height, int width) : base (caption)
		{
			_width = width;
			_height = height;
			_value = image;
		}
		public StaticImageElement (string caption, UIImage image, int height) : base (caption)
		{
			_width = (int)(height * image.Size.Width / image.Size.Height);
			_height = height;
			_value = image;
		}
		#endregion
		
		#region IElementSizing implementation
		public virtual nfloat GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return _height + 10;
		}
		#endregion
		
		#region OVERRIDE METHODS
		protected override NSString CellKey {
			get {
				return ikey;
			}
		}
        protected override UITableViewCellStyle CellStyle => UITableViewCellStyle.Default;
        public override UITableViewCell GetCell (UITableView tv)
		{
            //var cell = tv.DequeueReusableCell (CellKey);
            var cell = base.GetCell(tv);

   //         if (cell == null) {
			//	cell = new UITableViewCell (UITableViewCellStyle.Default, CellKey);
			//}
			
			cell.TextLabel.Text = Caption;
			cell.AccessoryView = new UIImageView (Scaled);
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
//			Section psection = Parent as Section;
//			bool roundTop = psection.Elements [0] == this;
//			bool roundBottom = psection.Elements [psection.Elements.Count - 1] == this;
//			
//			using (var cs = CGColorSpace.CreateDeviceRGB ()) {
//				using (var bit = new CGBitmapContext (IntPtr.Zero, _width, _height, 8, 0, cs, CGImageAlphaInfo.PremultipliedFirst)) {
//					// Clipping path for the image, different on top, middle and bottom.
//					if (roundBottom) {
//						bit.AddArc (
//							rad,
//							rad,
//							rad,
//							(float)Math.PI,
//							(float)(3 * Math.PI / 2),
//							false
//						);
//					} else {
//						bit.MoveTo (0, rad);
//						bit.AddLineToPoint (0, 0);
//					}
//					bit.AddLineToPoint (_width, 0);
//					bit.AddLineToPoint (_width, _height);
//					
//					if (roundTop) {
//						bit.AddArc (
//							rad,
//							_height - rad,
//							rad,
//							(float)(Math.PI / 2),
//							(float)Math.PI,
//							false
//						);
//						bit.AddLineToPoint (0, rad);
//					} else {
//						bit.AddLineToPoint (0, _height);
//					}
//					bit.Clip ();
//					bit.DrawImage (Rect, Scaled.CGImage);
//					
//					cell.ImageView.Image = UIImage.FromImage (bit.ToImage ());
//				}
//			}			
			return cell;
		}

		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (_scaled != null) {
					Scaled.Dispose ();
					_scaled = null;
				}	
				if (_value != null) {
					Value.Dispose ();
					_value = null;
				}
			}
			base.Dispose (disposing);
		}
		
		#endregion

		#region STATIC METHODS

		public static UIImage MakeTransparent(int width, int height)
		{
			using (var cs = CGColorSpace.CreateDeviceRGB ()) {
				using (var bit = new CGBitmapContext (IntPtr.Zero, width, height, 8, 0, cs, CGImageAlphaInfo.PremultipliedFirst)) {
					//bit.SetStrokeColor (0, 0, 0, 0);
					//bit.FillRect (new CGRect (0, 0, _width, _height));

					return UIImage.FromImage (bit.ToImage ());
				}
			}
		}
		#endregion
	}
}

