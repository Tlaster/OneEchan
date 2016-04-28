using System;

using UIKit;

namespace OneEchan.iOS
{
	public partial class AboutViewController : UIViewController
	{
		public AboutViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			this.OpenGithub.TouchUpInside+= (sender, e) => UIApplication.SharedApplication.OpenUrl(Foundation.NSUrl.FromString("https://github.com/Tlaster/OneEchan"));
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


