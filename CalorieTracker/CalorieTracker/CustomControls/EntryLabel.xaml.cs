using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartControls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EntryLabel : StackLayout
	{
		public EntryLabel ()
		{
			InitializeComponent ();

            the_entry.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
        }
        public string text { get { return the_entry.Text; } set { the_entry.Text = value; } }

        public string TextBinding { set { the_entry.SetBinding(Entry.TextProperty, value); } }
        public string LabelText   { set { the_label.Text = value; } }
        public Keyboard EntryKeyboard { set { the_entry.Keyboard = value; } }
        public int max_text_length { set { the_entry.MaxLength = value; } }

    }
}