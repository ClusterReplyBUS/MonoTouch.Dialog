using System;
using UIKit;

namespace MonoTouch.Dialog
{
    public class ScanCodeElement : LoadMoreElement
    {
        string cancelLabel;
        string flashLabel;

        public ScanCodeElement(string caption,string cancelLabel,string flashLabel,string flashOnLabel,string flashOffLabel) : base(caption,string.Empty,null)
        {
            this.flashLabel = flashLabel;
            this.cancelLabel = cancelLabel;
            
        }


        public override void Selected(DialogViewController dvc, UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            base.Selected(dvc, tableView, indexPath);

            var scan = new ScanCodeController(cancelLabel, flashLabel)
            {
             };
            this.Animating = false;

            scan.SendResponse += (s, e) =>
            {
                OnSendResponse(e.ScannerResult);
            };
             dvc.ActivateController(scan);

        }



        public override UIKit.UITableViewCell GetCell(UIKit.UITableView tv)
        {
            return base.GetCell(tv);
        }

        public event EventHandler<StringEventArgs> SendResponse;
        private void OnSendResponse(string scanResult)
        {
            if (SendResponse != null)
            {
                SendResponse(this, new StringEventArgs{ScannerResult=scanResult});
            }
        }

        public class StringEventArgs : EventArgs
        {

            public string ScannerResult { get; set; }

        }

    }
}
