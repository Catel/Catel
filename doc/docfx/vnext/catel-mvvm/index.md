---
title: "Catel.MVVM" 
---
The last few years, MVVM has become the number one pattern to write applications using WPF and other XAML related platforms. The actual pattern is very simple, but there are some flaws and questions lots of MVVM users have, such as:

-   How to show modal dialogs or message boxes inside a View-Model?
-   How to run processes inside a View-Model?
-   How to let the user select a file inside a View-Model?

In my opinion, this is where the good frameworks separate themselves from the bad ones. For example, people actually calling MessageBox.Show inside a View-Model are using the pattern wrong. If you are one of the developers that directly call a MessageBox inside a View-Model, ask yourself this: who is going to click the button during a unit test?

Before we actually started developing Catel, we did lots of investigations to make sure that the MVVM pattern was really useful in Line of Business (LoB) applications and does not miss the finishing touch. Thanks to this investigation and research, we created a solid MVVM framework which solves all the known problems of the MVVM pattern.

This part of the documentation explains all about the MVVM framework included with Catel. The MVVM framework that ships with Catel has the following characteristics and features:

-   Very easy to use, a view model is created within 10 minutes
-   Direct pass-through of view model properties to Models
-   Validation mapping from model to view model and back
-   Solves the nested user controls problem

