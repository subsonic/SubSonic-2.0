namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISubSonicRepository
    {
        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        DataProvider Provider { get; set; }

        /// <summary>
        /// Deletes the specified column name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnValue">The column value.</param>
        void Delete<T>(string columnName, object columnValue) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns></returns>
        Delete Delete();

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        void Delete<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Deletes the by key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemId">The item id.</param>
        void DeleteByKey<T>(object itemId) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Destroys the specified column name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        void Destroy<T>(string columnName, object value) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Destroys the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        void Destroy<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Destroys the by key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemId">The item id.</param>
        void DestroyByKey<T>(object itemId) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Gets the specified primary key value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyValue">The primary key value.</param>
        /// <returns></returns>
        T Get<T>(object primaryKeyValue) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Gets the specified column name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnValue">The column value.</param>
        /// <returns></returns>
        T Get<T>(string columnName, object columnValue) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        int Insert<T>(RepositoryRecord<T> item, string userName) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Inserts this instance.
        /// </summary>
        /// <returns></returns>
        Insert Insert();

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        InlineQuery Query();

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Save<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        int Save<T>(RepositoryRecord<T> item, string userName) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Selects this instance.
        /// </summary>
        /// <returns></returns>
        Select Select();

        /// <summary>
        /// Selects the specified aggregates.
        /// </summary>
        /// <param name="aggregates">The aggregates.</param>
        /// <returns></returns>
        Select Select(params Aggregate[] aggregates);

        /// <summary>
        /// Selects the specified columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        Select Select(params string[] columns);

        /// <summary>
        /// Selects all columns from.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Select SelectAllColumnsFrom<T>() where T : RecordBase<T>, new();

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Update Update<T>() where T : RecordBase<T>, new();

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Update<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        int Update<T>(RepositoryRecord<T> item, string userName) where T : RepositoryRecord<T>, new();

        /// <summary>
        /// Determines whether the specified connection string is online.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified connection string is online; otherwise, <c>false</c>.
        /// </returns>
        bool IsOnline(string connectionString);

        /// <summary>
        /// Determines whether this instance is online.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is online; otherwise, <c>false</c>.
        /// </returns>
        bool IsOnline();

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <typeparam name="ItemType">The type of the tem type.</typeparam>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <param name="itemList">The item list.</param>
        /// <returns></returns>
        int SaveAll<ItemType, ListType>(RepositoryList<ItemType, ListType> itemList)
            where ItemType : RepositoryRecord<ItemType>, new()
            where ListType : RepositoryList<ItemType, ListType>, new();

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <typeparam name="ItemType">The type of the tem type.</typeparam>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <param name="itemList">The item list.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        int SaveAll<ItemType, ListType>(RepositoryList<ItemType, ListType> itemList, string userName)
            where ItemType : RepositoryRecord<ItemType>, new()
            where ListType : RepositoryList<ItemType, ListType>, new();
    }
}