Imports Catel.MVVM

Namespace $safeprojectname$.ViewModels

	''' <summary>
	''' MainPage view model.
	''' </summary>
	Public Class MainPageViewModel
		Inherits ViewModelBase
		#Region "Fields"
		#End Region

		#Region "Constructors"
		''' <summary>
		''' Initializes a new instance of the <see cref="MainPageViewModel"/> class.
		''' </summary>
		Public Sub New()
		End Sub
		#End Region

		#Region "Properties"
		''' <summary>
		''' Gets the title of the view model.
		''' </summary>
		''' <value>The title.</value>
		Public Overrides ReadOnly Property Title() As String
			Get
				Return "View model title"
			End Get
		End Property

		' TODO: Register models with the vmpropmodel codesnippet
		' TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		#End Region

		#Region "Commands"
		' TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
		#End Region

		#Region "Methods"
		''' <summary>
		''' Called when the navigation has completed.
		''' </summary>
		''' <remarks>
		''' This should of course be a cleaner solution, but there is no other way to let a view-model
		''' know that navigation has completed. Another option is injection, but this would require every
		''' view-model for Windows Phone 7 to accept only the navigation context, which has actually nothing
		''' to do with the logic.
		''' <para/>
		''' It is also possible to use the <see cref="NavigationCompleted"/> event.
		''' </remarks>
		Protected Overrides Sub OnNavigationCompleted()
			' TODO: Handle arguments that are now in NavigationContext
		End Sub
		#End Region
	End Class
	
End Namespace