using System;
using CoreGraphics;
using UIKit;
using ZXing.Mobile;

namespace MonoTouch.Dialog
{
    public partial class ScanCodeController : UIViewController
    {
        public ScanCodeController() : base("ScanCodeController", null)
        {
        }

        public async override void ViewDidLoad()
        {
            MobileBarcodeScanner _scanner = new MobileBarcodeScanner(this);

            CustomOverlayScanner overlay = new CustomOverlayScanner(new CGRect(0, 0, View.Frame.Width, View.Frame.Height), "", "", "Cancel", "Flash", () =>
            {
                _scanner.Cancel();
            },
              () => { 
                _scanner.Torch(!_scanner.IsTorchOn); 
                });

            _scanner.UseCustomOverlay = true;
            _scanner.CustomOverlay = overlay;
            _scanner.AutoFocus();
            //  _scanner.Torch(false);

            var result = await _scanner.Scan();

            if (result != null)
            {
                OnSendResponse(result.Text);
                BeginInvokeOnMainThread(() =>
                {
                    if (this.NavigationController != null)
                        this.NavigationController.PopViewController(true);
                });
            }
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public event EventHandler<StringEventArgs> SendResponse;
        private void OnSendResponse(string scanResult)
        {
            if (SendResponse != null)
            {
                SendResponse(this, new StringEventArgs { ScannerResult = scanResult });
            }
        }
        public override void ViewDidDisappear(bool animated)
        {
            BeginInvokeOnMainThread(() => NavigationController.PopViewController(true));
            base.ViewDidDisappear(animated);
        }
        public class StringEventArgs : EventArgs
        {

            public string ScannerResult { get; set; }

        }
    }
}

