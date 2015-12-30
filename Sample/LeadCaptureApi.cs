using System;
using System.Collections.Generic;
using System.Text;
using MonoTouch.Dialog;

namespace Sample
{
	public partial class AppDelegate
    {
#if XAMCORE_2_0
        private const string SmallText = "Lorem ipsum dolor sit amet";
        private const string MediumText = "Integer molestie rhoncus bibendum. Cras ullamcorper magna a enim laoreet";
        private const string LargeText = "Phasellus laoreet, massa non cursus porttitor, sapien tellus placerat metus, vitae ornare urna mi sit amet dui.";
        private const string WellINeverWhatAWhopperString = "Nulla mattis tempus placerat. Curabitur euismod ullamcorper lorem. Praesent massa magna, pulvinar nec condimentum ac, blandit blandit mi. Donec vulputate sapien a felis aliquam consequat. Sed sit amet libero non sem rhoncus semper sed at tortor.";
#endif

        public void LeadCaptureApi()
        {
            var root = CreateLeadCaptureApiRoot();

            var dv = new DialogViewController(root, true);
            navigation.PushViewController(dv, true);
        }

        RootElement CreateLeadCaptureApiRoot()
        {
            return new RootElement("Settings") {
				new Section ("Read only"){
					new ReadonlyElement (SmallText, SmallText),
					new ReadonlyElement (MediumText, MediumText),
					new ReadonlyElement (LargeText, LargeText),
					new ReadonlyElement (WellINeverWhatAWhopperString, WellINeverWhatAWhopperString),
				}
            };
        }
    }
}
