---
title: "Catel.ReSharper" 
---
Catel.ReSharper is a ReSharper plugin that helps with development in the following
fields:

* Make classes to inherits from ModelBase or ViewModelBase
* Convert automatic properties to Catel properties
* Expose view model properties as view model ones
* Check for arguments using the Argument class

_Basically convert this:_

    /// <summary>
    ///     The person model.
    /// </summary>
    public class Person 
    {
      #region Public Properties

      /// <summary>
      ///     Gets or sets the first name.
      /// </summary>
      public string FirstName { get; set; }

      /// <summary>
      ///     Gets or sets the last name.
      /// </summary>
      public string LastName { get; set; }

      /// <summary>
      ///     Gets or sets the age.
      /// </summary>
      public int Age { get; set; }

      #endregion
    }

_into this:_

    /// <summary>
    ///     The person model.
    /// </summary>
    public class Person : ModelBase
    {
        #region Static Fields

        /// <summary>Register the FirstName property so it is known in the class.</summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty<Person, string>(model => model.FirstName);

        /// <summary>Register the LastName property so it is known in the class.</summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty<Person, string>(model => model.LastName, default(string), (s, e) => s.OnLastNameChanged());

        /// <summary>Register the Age property so it is known in the class.</summary>
        public static readonly PropertyData AgeProperty = RegisterProperty<Person, int>(model => model.Age, default(int), (s, e) => s.OnAgeChanged(e));

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return this.GetValue<string>(FirstNameProperty); }
            set { this.SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get { return this.GetValue<string>(LastNameProperty); }
            set { this.SetValue(LastNameProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the age.
        /// </summary>
        public int Age
        {
            get { return this.GetValue<int>(AgeProperty); }
            set { this.SetValue(AgeProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when the value of the Age property is changed.
        /// </summary>
        /// <param name="e">
        /// The event argument
        /// </param>
        private void OnAgeChanged(AdvancedPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Occurs when the value of the LastName property is changed.
        /// </summary>
        private void OnLastNameChanged()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

_with pleasure!_

## Checking arguments of a method

If you are not using the *Argument* class, you are definitely missing something! It allows you to check for a method input and make sure it is valid. So, instead of writing this:

	public void DoSomething(string myInput)
	{
	    if (string.IsNullOrWhitespace(myInput)
	    {
	        Log.Error("Argument 'myInput' cannot be null or whitespace");
	        throw new ArgumentException("Argument 'myInput' cannot be null or whitespace", "myInput");
	    }

	    // custom logic
	}

You can write this:

	public void DoSomething(string myInput)
	{
	    Argument.IsNotNullOrWhitespace(() => myInput);

	    // custom logic
	}

However, when you are writing lots of code, then even this piece of code can be too much. Thanks to the* Catel.Resharper* plugin, it is possible to select the argument (in this case myInput), hit ALT + Enter and generate the code.

## Converting regular properties into Catel properties

to start of metadata

Catel is extremely powerful, but sometimes the property definitions are lots of work to write down. The code snippets already make your life much easier, but with the Catel.Resharper plugin it might be even easier. You can simply write this code:

	public class Person : ModelBase
	{
	    public string FirstName { get; set; }
	    public string MiddleName { get; set; }
	    public string LastName { get; set; }
	}

Then hit ALT + Enter and turn properties into Catel properties, which will result in this class:

	public class Person : ModelBase
	{
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
	    public static readonly PropertyData FirstNameProperty = RegisterProperty<Person, string>(model => model.FirstName);

	    /// <summary>
	    /// Gets or sets the middle name.
	    /// </summary>
	    public string MiddleName
	    {
	        get { return GetValue<string>(MiddleNameProperty); }
	        set { SetValue(MiddleNameProperty, value); }
	    }

	    /// <summary>
	    /// Register the MiddleName property so it is known in the class.
	    /// </summary>
	    public static readonly PropertyData MiddleNameProperty = RegisterProperty<Person, string>(model => model.MiddleName);

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
	    public static readonly PropertyData LastNameProperty = RegisterProperty<Person, string>(model => model.LastName);
	}

