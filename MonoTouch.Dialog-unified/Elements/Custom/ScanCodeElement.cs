using System;
namespace MonoTouch.Dialog
{
    public class ScanCodeElement : LoadMoreElement
    {


        public ScanCodeElement(string caption) : base(caption,string.Empty,null)
        {
            
        }


        public override void Selected(DialogViewController dvc, UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            base.Selected(dvc, tableView, indexPath);
            var scan = new ScanCodeController()
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
