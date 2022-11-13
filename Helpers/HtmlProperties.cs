using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace appliedmaths.Helpers
{
  public  class HtmlHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
          "Html",
          typeof(string),
          typeof(HtmlHelper),
          new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebView2))]
        public static string GetHtml(WebView2 d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebView2 d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

       static async void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView2 wb = d as WebView2;
            if (wb != null)
            {
               
                    await wb.EnsureCoreWebView2Async().ConfigureAwait(true);
                    wb.CoreWebView2.NavigateToString(e.NewValue as string);               

                
            }
           
           // wb.NavigateToString(e.NewValue as string);
        }


    }
}
