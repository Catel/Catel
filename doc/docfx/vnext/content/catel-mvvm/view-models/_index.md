---
title: "View models" 
description: ""
---
The view model is a very important part in the MVVM pattern. The view model is responsible for the actual logic that ensures separation of concerns, but also allows unit testing on the view logic (which is implemented in the view model) without actually instantiating the views.

Like almost every other MVVM framework, the base class for all View-Models isÂ `ViewModelBase`. This base class is derived from theÂ `ModelBase`Â class explained earlier in this article, which gives the following advantages:

-   Dependency property a-like property registration;
-   Automatic change notification;
-   Support for field and business errors.

Because the class derives fromÂ `ModelBase`, you can simply add field and business errors that are automatically being reflected to the UI. Writing View-Models has never been so easy!

