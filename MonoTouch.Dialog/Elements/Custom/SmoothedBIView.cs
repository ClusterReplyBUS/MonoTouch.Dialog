using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

	public class SmoothedBIView : UIView
	{
		UIBezierPath Path {
			get;
			set;
		}

		public UIImage IncrementalImage{
			get {
				if (_incrementalImage == null)
					_incrementalImage = new UIImage ();
				return _incrementalImage;
			} set {
				_incrementalImage = value; 
			}
		}

		PointF[] Pts {
			get {
				if (_pts == null)
					_pts = new PointF[10];
				return _pts;
			}
		}

		UIImage _incrementalImage;
		PointF[] _pts;
		int _ctr;

		public SmoothedBIView (NSCoder aDecoder)
		{
			//            this.SetMultipleTouchEnabled(false);
			MultipleTouchEnabled = false;
			//            path = UIBezierPath.BezierPath();
			Path = new UIBezierPath ();
			//            path.SetLineWidth(2.0);
			Path.LineWidth = 3.0f;
		}

		public SmoothedBIView (RectangleF frame) : base (frame)
		{
			//            this.SetMultipleTouchEnabled(false);
			MultipleTouchEnabled = false;
			//            path = UIBezierPath.BezierPath();
			Path = new UIBezierPath ();
			//            path.SetLineWidth(2.0);
			Path.LineWidth = 4.0f;
			this.BackgroundColor = UIColor.FromWhiteAlpha(1f,0f);
		}

//
//        void DrawRect(RectangleF rect)
//        {
//            incrementalImage.DrawInRect(rect);
//            path.Stroke();
//        }
//
		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
			//PaintBackground (this.BackgroundColor);
			if (_incrementalImage != null)
				IncrementalImage.Draw (rect);
			Path.Stroke ();
		}
        
//		void TouchesBeganWithEvent (NSSet touches, UIEvent theEvent)
//		{
//			_ctr = 0;
//			UITouch touch = touches.AnyObject ();
//			Pts [0] = touch.LocationInView (this);
//		}
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			_ctr = 0;
			UITouch touch = touches.AnyObject as UITouch;
			Pts [0] = touch.LocationInView (this);
		}

//        void TouchesMovedWithEvent(NSSet touches, UIEvent theEvent)
//        {
//            UITouch touch = touches.AnyObject();
//            CGPoint p = touch.LocationInView(this);
//            _ctr++;
//            Pts[_ctr] = p;
//            if (_ctr == 4) {
//                Pts[3] = CGPointMake((Pts[2].X + Pts[4].X) / 2.0, (Pts[2].Y + Pts[4].Y) / 2.0);
//                path.MoveToPoint(Pts[0]);
//                path.AddCurveToPointControlPoint1ControlPoint2(Pts[3], Pts[1], Pts[2]);
//                this.SetNeedsDisplay();
//                Pts[0] = Pts[3];
//                Pts[1] = Pts[4];
//                _ctr = 1;
//            }
//
//        }
		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
			UITouch touch = touches.AnyObject as UITouch;
			PointF p = touch.LocationInView (this);
			_ctr++;
			Pts [_ctr] = p;
			if (_ctr == 4) {
				Pts [3] = new PointF ((float)((Pts [2].X + Pts [4].X) / 2.0), (float)((Pts [2].Y + Pts [4].Y) / 2.0));
				Path.MoveTo (Pts [0]);
				Path.AddCurveToPoint (Pts [3], Pts [1], Pts [2]);
				this.SetNeedsDisplay ();
				Pts [0] = Pts [3];
				Pts [1] = Pts [4];
				_ctr = 1;
			}
		}

//		void TouchesEndedWithEvent (NSSet touches, UIEvent theEvent)
//		{
//			this.DrawBitmap ();
//			this.SetNeedsDisplay ();
//			path.RemoveAllPoints ();
//			_ctr = 0;
//		}
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			this.DrawBitmap ();
			this.SetNeedsDisplay ();
			Path.RemoveAllPoints ();
			_ctr = 0;
		}

//		void TouchesCancelledWithEvent (NSSet touches, UIEvent theEvent)
//		{
//			this.TouchesEndedWithEvent (touches, theEvent);
//		}
		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
			this.TouchesEnded (touches, evt);
		}

		void PaintBackground(UIColor color)
		{
			UIBezierPath rectPath = UIBezierPath.FromRect (this.Bounds);
			color.SetFill ();
			rectPath.Fill ();
		}
		void DrawBitmap ()
		{
//			UIGraphicsBeginImageContextWithOptions (this.Bounds.Size, true, 0.0);
			UIGraphics.BeginImageContextWithOptions (this.Bounds.Size, false, 0.0f);
//			if (!incrementalImage) {
//				UIBezierPath rectpath = UIBezierPath.BezierPathWithRect (this.Bounds);
//				(UIColor.WhiteColor ()).SetFill ();
//				rectpath.Fill ();
//			}
//			if (_incrementalImage == null)
//				PaintBackground (this.BackgroundColor);
			
//			incrementalImage.DrawAtPoint (CGPointZero);
			IncrementalImage.Draw (new PointF (0f, 0f));
//			(UIColor.BlackColor ()).SetStroke ();
			UIColor.Black.SetStroke ();
//			path.Stroke ();
			Path.Stroke ();
//			incrementalImage = UIGraphicsGetImageFromCurrentImageContext ();
			IncrementalImage = UIGraphics.GetImageFromCurrentImageContext ();
//			UIGraphicsEndImageContext ();
			UIGraphics.EndImageContext ();
		}

	}
