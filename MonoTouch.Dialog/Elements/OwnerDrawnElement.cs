using System;
using System.Drawing;

#if __UNIFIED__
using UIKit;
using CoreFoundation;
using CoreGraphics;
using Foundation;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
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
	public abstract partial class OwnerDrawnElement : Element, IElementSizing
	{		
		public UITableViewCellStyle Style
		{
			get;set;	
		}
		
		public OwnerDrawnElement (UITableViewCellStyle style, string cellIdentifier) : base(null)
		{
			CellKey = (NSString)cellIdentifier;
			this.Style = style;
		}
		
		public nfloat GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return Height(tableView.Bounds);
		}
        public override UITableViewCell GetCell (UITableView tv)
		{
            OwnerDrawnCell cell = tv.DequeueReusableCell(CellKey) as OwnerDrawnCell;

            if (cell == null)
			{
				cell = new OwnerDrawnCell(this, this.Style, CellKey);
			}
			else
			{
				cell.Element = this;
			}
			
			cell.Update();
			return cell;
		}	
		
		public abstract void Draw(CGRect bounds, CGContext context, UIView view);
		
		public abstract nfloat Height(CGRect bounds);
		
		class OwnerDrawnCell : UITableViewCell
		{
			OwnerDrawnCellView view;
			
			public OwnerDrawnCell(OwnerDrawnElement element, UITableViewCellStyle style, string cellReuseIdentifier) : base(style, cellReuseIdentifier)
			{
				Element = element;
			}
			
			public OwnerDrawnElement Element
			{
				get {
					return view.Element;
				}
				set {
					if (view == null)
					{
						view = new OwnerDrawnCellView (value);
						ContentView.Add (view);
					}
					else
					{
						view.Element = value;
					}
				}
			}
				
			

			public void Update()
			{
				SetNeedsDisplay();
				view.SetNeedsDisplay();
			}		
	
			public override void LayoutSubviews()
			{
				base.LayoutSubviews();
				
				view.Frame = ContentView.Bounds;
			}
		}
		
		class OwnerDrawnCellView : UIView
		{				
			OwnerDrawnElement element;
			
			public OwnerDrawnCellView(OwnerDrawnElement element)
			{
				this.element = element;
			}
			
			
			public OwnerDrawnElement Element
			{
				get { return element; }
				set {
					element = value; 
				}
			}
			
			public void Update()
			{
				SetNeedsDisplay();
			
			}
			
			public override void Draw (CGRect rect)
			{
				CGContext context = UIGraphics.GetCurrentContext();
				element.Draw(rect, context, this);
			}
		}
	}
}

