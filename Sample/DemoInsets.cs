using System;
using MonoTouch.Dialog;
<<<<<<< HEAD
#if XAMCORE_2_0
using UIKit;
using CoreGraphics;
using Foundation;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
=======
#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
>>>>>>> migueldeicaza/master
#endif
using System.Drawing;

namespace Sample
{
	public partial class AppDelegate 
	{		
		public void DemoInsets ()
		{
			var uiView = new UIViewElement ("", new UIView (new RectangleF (0, 0, 20, 20)){
				BackgroundColor = UIColor.Red
			}, false);
			
			var root = new RootElement ("UIViewElement Inset"){
				new Section ("Simple Rectangle"){
					uiView,
					new StringElement ("Pad Left", delegate { var i = uiView.Insets; i.Left += 10; uiView.Insets = i; }),
					new StringElement ("Pad Top", delegate { var i = uiView.Insets; i.Top += 10; uiView.Insets = i; }),
					new StringElement ("Pad Bottom", delegate { var i = uiView.Insets; i.Bottom += 10; uiView.Insets = i; })
				}
			};
						
			var dv = new DialogViewController (root, true);
			navigation.PushViewController (dv, true);				
		}
	}
}

