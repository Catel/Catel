// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using Catel.Logging;

    /// <summary>
    /// Service to show a busy indicator.
    /// </summary>
    public partial class PleaseWaitService : IPleaseWaitService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ILanguageService _languageService;
        private readonly IDispatcherService _dispatcherService;

        private string _lastStatus = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitService" /> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="dispatcherService">The dispatcher service.</param>
        public PleaseWaitService(ILanguageService languageService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull("languageService", languageService);
            Argument.IsNotNull("dispatcherService", dispatcherService);

            _languageService = languageService;
            _dispatcherService = dispatcherService;
        }

        /// <summary>
        /// Gets the show counter.
        /// <para />
        /// This property can be used to get the current show counter if the please wait window should be hidden for a moment.
        /// </summary>
        /// <value>The show counter.</value>
        public int ShowCounter { get; private set; }

        partial void SetStatus(string status);
        partial void InitializeBusyIndicator();
        partial void ShowBusyIndicator(bool indeterminate);
        partial void HideBusyIndicator();

        /// <summary>
        /// Shows the please wait window with the specified status text.
        /// </summary>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default please wait text will be used.</param>
        /// <remarks>
        /// When this method is used, the <see cref="M:Catel.Services.IPleaseWaitService.Hide"/> method must be called to hide the window again.
        /// </remarks>
        public void Show(string status = "")
        {
            ShowCounter = 1;

            if (string.IsNullOrEmpty(status))
            {
                status = _languageService.GetString("PleaseWait");
            }

            UpdateStatus(status);

            ShowBusyIndicator(true);
        }

        /// <summary>
        /// Shows the please wait window with the specified status text and executes the work delegate (in a background thread). When the work
        /// is finished, the please wait window will be automatically closed.
        /// </summary>
        /// <param name="workDelegate">The work delegate.</param>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default please wait text will be used.</param>
        /// <remarks></remarks>
        public void Show(PleaseWaitWorkDelegate workDelegate, string status = "")
        {
            Argument.IsNotNull("workDelegate", workDelegate);

            InitializeBusyIndicator();

            Show(status);

            workDelegate();

            Hide();
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default please wait text will be used.</param>
        public void UpdateStatus(string status)
        {
            InitializeBusyIndicator();

            if (status == null)
            {
                status = string.Empty;
            }

            _lastStatus = status;

            SetStatus(status);
        }

        /// <summary>
        /// Updates the status and shows a progress bar with the specified status text. The percentage will be automatically calculated.
        /// <para/>
        /// The busy indicator will automatically hide when the <paramref name="totalItems"/> is larger than <paramref name="currentItem"/>.
        /// <para/>
        /// When providing the <paramref name="statusFormat"/>, it is possible to use <c>{0}</c> (represents current item) and
        /// <c>{1}</c> (represents total items).
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="totalItems">The total items.</param>
        /// <param name="statusFormat">The status format. Can be empty, but not <c>null</c>.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="currentItem"/> is smaller than zero.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="statusFormat"/> is <c>null</c>.</exception>
        /// <remarks></remarks>
        public void UpdateStatus(int currentItem, int totalItems, string statusFormat = "")
        {
            InitializeBusyIndicator();

            if (currentItem > totalItems)
            {
                Hide();
                return;
            }

            UpdateStatus(string.Format(statusFormat, currentItem, totalItems));

            ShowBusyIndicator(false);
        }

        /// <summary>
        /// Hides this please wait window.
        /// </summary>
        public void Hide()
        {
            InitializeBusyIndicator();

            HideBusyIndicator();

            ShowCounter = 0;
        }

        /// <summary>
        /// Increases the number of clients that show the please wait window. The implementing class
        /// is responsible for holding a counter internally which a call to this method will increase.
        /// <para/>
        /// As long as the internal counter is not zero (0), the please wait window will stay visible. To
        /// decrease the counter, make a call to <see cref="Pop"/>.
        /// <para/>
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        /// <param name="status">The status to change the text to.</param>
        /// <remarks></remarks>
        public void Push(string status = "")
        {
            if (ShowCounter <= 0)
            {
                Show(status);
            }
            else
            {
                ShowCounter++;
                UpdateStatus(status);
            }
        }

        /// <summary>
        /// Decreases the number of clients that show the please wait window. The implementing class
        /// is responsible for holding a counter internally which a call to this method will decrease.
        /// <para/>
        /// As long as the internal counter is not zero (0), the please wait window will stay visible. To
        /// increase the counter, make a call to <see cref="Pop"/>.
        /// <para/>
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        /// <remarks></remarks>
        public void Pop()
        {
            if (ShowCounter > 0)
            {
                ShowCounter--;
            }

            if (ShowCounter <= 0)
            {
                Hide();
            }
        }
    }
}