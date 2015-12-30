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
        private const string MediumText = "Integer molestie rhoncus bibendum. \nCras ullamcorper magna a enim laoreet";
        private const string LargeText = "Phasellus laoreet, massa non cursus porttitor, \nsapien tellus placerat metus, \nvitae ornare urna mi sit amet dui.";
        private const string WellINeverWhatAWhopperString = "Nulla mattis tempus placerat. \nCurabitur euismod ullamcorper lorem. \nPraesent massa magna, pulvinar nec condimentum ac, \nblandit blandit mi. Donec vulputate sapien a felis \naliquam consequat. Sed sit amet libero non sem \nrhoncus semper sed at tortor.";
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
				new Section(){
					new RightAlignEntryElement (SmallText, SmallText,MediumText),
					new RightAlignEntryElement (MediumText, SmallText,SmallText),
					new RightAlignEntryElement (LargeText, SmallText,SmallText),
					new RightAlignEntryElement (WellINeverWhatAWhopperString, SmallText, SmallText),
				}
            };
        }
    }
}
