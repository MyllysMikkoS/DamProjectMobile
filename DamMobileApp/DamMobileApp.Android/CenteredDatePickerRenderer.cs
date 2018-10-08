using Android.Content;
using Android.Views;
using DamMobileApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.DatePicker), typeof(CenteredDatePickerRenderer))]
namespace DamMobileApp.Droid
{
    class CenteredDatePickerRenderer : DatePickerRenderer
    {
        public CenteredDatePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Gravity = GravityFlags.Center;
            }
        }
    }
}