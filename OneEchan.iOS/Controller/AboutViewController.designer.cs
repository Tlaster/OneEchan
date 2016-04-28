// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace OneEchan.iOS
{
	[Register ("AboutViewController")]
	partial class AboutViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton OpenGithub { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (OpenGithub != null) {
				OpenGithub.Dispose ();
				OpenGithub = null;
			}
		}
	}
}
