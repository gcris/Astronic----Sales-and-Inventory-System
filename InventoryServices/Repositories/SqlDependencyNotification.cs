using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Repositories
{
    /// <summary>
    /// Handles the notifications from the database if there are any changes to it. 
    /// </summary>
    /// <typeparam name="TEntity">The generic parameter that must be a class.</typeparam>
    public sealed class SqlDependencyNotification<TEntity> where TEntity : class
    {
        private string connectionString;
        private SqlConnection sqlConnection;
        private Action<object, SqlNotificationEventArgs> onDependencyChangeCallback;
        private InventoryDbContext dbContext;
        private SqlDependency sqlDependency;

        public SqlDependencyNotification()
        {
            dbContext = new InventoryDbContext();
            connectionString = dbContext.Database.Connection.ConnectionString;
            sqlDependency = null;
        }

        /// <summary>
        /// Checks whether the user can request for notifications from the database.
        /// </summary>
        /// <returns>Returns the permission allowed to the user.</returns>
        private bool _UserCanRequestNotifications()
        {
            var permission = new SqlClientPermission(PermissionState.Unrestricted);
            try
            {
                permission.Demand();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// OnChangeEventHandler delegate.
        /// </summary>
        /// <param name="sender">The object that invokes the event.</param>
        /// <param name="e">The event argument.</param>
        private void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            // Notify only of the event type is of type Change.
            if (e.Type == SqlNotificationType.Change)
            {
                // Invoke the callback function.
                onDependencyChangeCallback.Invoke(sender, e);

                RevokeSqlDependencySubscription();

                // Renew Sql Dependency Subscription.
                InitiateNotifier(onDependencyChangeCallback);
            }
        }

        /// <summary>
        /// Removes any unused or deprecated Sql Dependency subscriptions.
        /// </summary>
        private void RevokeSqlDependencySubscription()
        {
            // Revoke any unused subscriptions.
            sqlDependency.OnChange -= OnDependencyChange;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public void StartSqlDependency()
        {
            SqlDependency.Stop(connectionString);
            SqlDependency.Start(connectionString);
        }

        /// <summary>
        /// Initiates and subscribes to an SqlDependency notification.
        /// </summary>
        /// <param name="onDependencyChangeCallback">The Callback method to execute if any changes on Sql Dependency Notification.</param>
        public void InitiateNotifier(Action<object, SqlNotificationEventArgs> onDependencyChangeCallback)
        {
            try
            {
                // Do only if the user is allowed to request for notifications.
                if (_UserCanRequestNotifications())
                {
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();

                    var queryString = (dbContext.Set<TEntity>() as DbQuery<TEntity>).ToString();

                    using (SqlCommand command = new SqlCommand(queryString, sqlConnection))
                    {
                        command.Notification = null;

                        this.onDependencyChangeCallback = onDependencyChangeCallback;

                        // Subscribe to Sql Dependency.
                        sqlDependency = new SqlDependency(command);

                        sqlDependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

                        using (SqlDataReader reader = command.ExecuteReader()) { /* Do nothing. */ }
                    }

                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Stops the Sql dependency.
        /// </summary>
        public void TerminateSqlDependency()
        {
            RevokeSqlDependencySubscription();
            SqlDependency.Stop(connectionString);
        }
    }
}
