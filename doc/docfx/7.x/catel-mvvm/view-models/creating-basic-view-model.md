---
title: "Creating a basic view model" 
---
This example shows how to create a view model without a model. This is useful when a UI item should acknowledge a step that does not need to be persisted to a persistence store. The view model does include validation.

## Code snippets

- vm - declare a view model
- vmprop - declare a property on a view model

## Explanation

When implementing a simple view model without a model, only one property has to be implemented that represents the checkbox that needs to be checked. The example view model declares a single property using the `vmprop` code snippet. Then, a field error is set if the user has not agreed in the `ValidateFields` method.

## Code

**C\#**

```
/// <summary>
/// Simple view model.
/// </summary>
public class SimpleViewModel : ViewModelBase
{
   /// <summary>
   /// Gets the title of the view model.
   /// </summary>
   /// <value>The title.</value>
   public override string Title { get { return "Just acknowledge"; } }

   /// <summary>
   /// Gets or sets whether the user has agreed to continue.
   /// </summary>
   public bool UserAgreedToContinue
   {
      get { return GetValue<bool>(UserAgreedToContinueProperty); }
      set { SetValue(UserAgreedToContinueProperty, value); }
   }

   /// <summary>
   /// Register the UserAgreedToContinue property so it is known in the class.
   /// </summary>
   public static readonly PropertyData UserAgreedToContinueProperty = RegisterProperty("UserAgreedToContinue", typeof(bool));

   /// <summary>
   /// Validates the fields.
   /// </summary>
   protected override void ValidateFields(List<FieldValidationResult> validationResults)
   {
      // Check if the user agrees to continue
      if (!UserAgreedToContinue) 
      {
          validationResults.Add(FieldValidationResult.CreateError(UserAgreedToContinueProperty, "User must agree to continue");
      }
   }
}
```



**XAML (assuming that the view model is set as datacontext)
**

```
<CheckBox Content="Check me to continue" IsChecked="{Binding UserAgreedToContinue, NotifyOnValidationError=True, ValidatesOnDataErrors=True}" />
```

