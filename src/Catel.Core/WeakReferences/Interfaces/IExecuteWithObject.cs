namespace Catel
{
    /// <summary>
    /// Interface defining a method accepting an object parameter. This allows the execution
    /// of generic objects in a non-generic way.
    /// </summary>
    public interface IExecuteWithObject
    {
        /// <summary>
        /// Executes the object with the object parameter.
        /// <para />
        /// The class implementing this interface is responsible for casting the <paramref name="parameter"/>
        /// to the right type and to determine whether <c>null</c> is allowed as parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if the action is executed successfully; otherwise <c>false</c>.</returns>
        bool ExecuteWithObject(object parameter);
    }

    /// <summary>
    /// Interface defining a method accepting an object parameter. This allows the execution
    /// of generic objects in a non-generic way.
    /// </summary>
    public interface IExecuteWithObject<TParameter> : IExecuteWithObject
    {
        /// <summary>
        /// Executes the object with the object parameter.
        /// <para />
        /// The class implementing this interface is responsible for casting the <paramref name="parameter"/>
        /// to the right type and to determine whether <c>null</c> is allowed as parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if the action is executed successfully; otherwise <c>false</c>.</returns>
        bool ExecuteWithObject(TParameter parameter);
    }

    /// <summary>
    /// Interface defining a method accepting an object parameter. This allows the execution
    /// of generic objects in a non-generic way.
    /// </summary>
    public interface IExecuteWithObject<TParameter, TResult>
    {
        /// <summary>
        /// Executes the object with the object parameter.
        /// <para />
        /// The class implementing this interface is responsible for casting the <paramref name="parameter"/>
        /// to the right type and to determine whether <c>null</c> is allowed as parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="result">The result</param>
        /// <returns><c>true</c> if the action is executed successfully; otherwise <c>false</c>.</returns>
        bool ExecuteWithObject(TParameter parameter, out TResult result);
    }
}
