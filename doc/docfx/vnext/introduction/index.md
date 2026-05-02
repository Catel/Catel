---
title: "Introduction" 
---
Welcome to the introduction of Catel. Catel is a  framework (or enterprise library, just use whatever you like) with data handling, diagnostics, logging, WPF controls, and an MVVM-framework. So Catel is more than "just" another MVVM-framework or some nice Extension Methods that can be used. It's more like a library that you want to include in all the XAML applications you are going to develop in the near future.

Catel is primarily meant for Line of Business (LoB) applications

It's important to realize that Catel is not just another Extension Methods library, nor only an MVVM-framework, but it is a combination of basic data handling, useful controls, and an MVVM-framework.

## Why another framework?

You might be thinking: why another framework, there are literally thousands of them out there. Well, first of all, thousands of them is quite a lot, let's just say there are hundreds of them. A few years ago, the lead developer of Catel was using serialization to serialize data from/to disk. But, as he noticed, he had to take care of different versions after every release. After every release, he had to take care of the serialization and backwards compatibility. Also, he had to implement some very basic interfaces (such as INotifyPropertyChanged) for every data object. Then, he decided to write a base class for data handling which can take care of different versions and serialization by itself, and implements the most basic interfaces of the .NET Framework out of the box. The article was published on CodeProject as DataObjectBase.

Then, he was working on a WPF project with five other developers and needed an MVVM-framework since the application was not using MVVM at the moment. Writing an MVVM-framework was no option because there were so many other frameworks out there. But, after looking at some Open-Source MVVM-frameworks (such as the excellent Cinch framework, which was the best one we could find), none of them seemed to be a real option. Creating the View Models was too much work, and the View Models still contained lots of repetitive code in, for example, the property definitions. After taking a closer look at the source code of Cinch and other frameworks, the lead developer thought: if we use the DataObjectBase published before as the base for a View Model class, it should be possible to create a framework in a very short amount of time.

Then, all other developers of the team he was working on the project got enthusiastic, and then the whole team decided to merge their personal libraries into one big enterprise library, and Catel was born.

## Why use this framework?

Before reading any further, it's important to know why you should use the framework. Below are a few reasons why Catel might be interesting for you:

- Catel is Open-Source. This way, you can customize it any way you want. If you want a new feature request, and the team does not respond fast enough, you can simply implement it yourself.
- The codebase for Catel is available on GitHub. This way, you have the option to either download the latest stable release, or live on the edge by downloading the latest source code.
- Catel uses unit tests to make sure that new updates do not break existing functionality.
- Catel is very well documented. Every method and property has comments, and the comments are available in a separate reference help file. There is also a lot of documentation available, and in the future, in-depth articles will be written.
- Catel is developed by a group of talented software developers, and is heavily under development. This is a great advantage because the knowledge is not at just one person, but at a whole group. The developers of Catel all have more than three years of development experience with WPF, and are using Catel in real life applications for at least 2 years.

