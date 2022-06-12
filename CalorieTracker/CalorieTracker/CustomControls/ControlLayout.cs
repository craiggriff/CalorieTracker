using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SmartControls
{
	public class ControlLayout : StackLayout
	{
		public ControlLayout()
		{
            Spacing = 3;
            Margin = 3;
            BackgroundColor = Color.White;
		}
	}
}