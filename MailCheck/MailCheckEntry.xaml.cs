using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MailCheck
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MailCheckEntry : ContentView
    {
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
                                                 propertyName: "Placeholder",
                                                 returnType: typeof(string),
                                                 declaringType: typeof(MailCheckEntry),
                                                 defaultValue: "",
                                                 defaultBindingMode: BindingMode.TwoWay,
                                                 propertyChanged: PlaceholderPropertyChanged);

        public string Placeholder
        {
            get { return base.GetValue(PlaceholderProperty).ToString(); }
            set { base.SetValue(PlaceholderProperty, value); }
        }

        private static void PlaceholderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MailCheckEntry)bindable;
            control.entry.Placeholder = newValue.ToString();
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                         propertyName: "Text",
                                         returnType: typeof(string),
                                         declaringType: typeof(MailCheckEntry),
                                         defaultValue: "",
                                         defaultBindingMode: BindingMode.TwoWay,
                                         propertyChanged: TextPropertyChanged);

        MailCheck mailCheck;
        string mailCheckResponse { get; set; }

        public string Text
        {
            get { return base.GetValue(TextProperty).ToString(); }
            set { base.SetValue(TextProperty, value); }
        }

        private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MailCheckEntry)bindable;
            if (newValue != null)
                control.entry.Text = newValue.ToString();
        }

        public MailCheckEntry()
        {
            InitializeComponent();

            if (mailCheck == null)
                mailCheck = new MailCheck();

            entry.TextChanged += (s, e) => CheckEmail();

            var emailHelperLabelGestureRecognizer = new TapGestureRecognizer();
            emailHelperLabelGestureRecognizer.Tapped += (s, e) =>
            {
                if (!String.IsNullOrEmpty(mailCheckResponse))
                    entry.Text = mailCheckResponse;

                label.IsVisible = false;
            };

            label.GestureRecognizers.Add(emailHelperLabelGestureRecognizer);
        }

        private void CheckEmail()
        {
            if (mailCheck == null)
                return;

            if (!String.IsNullOrEmpty(entry.Text) && entry.Text.Length <= 3)
            {
                string result = mailCheck.Run(entry.Text, null, null);

                if (!String.IsNullOrEmpty(result))
                {
                    // Let's make it visible before we even do any tests. 
                    label.IsVisible = true;

                    mailCheckResponse = result;

                    label.Text = $"Did you mean: {result}?";
                }
                else
                {
                    label.IsVisible = false;
                }
            }
        }

        //

    }

}