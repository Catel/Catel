---
title: "Introduction to MVVM and models" 
---
This part of the documentation will explain all the parts of MVVM, in the order in which we think they must be built. First of all, the Models, which are the closest to the business. Then, the View Models which define what part of the Models should be visible to the user in a specific situation. This also includes validation that is specific for the functionality that the View Model represents. Last, but not least, the View itself, which is the final representation of the View Model to the end-user.

As you will notice (or maybe you do not), this framework has a lot in common with other MVVM frameworks out there. This is normal because all of the frameworks are trying to implement the same pattern, which is not that hard to understand if you think about it long enough. Before we started writing the MVVM framework, we first investigated other frameworks because there already are enough, probably too many. However, even the better (or best) still took too much time to use, and there was too much code we had to write to create a View Model. That is the point where we decided to write our own framework that contains many luxury for lazy developers such as us.

## Models

The Models part is one of the three major parts of MVVM. Therefore, I want to tell you somewhat about what kind of Models we use to store our data. Basically, you can use all types of objects as Models, as long as the Models implement the most commonly used interfaces.

For MVVM, it is important that the following interfaces are implemented:

- **INotifyPropertyChanged**
 If this interface is not implemented, changes will not be reflected to the UI via bindings. In other words, your Model and View Model will be useless in an MVVM setting.

Finally, it is strongly recommended to have your Models implement the following interfaces as well:

- **IDataErrorInfo**
 If this interface is not implemented, errors cannot be shown to the user.
- **IEditableObject**
 If this interface is not implemented, a Model cannot work with “states”. This means that a user cannot start editing an object and finally cancel it (because there is no stored “state” that can be used to restore the values). Note that `ModelBase` does not implement `IEditableObject` out of the box; if you need this behavior, implement it in a derived class.

## View Models

View models are the classes that contain the view logic. They can be seen as a container for all models in a specific view, including the behavior to interact with these models (for example adding or removing them from a collection). It is important that a view model also implements the *INotifyPropertyChanged* interfaces just like the models do.

