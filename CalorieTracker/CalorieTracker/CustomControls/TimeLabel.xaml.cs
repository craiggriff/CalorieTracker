using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeLabel : Xamarin.Forms.StackLayout
    {
        bool bLoaded = false;
        public delegate void OnSelectionHasChanged(object sender, EventArgs e);
        public static readonly BindableProperty TheTimeProperty = BindableProperty.Create("TheTime", typeof(string), typeof(TimeLabel), default(string), BindingMode.TwoWay);
        public string TheTime
        {
            get { return (string)GetValue(TheTimeProperty); }
            set { SetValue(TheTimeProperty, value); }
        }
        public string TimeBinding { set { TheLabel.SetBinding(Label.TextProperty, value); } }
        public TimeLabel ()
		{
			InitializeComponent ();
        }
        public void SetTimeValue()
        {
            if (TheTime != null && TheTime!="00/00/00" && TheTime != "00/00")
            {
                TheTimeControl.Time = TimeSpan.Parse(TheTime);
                bLoaded = true;
            }
        }
        public void SetTimeText()
        {
            TheTime = this.TheTimeControl.Time.ToString().Substring(0,5);
        }
        public string LabelText
        {
            get { return TheLabel.Text; }
            set { TheLabel.Text = value; }
        }
        private void OnTimechanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (bLoaded == true)
            {
                SetTimeText();
            }
        }
        private void layout_changed(object sender, EventArgs e)
        {
            SetTimeValue();
        }
    }
}