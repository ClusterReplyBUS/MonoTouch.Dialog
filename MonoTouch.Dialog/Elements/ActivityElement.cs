using System;
using System.Drawing;

#if __UNIFIED__
using UIKit;
using CoreGraphics;
using Foundation;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
#endif

#if !__UNIFIED__
using nint = global::System.Int32;
using nuint = global::System.UInt32;
using nfloat = global::System.Single;

using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;
using CGRect = global::System.Drawing.RectangleF;
#endif


namespace MonoTouch.Dialog
{
	public class ActivityElement : Element {
		public ActivityElement () : base ("")
		{
		}

		UIActivityIndicatorView indicator;

		public bool Animating {
			get {
				return indicator.IsAnimating;
			}
			set {
				if (value)
					indicator.StartAnimating ();
				else
					indicator.StopAnimating ();
			}
		}

		static NSString ikey = new NSString ("ActivityElement");

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

   //         if (cell == null){
			//	cell = new UITableViewCell (UITableViewCellStyle.Default, CellKey);
			//}

			indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
			var sbounds = tv.Frame;
			var vbounds = indicator.Bounds;

			indicator.Frame = new CGRect((sbounds.Width-vbounds.Width)/2, 12, vbounds.Width, vbounds.Height);
			indicator.StartAnimating ();

			cell.Add (indicator);

			return cell;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing){
				if (indicator != null){
					indicator.Dispose ();
					indicator = null;
				}
			}
			base.Dispose (disposing);
		}
	}
}

