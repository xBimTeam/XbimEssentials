using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Collections.Generic
{

    /// <summary>
    /// A MultiValueDictionary can be viewed as a <see cref="IDictionary" /> that allows multiple 
    /// values for any given unique key. While the MultiValueDictionary API is 
    /// mostly the same as that of a regular <see cref="IDictionary" />, there is a distinction
    /// in that getting the value for a key returns a <see cref="ICollection{TValue}" /> of values
    /// rather than a single value associated with that key. Additionally, 
    /// there is functionality to allow adding or removing more than a single
    /// value at once. 
    /// 
    /// The MultiValueDictionary can also be viewed as a IReadOnlyDictionary&lt;TKey,ICollection&lt;TValue&gt;t&gt;
    /// where the <see cref="ICollection{TValue}" /> is abstracted from the view of the programmer.
    /// 
    /// For a read-only MultiValueDictionary, see <see cref="ILookup{TKey, TValue}" />.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class XbimMultiValueDictionary<TKey, TValue> :
            IDictionary<TKey, ICollection<TValue>>
    {
        #region Variables
        /*======================================================================
        ** Variables
        ======================================================================*/

        /// <summary>
        /// The private dictionary that this class effectively wraps around
        /// </summary>
        private Dictionary<TKey, InnerCollectionView> dictionary;

        /// <summary>
        /// The function to construct a new <see cref="ICollection{TValue}"/>
        /// </summary>
        /// <returns></returns>
        private Func<ICollection<TValue>> NewCollectionFactory = () => new List<TValue>();

        /// <summary>
        /// The current version of this MultiValueDictionary used to determine MultiValueDictionary modification
        /// during enumeration
        /// </summary>
        private int version;

        #endregion

        #region Constructors
        /*======================================================================
        ** Constructors
        ======================================================================*/

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the default initial capacity, and uses the default
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>.
        /// </summary>
        public XbimMultiValueDictionary()
        {
            dictionary = new Dictionary<TKey, InnerCollectionView>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class that is 
        /// empty, has the specified initial capacity, and uses the default <see cref="IEqualityComparer{TKey}"/>
        /// for <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="capacity">Initial number of keys that the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> will allocate space for</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity must be >= 0</exception>
        public XbimMultiValueDictionary(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "ArgumentOutOfRange_NeedNonNegNum");
            dictionary = new Dictionary<TKey, InnerCollectionView>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class 
        /// that is empty, has the default initial capacity, and uses the 
        /// specified <see cref="IEqualityComparer{TKey}" />.
        /// </summary>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        public XbimMultiValueDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, InnerCollectionView>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class 
        /// that is empty, has the specified initial capacity, and uses the 
        /// specified <see cref="IEqualityComparer{TKey}" />.
        /// </summary>
        /// <param name="capacity">Initial number of keys that the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> will allocate space for</param>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <exception cref="ArgumentOutOfRangeException">Capacity must be >= 0</exception>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        public XbimMultiValueDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Properties.Resources.ArgumentOutOfRange_NeedNonNegNum");
            dictionary = new Dictionary<TKey, InnerCollectionView>(capacity, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class that contains 
        /// elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, ICollection&lt;TValue&gt;&gt;&gt; and uses the 
        /// default <see cref="IEqualityComparer{TKey}" /> for the <typeparamref name="TKey"/> type.
        /// </summary>
        /// <param name="enumerable">IEnumerable to copy elements into this from</param>
        /// <exception cref="ArgumentNullException">enumerable must be non-null</exception>
        public XbimMultiValueDictionary(IEnumerable<KeyValuePair<TKey, ICollection<TValue>>> enumerable)
            : this(enumerable, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class that contains 
        /// elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, ICollection&lt;TValue&gt;&gt;&gt; and uses the 
        /// specified <see cref="IEqualityComparer{TKey}" />.
        /// </summary>
        /// <param name="enumerable">IEnumerable to copy elements into this from</param>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <exception cref="ArgumentNullException">enumerable must be non-null</exception>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        public XbimMultiValueDictionary(IEnumerable<KeyValuePair<TKey, ICollection<TValue>>> enumerable, IEqualityComparer<TKey> comparer)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            dictionary = new Dictionary<TKey, InnerCollectionView>(comparer);
            foreach (var pair in enumerable)
                AddRange(pair.Key, pair.Value);
        }

        #endregion

        #region Static Factories
        /*======================================================================
        ** Static Factories
        ======================================================================*/

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the default initial capacity, and uses the default
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><typeparamref name="TValueCollection"/> must not have
        /// IsReadOnly set to true by default.</exception>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>()
            where TValueCollection : ICollection<TValue>, new()
        {
            if (new TValueCollection().IsReadOnly)
                throw new InvalidOperationException("Properties.Resources.Create_TValueCollectionReadOnly");

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>();
            multiValueDictionary.NewCollectionFactory = () => new TValueCollection();
            return multiValueDictionary;
        }

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the specified initial capacity, and uses the default
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="capacity">Initial number of keys that the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> will allocate space for</param>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Capacity must be >= 0</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TValueCollection"/> must not have
        /// IsReadOnly set to true by default.</exception>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(int capacity)
            where TValueCollection : ICollection<TValue>, new()
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Properties.Resources.ArgumentOutOfRange_NeedNonNegNum");
            if (new TValueCollection().IsReadOnly)
                throw new InvalidOperationException("Properties.Resources.Create_TValueCollectionReadOnly");

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(capacity);
            multiValueDictionary.NewCollectionFactory = () => new TValueCollection();
            return multiValueDictionary;
        }

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the default initial capacity, and uses the specified
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <exception cref="InvalidOperationException"><typeparamref name="TValueCollection"/> must not have
        /// IsReadOnly set to true by default.</exception>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(IEqualityComparer<TKey> comparer)
            where TValueCollection : ICollection<TValue>, new()
        {
            if (new TValueCollection().IsReadOnly)
                throw new InvalidOperationException("Properties.Resources.Create_TValueCollectionReadOnly");

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(comparer);
            multiValueDictionary.NewCollectionFactory = () => new TValueCollection();
            return multiValueDictionary;
        }

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the specified initial capacity, and uses the specified
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="capacity">Initial number of keys that the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> will allocate space for</param>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><typeparamref name="TValueCollection"/> must not have
        /// IsReadOnly set to true by default.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Capacity must be >= 0</exception>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(int capacity, IEqualityComparer<TKey> comparer)
            where TValueCollection : ICollection<TValue>, new()
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Properties.Resources.ArgumentOutOfRange_NeedNonNegNum");
            if (new TValueCollection().IsReadOnly)
                throw new InvalidOperationException("Properties.Resources.Create_TValueCollectionReadOnly");

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(capacity, comparer);
            multiValueDictionary.NewCollectionFactory = () => new TValueCollection();
            return multiValueDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class that contains 
        /// elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, ICollection&lt;TValue&gt;&gt;&gt;
        /// and uses the default <see cref="IEqualityComparer{TKey}" /> for the <typeparamref name="TKey"/> type.
        /// The internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="enumerable">IEnumerable to copy elements into this from</param>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><typeparamref name="TValueCollection"/> must not have
        /// IsReadOnly set to true by default.</exception>
        /// <exception cref="ArgumentNullException">enumerable must be non-null</exception>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(IEnumerable<KeyValuePair<TKey, ICollection<TValue>>> enumerable)
            where TValueCollection : ICollection<TValue>, new()
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (new TValueCollection().IsReadOnly)
                throw new InvalidOperationException("Properties.Resources.Create_TValueCollectionReadOnly");

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>();
            multiValueDictionary.NewCollectionFactory = () => new TValueCollection();
            foreach (var pair in enumerable)
                multiValueDictionary.AddRange(pair.Key, pair.Value);
            return multiValueDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class that contains 
        /// elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, ICollection&lt;TValue&gt;&gt;&gt;
        /// and uses the specified <see cref="IEqualityComparer{TKey}" /> for the <typeparamref name="TKey"/> type.
        /// The internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="enumerable">IEnumerable to copy elements into this from</param>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><typeparamref name="TValueCollection"/> must not have
        /// IsReadOnly set to true by default.</exception>
        /// <exception cref="ArgumentNullException">enumerable must be non-null</exception>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(IEnumerable<KeyValuePair<TKey, ICollection<TValue>>> enumerable, IEqualityComparer<TKey> comparer)
            where TValueCollection : ICollection<TValue>, new()
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (new TValueCollection().IsReadOnly)
                throw new InvalidOperationException("Properties.Resources.Create_TValueCollectionReadOnly");

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(comparer);
            multiValueDictionary.NewCollectionFactory = () => new TValueCollection();
            foreach (var pair in enumerable)
                multiValueDictionary.AddRange(pair.Key, pair.Value);
            return multiValueDictionary;
        }

        #endregion

        #region Static Factories with Func parameters
        /*======================================================================
        ** Static Factories with Func parameters
        ======================================================================*/

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the default initial capacity, and uses the default
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="collectionFactory">A function to create a new <see cref="ICollection{TValue}"/> to use
        /// in the internal dictionary store of this <see cref="XbimMultiValueDictionary{TKey,TValue}" />.</param>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="collectionFactory"/> must create collections with
        /// IsReadOnly set to true by default.</exception>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(Func<TValueCollection> collectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            if (collectionFactory().IsReadOnly)
                throw new InvalidOperationException(("Properties.Resources.Create_TValueCollectionReadOnly"));

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>();
            multiValueDictionary.NewCollectionFactory = (Func<ICollection<TValue>>)(Delegate)collectionFactory;
            return multiValueDictionary;
        }

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the specified initial capacity, and uses the default
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="capacity">Initial number of keys that the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> will allocate space for</param>
        /// <param name="collectionFactory">A function to create a new <see cref="ICollection{TValue}"/> to use
        /// in the internal dictionary store of this <see cref="XbimMultiValueDictionary{TKey,TValue}" />.</param> 
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Capacity must be >= 0</exception>
        /// <exception cref="InvalidOperationException"><paramref name="collectionFactory"/> must create collections with
        /// IsReadOnly set to true by default.</exception>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(int capacity, Func<TValueCollection> collectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Properties.Resources.ArgumentOutOfRange_NeedNonNegNum");
            if (collectionFactory().IsReadOnly)
                throw new InvalidOperationException(("Properties.Resources.Create_TValueCollectionReadOnly"));

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(capacity);
            multiValueDictionary.NewCollectionFactory = (Func<ICollection<TValue>>)(Delegate)collectionFactory;
            return multiValueDictionary;
        }

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the default initial capacity, and uses the specified
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <param name="collectionFactory">A function to create a new <see cref="ICollection{TValue}"/> to use
        /// in the internal dictionary store of this <see cref="XbimMultiValueDictionary{TKey,TValue}" />.</param> 
        /// <exception cref="InvalidOperationException"><paramref name="collectionFactory"/> must create collections with
        /// IsReadOnly set to true by default.</exception>
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(IEqualityComparer<TKey> comparer, Func<TValueCollection> collectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            if (collectionFactory().IsReadOnly)
                throw new InvalidOperationException(("Properties.Resources.Create_TValueCollectionReadOnly"));

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(comparer);
            multiValueDictionary.NewCollectionFactory = (Func<ICollection<TValue>>)(Delegate)collectionFactory;
            return multiValueDictionary;
        }

        /// <summary>
        /// Creates a new new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> 
        /// class that is empty, has the specified initial capacity, and uses the specified
        /// <see cref="IEqualityComparer{TKey}" /> for <typeparamref name="TKey"/>. The 
        /// internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="capacity">Initial number of keys that the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> will allocate space for</param>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <param name="collectionFactory">A function to create a new <see cref="ICollection{TValue}"/> to use
        /// in the internal dictionary store of this <see cref="XbimMultiValueDictionary{TKey,TValue}" />.</param> 
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="collectionFactory"/> must create collections with
        /// IsReadOnly set to true by default.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Capacity must be >= 0</exception>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(int capacity, IEqualityComparer<TKey> comparer, Func<TValueCollection> collectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Properties.Resources.ArgumentOutOfRange_NeedNonNegNum");
            if (collectionFactory().IsReadOnly)
                throw new InvalidOperationException(("Properties.Resources.Create_TValueCollectionReadOnly"));

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(capacity, comparer);
            multiValueDictionary.NewCollectionFactory = (Func<ICollection<TValue>>)(Delegate)collectionFactory;
            return multiValueDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class that contains 
        /// elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, ICollection&lt;TValue&gt;&gt;&gt;
        /// and uses the default <see cref="IEqualityComparer{TKey}" /> for the <typeparamref name="TKey"/> type.
        /// The internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="enumerable">IEnumerable to copy elements into this from</param>
        /// <param name="collectionFactory">A function to create a new <see cref="ICollection{TValue}"/> to use
        /// in the internal dictionary store of this <see cref="XbimMultiValueDictionary{TKey,TValue}" />.</param> 
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="collectionFactory"/> must create collections with
        /// IsReadOnly set to true by default.</exception>
        /// <exception cref="ArgumentNullException">enumerable must be non-null</exception>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(IEnumerable<KeyValuePair<TKey, ICollection<TValue>>> enumerable, Func<TValueCollection> collectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (collectionFactory().IsReadOnly)
                throw new InvalidOperationException(("Properties.Resources.Create_TValueCollectionReadOnly"));

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>();
            multiValueDictionary.NewCollectionFactory = (Func<ICollection<TValue>>)(Delegate)collectionFactory;
            foreach (var pair in enumerable)
                multiValueDictionary.AddRange(pair.Key, pair.Value);
            return multiValueDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimMultiValueDictionary{TKey,TValue}" /> class that contains 
        /// elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, ICollection&lt;TValue&gt;&gt;&gt;
        /// and uses the specified <see cref="IEqualityComparer{TKey}" /> for the <typeparamref name="TKey"/> type.
        /// The internal dictionary will use instances of the <typeparamref name="TValueCollection"/>
        /// class as its collection type.
        /// </summary>
        /// <typeparam name="TValueCollection">
        /// The collection type that this <see cref="XbimMultiValueDictionary{TKey,TValue}" />
        /// will contain in its internal dictionary.
        /// </typeparam>
        /// <param name="enumerable">IEnumerable to copy elements into this from</param>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s</param>
        /// <param name="collectionFactory">A function to create a new <see cref="ICollection{TValue}"/> to use
        /// in the internal dictionary store of this <see cref="XbimMultiValueDictionary{TKey,TValue}" />.</param> 
        /// <returns>A new <see cref="XbimMultiValueDictionary{TKey,TValue}" /> with the specified
        /// parameters.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="collectionFactory"/> must create collections with
        /// IsReadOnly set to true by default.</exception>
        /// <exception cref="ArgumentNullException">enumerable must be non-null</exception>
        /// <remarks>If <paramref name="comparer"/> is set to null, then the default <see cref="IEqualityComparer" /> for <typeparamref name="TKey"/> is used.</remarks>
        /// <remarks>
        /// Note that <typeparamref name="TValueCollection"/> must implement <see cref="ICollection{TValue}"/>
        /// in addition to being constructable through new(). The collection returned from the constructor
        /// must also not have IsReadOnly set to True by default.
        /// </remarks>
        public static XbimMultiValueDictionary<TKey, TValue> Create<TValueCollection>(IEnumerable<KeyValuePair<TKey, ICollection<TValue>>> enumerable, IEqualityComparer<TKey> comparer, Func<TValueCollection> collectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (collectionFactory().IsReadOnly)
                throw new InvalidOperationException(("Properties.Resources.Create_TValueCollectionReadOnly"));

            var multiValueDictionary = new XbimMultiValueDictionary<TKey, TValue>(comparer);
            multiValueDictionary.NewCollectionFactory = (Func<ICollection<TValue>>)(Delegate)collectionFactory;
            foreach (var pair in enumerable)
                multiValueDictionary.AddRange(pair.Key, pair.Value);
            return multiValueDictionary;
        }

        #endregion

        #region Concrete Methods
        /*======================================================================
        ** Concrete Methods
        ======================================================================*/

        /// <summary>
        /// Adds the specified <typeparamref name="TKey"/> and <typeparamref name="TValue"/> to the <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element to add.</param>
        /// <param name="value">The <typeparamref name="TValue"/> of the element to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Unlike the Add for <see cref="IDictionary" />, the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> Add will not
        /// throw any exceptions. If the given <typeparamref name="TKey"/> is already in the <see cref="XbimMultiValueDictionary{TKey,TValue}"/>,
        /// then <typeparamref name="TValue"/> will be added to <see cref="ICollection{TValue}"/> associated with <paramref name="key"/>
        /// </remarks>
        /// <remarks>
        /// A call to this Add method will always invalidate any currently running enumeration regardless
        /// of whether the Add method actually modified the <see cref="XbimMultiValueDictionary{TKey,TValue}" />.
        /// </remarks>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            InnerCollectionView collection;
            if (!dictionary.TryGetValue(key, out collection))
            {
                collection = new InnerCollectionView(key, NewCollectionFactory());
                dictionary.Add(key, collection);
            }
            collection.AddValue(value);
            version++;
        }

        /// <summary>
        /// Adds a number of key-value pairs to this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>, where
        /// the key for each value is <paramref name="key"/>, and the value for a pair
        /// is an element from <paramref name="values"/>
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of all entries to add</param>
        /// <param name="values">An <see cref="IEnumerable{TValue}"/> of values to add</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> and <paramref name="values"/> must be non-null</exception>
        /// <remarks>
        /// A call to this AddRange method will always invalidate any currently running enumeration regardless
        /// of whether the AddRange method actually modified the <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </remarks>
        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (values == null)
                throw new ArgumentNullException("values");

            InnerCollectionView collection;
            if (!dictionary.TryGetValue(key, out collection))
            {
                collection = new InnerCollectionView(key, NewCollectionFactory());
                dictionary.Add(key, collection);
            }
            foreach (TValue value in values)
            {
                collection.AddValue(value);
            }
            version++;
        }

        public void Add(TKey key, ICollection<TValue> value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes every <typeparamref name="TValue"/> associated with the given <typeparamref name="TKey"/>
        /// from the <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the elements to remove</param>
        /// <returns><c>true</c> if the removal was successful; otherwise <c>false</c></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            InnerCollectionView collection;
            if (dictionary.TryGetValue(key, out collection) && dictionary.Remove(key))
            {
                version++;
                return true;
            }
            return false;
        }



        ICollection<TValue> IDictionary<TKey, ICollection<TValue>>.this[TKey key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Removes the first instance (if any) of the given <typeparamref name="TKey"/>-<typeparamref name="TValue"/> 
        /// pair from this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>. 
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element to remove</param>
        /// <param name="value">The <typeparamref name="TValue"/> of the element to remove</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> must be non-null</exception>
        /// <returns><c>true</c> if the removal was successful; otherwise <c>false</c></returns>
        /// <remarks>
        /// If the <typeparamref name="TValue"/> being removed is the last one associated with its <typeparamref name="TKey"/>, then that 
        /// <typeparamref name="TKey"/> will be removed from the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> and its 
        /// associated <see cref="ICollection{TValue}"/> will be freed as if a call to <see cref="Remove(TKey)"/>
        /// had been made.
        /// </remarks>
        public bool Remove(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            InnerCollectionView collection;
            if (dictionary.TryGetValue(key, out collection) && collection.RemoveValue(value))
            {
                if (collection.Count == 0)
                    dictionary.Remove(key);
                version++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the given <typeparamref name="TKey"/>-<typeparamref name="TValue"/> 
        /// pair exists within this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element.</param>
        /// <param name="value">The <typeparamref name="TValue"/> of the element.</param>
        /// <returns><c>true</c> if found; otherwise <c>false</c></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> must be non-null</exception>
        public bool Contains(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            InnerCollectionView collection;
            return (dictionary.TryGetValue(key, out collection) && collection.Contains(value));
        }

        /// <summary>
        /// Determines if the given <typeparamref name="TValue"/> exists within this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="value">A <typeparamref name="TValue"/> to search the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> for</param>
        /// <returns><c>true</c> if the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> contains the <paramref name="value"/>; otherwise <c>false</c></returns>      
        public bool ContainsValue(TValue value)
        {
            foreach (InnerCollectionView sublist in dictionary.Values)
                if (sublist.Contains(value))
                    return true;
            return false;
        }

        public void Add(KeyValuePair<TKey, ICollection<TValue>> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes every <typeparamref name="TKey"/> and <typeparamref name="TValue"/> from this 
        /// <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
            version++;
        }

        public bool Contains(KeyValuePair<TKey, ICollection<TValue>> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, ICollection<TValue>> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Members implemented from IReadOnlyDictionary<TKey, ICollection<TValue>>
        /*======================================================================
        ** Members implemented from IReadOnlyDictionary<TKey, ICollection<TValue>>
        ======================================================================*/

        /// <summary>
        /// Determines if the given <typeparamref name="TKey"/> exists within this <see cref="XbimMultiValueDictionary{TKey,TValue}"/> and has
        /// at least one <typeparamref name="TValue"/> associated with it.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> to search the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> for</param>
        /// <returns><c>true</c> if the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> contains the requested <typeparamref name="TKey"/>;
        /// otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> must be non-null</exception>
        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            // Since modification to the MultiValueDictionary is only allowed through its own API, we
            // can ensure that if a collection is in the internal dictionary then it must have at least one
            // associated TValue, or else it would have been removed whenever its final TValue was removed.
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Gets each <typeparamref name="TKey"/> in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/> that
        /// has one or more associated <typeparamref name="TValue"/>.
        /// </summary>
        /// <value>
        /// An <see cref="IEnumerable{TKey}"/> containing each <typeparamref name="TKey"/> 
        /// in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/> that has one or more associated 
        /// <typeparamref name="TValue"/>.
        /// </value>
     
        public IEnumerable<TKey> Keys
        {
            get
            {
                return dictionary.Keys;
            }
        }

        ICollection<ICollection<TValue>> IDictionary<TKey, ICollection<TValue>>.Values
        {
            get
            {               
               return dictionary.Select(kv => kv.Value).Cast<ICollection<TValue>>().ToList();               
            }
        }

        /// <summary>
        /// Attempts to get the <typeparamref name="TValue"/> associated with the given
        /// <typeparamref name="TKey"/> and place it into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the element to retrieve</param>
        /// <param name="value">
        /// When this method returns, contains the <typeparamref name="TValue"/> associated with the specified
        /// <typeparamref name="TKey"/> if it is found; otherwise contains the default value of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> contains an element with the specified 
        /// <typeparamref name="TKey"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> must be non-null</exception>
        public bool TryGetValue(TKey key, out ICollection<TValue> value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            InnerCollectionView collection;
            var success = dictionary.TryGetValue(key, out collection);
            value = collection;
            return success;
        }

        ICollection<TKey> IDictionary<TKey, ICollection<TValue>>.Keys
        {
            get
            {
                return dictionary.Keys;
            }
        }

        /// <summary>
        /// Gets an enumerable of <see cref="ICollection{TValue}"/> from this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>,
        /// where each <see cref="ICollection{TValue}" /> is the collection of every <typeparamref name="TValue"/> associated
        /// with a <typeparamref name="TKey"/> present in the <see cref="XbimMultiValueDictionary{TKey,TValue}"/>. 
        /// </summary>
        /// <value>An IEnumerable of each <see cref="ICollection{TValue}"/> in this 
        /// <see cref="XbimMultiValueDictionary{TKey,TValue}"/></value>
        public IEnumerable<ICollection<TValue>> Values
        {
            get
            {
                return dictionary.Values;
            }
        }

        /// <summary>
        /// Get every <typeparamref name="TValue"/> associated with the given <typeparamref name="TKey"/>. If 
        /// <paramref name="key"/> is not found in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>, will 
        /// throw a <see cref="KeyNotFoundException"/>.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> of the elements to retrieve.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> must be non-null</exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not have any associated 
        /// <typeparamref name="TValue"/>s in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.</exception>
        /// <value>
        /// An <see cref="ICollection{TValue}"/> containing every <typeparamref name="TValue"/>
        /// associated with <paramref name="key"/>.
        /// </value>
        /// <remarks>
        /// Note that the <see cref="ICollection{TValue}"/> returned will change alongside any changes 
        /// to the <see cref="XbimMultiValueDictionary{TKey,TValue}"/> 
        /// </remarks>
        public ICollection<TValue> this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                InnerCollectionView collection;
                if (dictionary.TryGetValue(key, out collection))
                    return collection;
                else
                    throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Returns the number of <typeparamref name="TKey"/>s with one or more associated <typeparamref name="TValue"/>
        /// in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <value>The number of <typeparamref name="TKey"/>s in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.</value>
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an Enumerator over the <typeparamref name="TKey"/>-<see cref="ICollection{TValue}"/>
        /// pairs in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>an Enumerator over the <typeparamref name="TKey"/>-<see cref="ICollection{TValue}"/>
        /// pairs in this <see cref="XbimMultiValueDictionary{TKey,TValue}"/>.</returns>
        public IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        /// <summary>
        /// The Enumerator class for a <see cref="XbimMultiValueDictionary{TKey,TValue}"/>
        /// that iterates over <typeparamref name="TKey"/>-<see cref="ICollection{TValue}"/>
        /// pairs.
        /// </summary>
        private class Enumerator :
            IEnumerator<KeyValuePair<TKey, ICollection<TValue>>>
        {
            private XbimMultiValueDictionary<TKey, TValue> _xbimMultiValueDictionary;
            private int version;
            private KeyValuePair<TKey, ICollection<TValue>> current;
            private Dictionary<TKey, InnerCollectionView>.Enumerator enumerator;
            private enum EnumerationState { BeforeFirst, During, AfterLast };
            private EnumerationState state;

            /// <summary>
            /// Constructor for the enumerator
            /// </summary>
            /// <param name="_xbimMultiValueDictionary">A MultiValueDictionary to iterate over</param>
            internal Enumerator(XbimMultiValueDictionary<TKey, TValue> _xbimMultiValueDictionary)
            {
                this._xbimMultiValueDictionary = _xbimMultiValueDictionary;
                this.version = _xbimMultiValueDictionary.version;
                this.current = default(KeyValuePair<TKey, ICollection<TValue>>);
                this.enumerator = _xbimMultiValueDictionary.dictionary.GetEnumerator();
                this.state = EnumerationState.BeforeFirst; ;
            }

            public KeyValuePair<TKey, ICollection<TValue>> Current
            {
                get { return current; }
            }

            object IEnumerator.Current
            {
                get
                {
                    switch (state)
                    {
                        case EnumerationState.BeforeFirst:
                            throw new InvalidOperationException(("Properties.Resources.InvalidOperation_EnumNotStarted"));
                        case EnumerationState.AfterLast:
                            throw new InvalidOperationException(("Properties.Resources.InvalidOperation_EnumEnded"));
                        default:
                            return current;
                    }
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                if (version != _xbimMultiValueDictionary.version)
                {
                    throw new InvalidOperationException("Properties.Resources.InvalidOperation_EnumFailedVersion");
                }
                else if (enumerator.MoveNext())
                {
                    current = new KeyValuePair<TKey, ICollection<TValue>>(enumerator.Current.Key, (ICollection<TValue>)enumerator.Current.Value);
                    state = EnumerationState.During;
                    return true;
                }
                else
                {
                    current = default(KeyValuePair<TKey, ICollection<TValue>>);
                    state = EnumerationState.AfterLast;
                    return false;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                if (version != _xbimMultiValueDictionary.version)
                    throw new InvalidOperationException("Properties.Resources.InvalidOperation_EnumFailedVersion");
                enumerator.Dispose();
                enumerator = _xbimMultiValueDictionary.dictionary.GetEnumerator();
                current = default(KeyValuePair<TKey, ICollection<TValue>>);
                state = EnumerationState.BeforeFirst;
            }

            /// <summary>
            /// Frees resources associated with this Enumerator
            /// </summary>
            public void Dispose()
            {
                enumerator.Dispose();
            }
        }

        /// <summary>
        /// An inner class that functions as a view of an ICollection within a MultiValueDictionary
        /// </summary>
        private class InnerCollectionView :
            ICollection<TValue>
        {
            private TKey key;
            private ICollection<TValue> collection;

            #region Private Concrete API
            /*======================================================================
            ** Private Concrete API
            ======================================================================*/

            public InnerCollectionView(TKey key, ICollection<TValue> collection)
            {
                this.key = key;
                this.collection = collection;
            }

            public void AddValue(TValue item)
            {
                collection.Add(item);
            }

            public bool RemoveValue(TValue item)
            {
                return collection.Remove(item);
            }

            #endregion

            #region Shared API
            /*======================================================================
            ** Shared API
            ======================================================================*/

            public bool Contains(TValue item)
            {
                return collection.Contains(item);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException("arrayIndex", "Properties.Resources.ArgumentOutOfRange_NeedNonNegNum");
                if (arrayIndex > array.Length)
                    throw new ArgumentOutOfRangeException("arrayIndex", "Properties.Resources.ArgumentOutOfRange_Index");
                if (array.Length - arrayIndex < collection.Count)
                    throw new ArgumentException("Properties.Resources.CopyTo_ArgumentsTooSmall", "arrayIndex");

                collection.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get
                {
                    return collection.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public TKey Key
            {
                get { return key; }
            }

            #endregion

            #region Public-Facing API
            /*======================================================================
            ** Public-Facing API
            ======================================================================*/

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException("Properties.Resources.ReadOnly_Modification");
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("Properties.Resources.ReadOnly_Modification");
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException("Properties.Resources.ReadOnly_Modification");
            }

            #endregion
        }
    }
}



