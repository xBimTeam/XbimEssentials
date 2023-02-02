using System;
using System.Collections.Generic;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Common.Metadata;
using Microsoft.Extensions.Logging;

namespace Xbim.Common
{
	public interface IModel: IDisposable
	{
       

        /// <summary>
        /// Returns or sets a user defined id for the model
        /// </summary>
        int UserDefinedId { get; set; }

        object Tag { get; set; }

        /// <summary>
        /// Returns a geometry store, null if geometry storage is not supported
        /// </summary>
        IGeometryStore GeometryStore { get; }
		IStepFileHeader Header { get; }

		bool IsTransactional { get; }

        IList<XbimInstanceHandle> InstanceHandles { get; }

        /// <summary>
        /// All instances which exist within a scope of the model.
        /// Use this property to retrieve the data from model.
        /// </summary>
	    IEntityCollection Instances { get; }

        /// <summary>
        /// This function is to be used by entities in the model
        /// in cases where data are persisted and entities are activated 
        /// on-the-fly as their properties are accessed.
        /// </summary>
        /// <param name="owningEntity">Entity to be activated</param>
        /// <returns>True if activation was successful, False otherwise</returns>
	    bool Activate(IPersistEntity owningEntity);

        /// <summary>
        /// Deletes entity from the model and removes all references to this entity in all entities
        /// in the model. This operation is potentially very expensive and some implementations of
        /// IModel might not implement it at all.
        /// </summary>
        /// <param name="entity"></param>
		void Delete (IPersistEntity entity);
		
        /// <summary>
        /// Begins transaction on the model to handle all modifications. You should use this function within
        /// a 'using' statement to restrict scope of the transaction. IModel should only hold weak reference to
        /// this object in 'CurrentTransaction' property.
        /// </summary>
        /// <param name="name">Name of the transaction. This is useful in case you keep the transactions for undo-redo sessions</param>
        /// <returns>Transaction object.</returns>
		ITransaction BeginTransaction(string name);
		
		/// <summary>
        /// It is a good practise to implement this property with WeakReference back field so it gets disposed 
		/// when transaction goes out of the scope. It would stay alive otherwise which is not desired unless you 
		/// want to keep it for undo-redo sessions. But even it that case it should be referenced from elsewhere.
        /// </summary>
		ITransaction CurrentTransaction { get; }

        /// <summary>
        /// Metadata representing current data schema of the model. This keeps pre-cached reflection information
        /// for efficient operations on the schema.
        /// </summary>
		ExpressMetaData Metadata { get; }

        /// <summary>
        /// If model contains a geometry and if IModel implementation supports it this property will return conversion factors for 
        /// base units to be used for geometry processing and other tasks.
        /// </summary>
		IModelFactors ModelFactors { get; }

        ///<summary>
        /// Implementation of IModel variant of InsertCopy() function
        /// </summary>
        /// <typeparam name="T">Type of the object to be inserted. This must be a type supported by this model</typeparam>
        /// <param name="toCopy">Object to copy</param>
        /// <param name="mappings">Mappings make sure object is only inserted once. You should use one instance of mappings for all InsertCopy() calls between two models</param>
        /// <param name="propTransform">Delegate which can be used to transform properties. You can use this to filter out certain properties or referenced objects</param>
        /// <param name="includeInverses">If TRUE inverse relations are also copied over. This may potentially bring over almost entire model if not controlled by propTransform delegate</param>
        /// <param name="keepLabels">If TRUE entity labels of inserted objects will be the same as the labels of original objects. This should be FALSE if you are inserting objects to existing model
        /// or if you are inserting objects from multiple source models into a single target model where entity labels may potentially clash.</param>
        /// <returns>New created object in this model which is a deep copy of original object</returns>
        /// <returns></returns>
	    T InsertCopy<T>(T toCopy, XbimInstanceHandleMap mappings, PropertyTranformDelegate propTransform,
	        bool includeInverses, bool keepLabels) where T : IPersistEntity;

		/// <summary>
        /// Performs a set of actions on a collection of entities inside a single read only transaction
        /// This improves database  performance for retrieving and accessing complex and deep objects
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) where TSource : IPersistEntity;

		/// <summary>
        /// This event is fired every time new entity is created.
        /// </summary>
        event NewEntityHandler EntityNew;
        
		/// <summary>
        /// This event is fired every time any entity is modified. If your model is not
        /// transactional it might not be called at all as the central point for all
        /// modifications is a transaction.
        /// </summary>
        event ModifiedEntityHandler EntityModified;
        
		/// <summary>
        /// This event is fired every time when entity gets deleted from model.
        /// </summary>
        event DeletedEntityHandler EntityDeleted;

        /// <summary>
        /// This will start to cache inverse relations which are heavily used in EXPRESS schema
        /// to model bidirectional relations. You shouldn't only use cache outside of transaction
        /// when you query the data but you don't change any values. Implementations of IModel
        /// might throw an exception in case you call this function inside of transaction
        /// or if you begin transaction before you stop caching. You should always keep the caching
        /// object inside of using statement as IModel should only hold the weak reference to it.
        /// </summary>
        /// <returns></returns>
        IInverseCache BeginInverseCaching();

        /// <summary>
        /// Inverse cache created with BeginInverseCaching(). This should be a weak reference internally.
        /// </summary>
        IInverseCache InverseCache { get; }

        /// <summary>
        /// This will start to cache entities if IModel implementation uses any kind of
        /// on-demand data loading and entity activation. This will keep entities alive and will
        /// minimize number of read operations needed to get data. It will also grow in memory
        /// which might not be desired. Always use this in using statement or dispose the object explicitly.
        /// </summary>
        /// <returns></returns>
        IEntityCache BeginEntityCaching();

        /// <summary>
        /// Entity cache created with BeginEntityCaching(). This should be a weak reference internally.
        /// </summary>
        IEntityCache EntityCache { get; }

        /// <summary>
        /// Predefined schemas supported by xBIM
        /// </summary>
        XbimSchemaVersion SchemaVersion { get; }
    }

	public delegate void NewEntityHandler(IPersistEntity entity);
    public delegate void ModifiedEntityHandler(IPersistEntity entity, int property);
    public delegate void DeletedEntityHandler(IPersistEntity entity);

    public delegate object PropertyTranformDelegate(ExpressMetaProperty property, object parentObject);
}
