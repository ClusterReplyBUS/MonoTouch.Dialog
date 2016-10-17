using System;
namespace MonoTouch.Dialog
{
	public class ProductOfInterestCheckboxElement : TaggedCheckboxElement
	{
		public string ProductGroup { get; set; }

		public ProductOfInterestCheckboxElement(string caption) : base(caption)
		{
		}

		public ProductOfInterestCheckboxElement(string caption, string productGroup)
			: base(caption)
		{
			ProductGroup = productGroup;
		}
	}
}
