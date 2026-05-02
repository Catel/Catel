---
title: "Creating a view model with a model" 
---
This example shows how to create a "classical" view model without any Catel specific MVVM features such as data pass-through. Although it is recommended to use the pass-through features, some people want to have custom validation on the view model, or want to be fully in control.

## Code snippets

-   vm - declare a view model
-   vmprop - declare a property on a view model

## Explanation

To be in full control, the only thing required is to create a basic view model with the `vm` code snippet. Then, the following methods should be implemented:

-   Constructor - initialize the properties on the view model
-   ValidateFields - check for field errors in the view model
-   ValidateBusinessRules - check for business rules in the view model
-   Save - save the view model data to the model and then save the model

## Code

**C\#**

```
/// <summary>
/// Classical view model.
/// </summary>
public class ClassicalViewModel : ViewModelBase
{
   #region Properties
   /// <summary>
   /// Gets or sets the Person.
   /// </summary>
   private Person Person
   {
      get { return GetValue<Person>(PersonProperty); }
      set { SetValue(PersonProperty, value); }
   }

   /// <summary>
   /// Register the Person property so it is known in the class.
   /// </summary>
   public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));

   /// <summary>
   /// Gets or sets the first name.
   /// </summary>
   public string FirstName
   {
      get { return GetValue<string>(FirstNameProperty); }
      set { SetValue(FirstNameProperty, value); }
   }

   /// <summary>
   /// Register the FirstName property so it is known in the class.
   /// </summary>
   public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));

   /// <summary>
   /// Gets or sets the last name.
   /// </summary>
   public string LastName
   {
      get { return GetValue<string>(LastNameProperty); }
      set { SetValue(LastNameProperty, value); }
   }

   /// <summary>
   /// Register the LastName property so it is known in the class.
   /// </summary>
   public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string));
   #endregion

   #region Methods
   /// <summary>
   /// Initializes the object by setting default values.
   /// </summary>   
   protected override void Initialize()
   {
      // Get the person from (in this case) a magical context
      Person = Context.CurrentPerson;
Â 
      // Load the data manually to the view model
      FirstName = Person.FirstName;
      LastName = Person.LastName;
   }

   /// <summary>
   /// Validates the field values of this object. Override this method to enable
   /// validation of field values.
   /// </summary>
   /// <param name="validationResults">The validation results, add additional results to this list.</param>
   protected override void ValidateFields(List<FieldValidationResult> validationResults)
   {
       if (string.IsNullOrWhiteSpace(FirstName))
       {
           validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "First name is required"));
       }
Â 
       if (string.IsNullOrWhiteSpace(LastName))
       {
           validationResults.Add(FieldValidationResult.CreateError(LastNameProperty, "Last name is required"));
       }
   }

   /// <summary>
   /// Saves the data.
   /// </summary>
   /// <returns>
   ///    <c>true</c> if successful; otherwise <c>false</c>.
   /// </returns>   
   protected override Task<bool> Save()
   {
      return Task.Factory.StartNew(() =>
      {
          // Save the data manually to the model
          Person.FirstName = FirstName;
          Person.LastName = LastName;
Â 
          // Save the model
          return Person.Save();
      });
   }
   #endregion
}
```

