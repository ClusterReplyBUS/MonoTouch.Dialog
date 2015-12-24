using System;

namespace MonoTouch.Dialog
{
	public class ProductOfInterestRadioElement : TaggedRadioElement
	{
		public string ProductGroup { get; set; }

		public ProductOfInterestRadioElement (string caption):base(caption)
		{
		}

		public ProductOfInterestRadioElement (string caption, string productGroup):base(caption)
		{
			ProductGroup = productGroup;
		}
	}
}

