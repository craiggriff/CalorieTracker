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
    public partial class DateLabel : Xamarin.Forms.StackLayout
    {
        bool bLoaded = false;
        public delegate void OnSelectionHasChanged(object sender, EventArgs e);

        public static readonly BindableProperty TheDateProperty = BindableProperty.Create("TheDate", typeof(string), typeof(DateLabel), default(string), BindingMode.TwoWay);

        public string TheDate
        {
            get { return (string)GetValue(TheDateProperty); }
            set { SetValue(TheDateProperty, value); }
        }
        public DateLabel()
        {
            InitializeComponent();
        }
        public void SetDateValue()
        {
            if (TheDate != null && TheDate != "")
            {
                TheDateControl.Date = DateTime.Parse(TheDate);
            }
            bLoaded = true;
        }
        public void SetDateText()
        {
            TheDate = this.TheDateControl.Date.ToShortDateString();
        }
        public string LabelText
        {
            get { return TheLabel.Text; }
            set { TheLabel.Text = value; }
        }
        private void OnDatechanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (bLoaded == true)
            {
                SetDateText();
            }
        }
        private void layout_changed(object sender, EventArgs e)
        {
            SetDateValue();
        }
    }
}