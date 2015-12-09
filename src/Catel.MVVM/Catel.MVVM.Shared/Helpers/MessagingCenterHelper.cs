using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

#if XAMARIN_FORMS

namespace Catel
{
    public static class MessagingCenterHelper
    {
        public static void Send(Type typeOfSender, object sender, string message, Type typeOfArgs, object args)
        {
            Argument.IsOfType(() => sender, typeOfSender);
            Argument.IsOfType(() => args, typeOfArgs);

            var type = typeof (MessagingCenter);
            //// TODO: Use reflection API instead but reflection API requires some fixes.
            var methodInfo =
                type.GetRuntimeMethods()
                    .FirstOrDefault(
                        info =>
                            info.Name == "Send" && info.GetGenericArguments().Length == 2 &&
                            info.GetParameters().Length == 3);
            var makeGenericMethod = methodInfo.MakeGenericMethod(typeof (Page), typeOfArgs);
            makeGenericMethod.Invoke(type, new[] {sender, message, args});
        }
    }
}

#endif