// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MonoTouch.Dialog
{
    [Register ("PhotoViewController")]
    partial class PhotoViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SelectPhotoBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton takePicture { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SelectPhotoBtn != null) {
                SelectPhotoBtn.Dispose ();
                SelectPhotoBtn = null;
            }

            if (takePicture != null) {
                takePicture.Dispose ();
                takePicture = null;
            }
        }
    }
}