namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Busy indicator work delegate.
    /// </summary>
    public delegate void BusyIndicatorWorkDelegate();

    /// <summary>
    /// Busy indicator async work delegate.
    /// </summary>
    public delegate Task BusyIndicatorWorkAsyncDelegate();

    /// <summary>
    /// Interface for the busy indicator service.
    /// </summary>
    public interface IBusyIndicatorService
    {
        /// <summary>
        /// Gets the show counter.
        /// <para />
        /// This property can be used to get the current show counter if the busy indicator window should be hidden for a moment.
        /// </summary>
        /// <value>The show counter.</value>
        int ShowCounter { get; }
        
        #region Methods
        /// <summary>
        /// Shows the busy indicator window with the specified status text.
        /// </summary>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default busy indicator text will be used.</param>
        /// <remarks>
        /// When this method is used, the <see cref="Hide"/> method must be called to hide the window again.
        /// </remarks>
        void Show(string status = "");

        /// <summary>
        /// Shows the busy indicator window with the specified status text and executes the work delegate (in a background thread). When the work 
        /// is finished, the busy indicator window will be automatically closed.
        /// </summary>
        /// <param name="workDelegate">The work delegate.</param>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default busy indicator text will be used.</param>
        void Show(BusyIndicatorWorkDelegate workDelegate, string status = "");

        /// <summary>
        /// Shows the busy indicator window with the specified status text and executes the work delegate (in a background thread). When the work 
        /// is finished, the busy indicator window will be automatically closed.
        /// </summary>
        /// <param name="workDelegate">The work delegate.</param>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default busy indicator text will be used.</param>
        void Show(BusyIndicatorWorkAsyncDelegate workDelegate, string status = "");

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default busy indicator text will be used.</param>
        void UpdateStatus(string status);

        /// <summary>
        /// Updates the status and shows a progress bar with the specified status text. The percentage will be automatically calculated.
        /// <para />
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
        void UpdateStatus(int currentItem, int totalItems, string statusFormat = "");

        /// <summary>
        /// Hides this busy indicator window.
        /// </summary>
        void Hide();

        /// <summary>
        /// Increases the number of clients that show the busy indicator window. The implementing class
        /// is responsible for holding a counter internally which a call to this method will increase.
        /// <para/>
        /// As long as the internal counter is not zero (0), the busy indicator window will stay visible. To
        /// decrease the counter, make a call to <see cref="Pop"/>.
        /// <para/>
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        /// <param name="status">The status to change the text to.</param>
        void Push(string status = "");

        /// <summary>
        /// Decreases the number of clients that show the busy indicator window. The implementing class 
        /// is responsible for holding a counter internally which a call to this method will decrease.
        /// <para />
        /// As long as the internal counter is not zero (0), the busy indicator window will stay visible. To
        /// increase the counter, make a call to <see cref="Pop"/>.
        /// <para />
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        void Pop();
        #endregion
    }
}
