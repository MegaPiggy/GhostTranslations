using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostTranslations.Collections
{

    public struct LightEnumerator : IEnumerator
    {

        private object _e;
        private object _current;
        private int _index;

        #region CONSTRUCTOR

        public LightEnumerator(IEnumerable e)
        {
            if (e is IList)
            {
                _e = e;
                _index = 0;
            }
            else
            {
                _e = e.GetEnumerator();
                _index = -1;
            }
            _current = null;
        }

        #endregion


        #region IEnumerator Interface

        public object Current
        {
            get
            {
                return _current;
            }
        }

        public void Dispose()
        {
            _e = null;
            _current = null;
            _index = -1;
        }

        public bool MoveNext()
        {
            if (_e == null) return false;

            if (_index < 0)
            {
                var e = _e as IEnumerator;
                if (e.MoveNext())
                {
                    _current = e.Current;
                    return true;
                }
                else
                {
                    _current = null;
                    return false;
                }
            }
            else
            {
                var lst = _e as IList;
                if (_index < lst.Count)
                {
                    _current = lst[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _current = null;
                    return false;
                }
            }
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        #endregion



        public static LightEnumerator Create(IEnumerable e)
        {
            return new LightEnumerator(e);
        }

        public static LightEnumerator<T> Create<T>(IEnumerable<T> e)
        {
            return new LightEnumerator<T>(e);
        }

    }

    public struct LightEnumerator<T> : IEnumerator<T>
    {

        private object _e;
        private T _current;
        private int _index;

        #region CONSTRUCTOR

        public LightEnumerator(IEnumerable<T> e)
        {
            if (e is IList<T>)
            {
                _e = e;
                _index = 0;
            }
            else
            {
                _e = e.GetEnumerator();
                _index = -1;
            }
            _current = default(T);
        }

        #endregion


        #region IEnumerator Interface

        public T Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public void Dispose()
        {
            _e = null;
            _current = default(T);
            _index = -1;
        }

        public bool MoveNext()
        {
            if (_e == null) return false;

            if (_index < 0)
            {
                var e = _e as IEnumerator<T>;
                if (e.MoveNext())
                {
                    _current = e.Current;
                    return true;
                }
                else
                {
                    _current = default(T);
                    return false;
                }
            }
            else
            {
                var lst = _e as IList<T>;
                if (_index < lst.Count)
                {
                    _current = lst[_index];
                    _index++;
                    return true;
                }
                else
                {
                    _current = default(T);
                    return false;
                }
            }
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

    public interface ICachePool<T> where T : class
    {

        T GetInstance();

        void Release(T obj);

    }

    /// <summary>
    /// Creates a pool that will cache instances of objects for later use so that you don't have to construct them again. 
    /// There is a max cache size, if set to 0 or less, it's considered endless in size.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectCachePool<T> : ICachePool<T> where T : class
    {

        private const int DEFAULT_CACHESIZE = 64; //1024;

        #region Fields

        private HashSet<T> _inactive;

        private int _cacheSize;
        private Func<T> _constructorDelegate;
        private Action<T> _resetObjectDelegate;
        private bool _resetOnGet;

        #endregion

        #region CONSTRUCTOR

        public ObjectCachePool(int cacheSize)
        {
            this.CacheSize = cacheSize;
            //_inactive = (_cacheSize <= 0) ? new Bag<T>() : new Bag<T>(_cacheSize);
            _inactive = new HashSet<T>();
            _constructorDelegate = this.SimpleConstructor;
        }

        public ObjectCachePool(int cacheSize, Func<T> constructorDelegate)
        {
            this.CacheSize = cacheSize;
            //_inactive = (_cacheSize <= 0) ? new Bag<T>() : new Bag<T>(_cacheSize);
            _inactive = new HashSet<T>();
            _constructorDelegate = (constructorDelegate != null) ? constructorDelegate : this.SimpleConstructor;
        }

        public ObjectCachePool(int cacheSize, Func<T> constructorDelegate, Action<T> resetObjectDelegate)
        {
            this.CacheSize = cacheSize;
            //_inactive = (_cacheSize <= 0) ? new Bag<T>() : new Bag<T>(_cacheSize);
            _inactive = new HashSet<T>();
            _constructorDelegate = (constructorDelegate != null) ? constructorDelegate : this.SimpleConstructor;
            _resetObjectDelegate = resetObjectDelegate;
        }

        public ObjectCachePool(int cacheSize, Func<T> constructorDelegate, Action<T> resetObjectDelegate, bool resetOnGet)
        {
            this.CacheSize = cacheSize;
            //_inactive = (_cacheSize <= 0) ? new Bag<T>() : new Bag<T>(_cacheSize);
            _inactive = new HashSet<T>();
            _constructorDelegate = (constructorDelegate != null) ? constructorDelegate : this.SimpleConstructor;
            _resetObjectDelegate = resetObjectDelegate;
            _resetOnGet = resetOnGet;
        }

        private T SimpleConstructor()
        {
            return Activator.CreateInstance<T>();
        }

        #endregion

        #region Properties

        public int CacheSize
        {
            get { return _cacheSize; }
            set
            {
                _cacheSize = value > 0 ? value : DEFAULT_CACHESIZE;
            }
        }

        public bool ResetOnGet
        {
            get { return _resetOnGet; }
            set { _resetOnGet = value; }
        }

        public int InactiveCount
        {
            get { return _inactive.Count; }
        }

        #endregion

        #region Methods

        public bool TryGetInstance(out T result)
        {
            result = null;
            lock (_inactive)
            {
                if (_inactive.Count > 0)
                {
                    result = PopInactive();
                }
            }
            if (result != null)
            {
                if (_resetOnGet && _resetObjectDelegate != null)
                    _resetObjectDelegate(result);
                return true;
            }
            else
            {
                return false;
            }
        }

        public T PopInactive()
        {
            if (_inactive == null) throw new System.ArgumentNullException("_inactive");

            var e = _inactive.GetEnumerator();
            if (e.MoveNext())
            {
                _inactive.Remove(e.Current);
                return e.Current;
            }

            throw new System.ArgumentException("HashSet must not be empty.");
        }

        public T GetInstance()
        {
            T result = null;
            lock (_inactive)
            {
                if (_inactive.Count > 0)
                {
                    result = PopInactive();
                }
            }
            if (result != null)
            {
                if (_resetOnGet && _resetObjectDelegate != null)
                    _resetObjectDelegate(result);
                return result;
            }
            else
            {
                return _constructorDelegate();
            }
        }

        public bool Release(T obj)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            if (!_resetOnGet && _resetObjectDelegate != null && _inactive.Count < _cacheSize) _resetObjectDelegate(obj);

            lock (_inactive)
            {
                if (_inactive.Count < _cacheSize)
                {
                    _inactive.Add(obj);
                    return true;
                }
            }

            return false;
        }

        void ICachePool<T>.Release(T obj)
        {
            this.Release(obj);
        }

        public bool IsTreatedAsInactive(T obj)
        {
            return _inactive.Contains(obj);
        }

        #endregion

    }

    public interface ITempCollection<T> : ICollection<T>, IDisposable
    {

    }

    /// <summary>
    /// This is intended for a short lived collection that needs to be memory efficient and fast. 
    /// Call the static 'GetCollection' method to get a cached collection for use. 
    /// When you're done with the collection you call Release to make it available for reuse again later. 
    /// Do NOT use it again after calling Release.
    /// 
    /// Due to the design of this, it should only ever be used in a single threaded manner. Primarily intended 
    /// for the main Unity thread. 
    /// 
    /// If you're in a separate thread, it's best to cache your own list local to there, and don't even bother with 
    /// this.
    /// </summary>
    public static class TempCollection
    {

        #region Static Interface

        /// <summary>
        /// Returns the any available collection for use generically. 
        /// The collection could be a HashSet, List, or any temp implementation. 
        /// This is intended to reduce the need for creating a new collection 
        /// unnecessarily.
        /// </summary>
        /// <returns></returns>
        public static ITempCollection<T> GetCollection<T>()
        {
            return GetList<T>();
        }

        /// <summary>
        /// Returns the any available collection for use generically. 
        /// The collection could be a HashSet, List, or any temp implementation. 
        /// This is intended to reduce the need for creating a new collection 
        /// unnecessarily.
        /// </summary>
        /// <returns></returns>
        public static ITempCollection<T> GetCollection<T>(IEnumerable<T> e)
        {
            return GetList<T>(e);
        }


        public static TempList<T> GetList<T>()
        {
            return TempList<T>.GetList();
        }

        public static TempList<T> GetList<T>(IEnumerable<T> e)
        {
            return TempList<T>.GetList(e);
        }

        public static TempList<T> GetList<T>(int count)
        {
            return TempList<T>.GetList(count);
        }

        #endregion

    }

    public class TempList<T> : List<T>, ITempCollection<T>
    {

        private const int MAX_SIZE_INBYTES = 1024;

        #region Fields

        private static ObjectCachePool<TempList<T>> _pool = new ObjectCachePool<TempList<T>>(-1, () => new TempList<T>());

        private int _maxCapacityOnRelease;
        private int _version;

        #endregion

        #region CONSTRUCTOR

        public TempList()
            : base()
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
        }

        public TempList(IEnumerable<T> e)
            : base(e)
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
        }

        public TempList(int count)
            : base(count)
        {
            var tp = typeof(T);
            int sz = (tp.IsValueType && !tp.IsEnum) ? System.Runtime.InteropServices.Marshal.SizeOf(tp) : 4;
            _maxCapacityOnRelease = MAX_SIZE_INBYTES / sz;
            _version = 1;
        }

        #endregion

        #region IDisposable Interface

        public virtual void Dispose()
        {
            this.Clear();
            if (_pool.Release(this))
            {
                if (this.Capacity > _maxCapacityOnRelease / Math.Min(_version, 4))
                {
                    this.Capacity = _maxCapacityOnRelease / Math.Min(_version, 4);
                    _version = 1;
                }
                else
                {
                    _version++;
                }
            }
        }

        #endregion

        #region Static Methods

        public static TempList<T> GetList()
        {
            return _pool.GetInstance();
        }

        public static TempList<T> GetList(IEnumerable<T> e)
        {
            TempList<T> result;
            if (_pool.TryGetInstance(out result))
            {
                //result.AddRange(e);
                var e2 = new LightEnumerator<T>(e);
                while (e2.MoveNext())
                {
                    result.Add(e2.Current);
                }
            }
            else
            {
                result = new TempList<T>(e);
            }
            return result;
        }

        public static TempList<T> GetList(int count)
        {
            TempList<T> result;
            if (_pool.TryGetInstance(out result))
            {
                if (result.Capacity < count) result.Capacity = count;
                return result;
            }
            else
            {
                result = new TempList<T>(count);
            }
            return result;
        }

        #endregion

    }
}
