using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace MyDictionaryEng_Rus
{
    [Serializable]
    public class DictionaryEng_Rus<Tkey, TValue> : IDictionary<Tkey,TValue> where Tkey : IComparable
    {
        #region inner classes
        class DictionaryItem
        {
            public KeyValuePair<Tkey, TValue> _pair;
            public DictionaryItem _parent;
            public DictionaryItem _left;
            public DictionaryItem _right;
            public DictionaryItem(Tkey key, TValue value, DictionaryItem parent = null, DictionaryItem left = null, DictionaryItem right = null)
            {
                _pair = new KeyValuePair<Tkey, TValue>(key, value);
                _parent = parent;
                _left = left;
                _right = right;
            }
            public DictionaryItem(KeyValuePair<Tkey, TValue> pair, DictionaryItem parent = null, DictionaryItem left = null, DictionaryItem right = null)
            {
                _pair = pair;
                _parent = parent;
                _left = left;
                _right = right;
            }
        }
        #endregion

        #region fields
        private DictionaryItem _root = null;
        private bool _allowDublicateKeys;
        private int _count = 0;
        private List<Tkey> keysList = new List<Tkey>();
        private List<TValue> keysValue = new List<TValue>();
        #endregion

        #region properties and indexer
        public ICollection<Tkey> Keys => GetKeys();

        public ICollection<TValue> Values => GetValues();

        public int Count { get => _count; }

        public bool IsReadOnly { get => false; }

        public TValue this[Tkey key]
        {
            get => this.First(x => x.Key.Equals(key)).Value;

            set
            {
                Add(key, value);
            }
        }
        #endregion

        #region ctors
        /// <summary>
        /// Default constructor
        /// </summary>
        public DictionaryEng_Rus()
        {

        }

        public DictionaryEng_Rus(bool allowDublicateKeys = false)
        {
            _allowDublicateKeys = allowDublicateKeys;
        }
        /// <summary>
        /// Initializes a new inctance of MyDictionary class that contains elements copied from the specefied IDictionary
        /// </summary>
        /// <param name="dict">IDictionary collection</param>
        public DictionaryEng_Rus(IDictionary<Tkey, TValue> dictionary)
        {
            foreach (var d in dictionary)
            {
                Add(d);
            }
        }
        /// <summary>
        /// Initializes a new inctance of MyDictionary class from specified array KeyValuePair
        /// </summary>
        /// <param name="pairs">Pair from Key and Value</param>
        public DictionaryEng_Rus(params KeyValuePair<Tkey, TValue>[] pairs)
        {
            foreach (var p in pairs)
            {
                Add(p);
            }
        }
        #endregion

        #region private methods
        private List<Tkey> GetKeys()
        {
            keysList.Clear();
            foreach (var x in this)
            {
                keysList.Add(x.Key);
            }
            return keysList;
        }

        private List<TValue> GetValues()
        {
            keysValue.Clear();
            foreach (var x in this)
            {
                keysValue.Add(x.Value);
            }
            return keysValue;
        }

        private void Add(KeyValuePair<Tkey, TValue> pair, DictionaryItem item)
        {
            //если повторяются ключи
            if (!_allowDublicateKeys && pair.Key.CompareTo(item._pair.Key) == 0)
            {
                item._pair = pair;
            }
            //go to left
            else if (pair.Key.CompareTo(item._pair.Key) < 0)
            {
                if (item._left == null)
                {
                    item._left = new DictionaryItem(pair, item);
                    ++_count;
                }
                else
                {
                    Add(pair, item._left);
                }
            }
            else //go to right 
            {
                if (item._right == null)
                {
                    item._right = new DictionaryItem(pair, item);
                    ++_count;
                }
                else
                {
                    Add(pair, item._right);
                }
            }
        }
        /// <summary>
        /// Private method that remove item from the tree
        /// </summary>
        /// <param name="item">Item that removed</param>
        /// <param name="parent">Parent of the item</param>
        private void RemoveItem(DictionaryItem item, DictionaryItem parent)
        {
            if (item._left == null && item._right == null)
            {
                RemoveItemWithoutChildren(item, parent);
            }
            else if (item._left != null && item._right != null)
            {
                RemoveItemWithBothChildren(item, parent);
            }
            else
            {
                RemoveItemWithOneChild(item, parent);
            }
            --_count;
        }
        /// <summary>
        /// Method that remove item without children
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parent"></param>
        private void RemoveItemWithoutChildren(DictionaryItem item, DictionaryItem parent)
        {
            if (item == _root)
            {
                _root = null;
                return;
            }
            if (parent._left == item)
            {
                parent._left = null;
            }
            else
            {
                parent._right = null;
            }
        }
        /// <summary>
        /// Method that remove item with one child
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parent"></param>
        private void RemoveItemWithOneChild(DictionaryItem item, DictionaryItem parent)
        {
            DictionaryItem succ = item._right;
            DictionaryItem succparent = item;
            if (item._right != null)
            {
                while (succ._right != null)
                {
                    succparent = succ;
                    succ = succ._right;
                }
            }
            else
            {
                succ = item._left;
                while (succ._right != null)
                {
                    succparent = succ;
                    succ = succ._right;
                }
            }
            //replacevalue deleted node and successor
            item._pair = succ._pair;

            //delete successor 
            succparent._right = succ._left;
        }
        /// <summary>
        /// Method that remove item with both children
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parent"></param>
        private void RemoveItemWithBothChildren(DictionaryItem item, DictionaryItem parent)
        {
            //Find successor Node
            DictionaryItem success = item._right;
            DictionaryItem successParent = item;
            while (success._left != null)
            {
                successParent = success;
                success = success._left;
            }
            //replace value deleted node and successor
            item._pair = success._pair;
            //delete successor
            successParent._left = success._right;
        }
        /// <summary>
        /// Return the list of the items of the dictionary
        /// </summary>
        /// <returns></returns>
        private List<DictionaryItem> GetListItems()
        {
            List<DictionaryItem> resList = new List<DictionaryItem>();
            DictionaryItem item = _root;           
            Stack<DictionaryItem> itemStack = new Stack<DictionaryItem>();
            while (item != null || itemStack.Count != 0)
            {
                if (itemStack.Count != 0)
                {
                    item = itemStack.Pop();
                    //this is my action
                    resList.Add(item);

                    if (item._right != null)
                    {
                        item = item._right;
                    }
                    else
                    {
                        item = null;
                    }
                }
                while (item != null)
                {
                    itemStack.Push(item);
                    item = item._left;
                }
            }
            return resList;
        }

        private IEnumerator<DictionaryItem> GetTreeItemEnumerator(DictionaryItem item)
        {
            Stack<DictionaryItem> itemStack = new Stack<DictionaryItem>();
            while (item != null || itemStack.Count != 0)
            {
                if (itemStack.Count != 0)
                {
                    //извлекает елемент из стека
                    item = itemStack.Pop();
                    //this is my action
                    yield return item;
                    if (item._right != null)
                    {
                        item = item._right;
                    }
                    else
                    {
                        item = null;
                    }
                }
                while (item != null)
                {
                    //put
                    itemStack.Push(item);
                    item = item._left;
                }
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Return list of keys and values of binary tree
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<Tkey, TValue>> GetListItemsWithQueue()
        {
            List<KeyValuePair<Tkey, TValue>> resList = new List<KeyValuePair<Tkey, TValue>>();
            DictionaryItem item = _root;
            Queue<DictionaryItem> itemQueue = new Queue<DictionaryItem>();
            while (item != null || itemQueue.Count != 0)
            {
                if (itemQueue.Count != 0)
                {
                    item = itemQueue.Dequeue();
                    //this is my action
                    resList.Add(item._pair);

                    if (item._right != null)
                    {
                        item = item._right;
                    }
                    else
                    {
                        item = null;
                    }
                }
                while (item != null)
                {
                    itemQueue.Enqueue(item);
                    item = item._left;
                }
            }
            return resList;
        }
        /// <summary>
        /// Adds an element with the provided key and value to the MyDictionary
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add</param>
        /// <param name="value">The object to use as the value of the element to add</param>
        public void Add(Tkey key, TValue value)
        {
            if (_root == null)
            {
                _root = new DictionaryItem(key, value);
                ++_count;
            }
            else
            {
                Add(new KeyValuePair<Tkey, TValue>(key, value), _root);
            }
        }
        /// <summary>
        /// Adds an item to the MyDictionary
        /// </summary>
        /// <param name="pair">The object to add</param>
        public void Add(KeyValuePair<Tkey, TValue> pair)
        {
            if (_root == null)
            {
                _root = new DictionaryItem(pair);
                ++_count;
            }
            else
            {
                //Call a private method
                Add(pair, _root);
            }
        }
        /// <summary>
        /// Removes all items from the MyDictionary
        /// </summary>
        public void Clear()
        {
            _root = null;
            _count = 0;
        }
        /// <summary>
        /// Determines whether the MyDictionary contains a specific value
        /// </summary>
        /// <param name="item">The object to locate in the MyDictionary</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<Tkey, TValue> item)
        {
            foreach (var x in this)
            {
                if (x.Key.Equals(item.Key) && x.Value.Equals(item.Value))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determines whether the MyDictionaty contains an element with the specified key
        /// </summary>
        /// <param name="key">The key to locate in the MyDictionary</param>
        /// <returns></returns>
        public bool ContainsKey(Tkey key)
        {
            foreach (var x in this)
            {
                if (x.Key.Equals(key))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Copies the elements of the MyDictionary to an Array, starting at a particular Array index
        /// </summary>
        /// <param name="array">Array where the element would copied</param>
        /// <param name="arrayIndex">Index of the array</param>
        public void CopyTo(KeyValuePair<Tkey, TValue>[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("");
            }
            if (array == null)
            {
                throw new ArgumentNullException("");
            }
            if (_count > array.Length - arrayIndex)
            {
                throw new ArgumentException("");
            }
            using (IEnumerator<DictionaryItem> e = GetTreeItemEnumerator(_root))
            {
                while (e.MoveNext())
                {
                    array[arrayIndex++] = e.Current._pair;
                }
            }
        }

        public IEnumerator<KeyValuePair<Tkey, TValue>> GetEnumerator()
        {
            using (IEnumerator<DictionaryItem> e = GetTreeItemEnumerator(_root))
            {
                while (e.MoveNext())
                {
                    yield return e.Current._pair;
                }
            }
        }
        /// <summary>
        /// Removes the element with the specified key from the MyDictionary
        /// </summary>
        /// <param name="key">The key of the element to remove</param>
        /// <returns></returns>
        public bool Remove(Tkey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("");
            }
            List<DictionaryItem> listItems = GetListItems();
            if (ContainsKey(key))
            {
                RemoveItem(listItems.First(x => x._pair.Key.Equals(key)),
                    listItems.First(x => x._pair.Key.Equals(key))._parent);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the MyDictionary
        /// </summary>
        /// <param name="item">The object to remove</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<Tkey, TValue> item)
        {
            List<DictionaryItem> listItems = GetListItems();
            if (Contains(item))
            {
                RemoveItem(listItems.First(x => x._pair.Equals(item)),
                    listItems.First(x => x._pair.Equals(item))._parent);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the value associated with the specified key
        /// </summary>
        /// <param name="key">The key whose value to get</param>
        /// <param name="value">The value associated with the specified key</param>
        /// <returns></returns>
        public bool TryGetValue(Tkey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("");
            }
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = default(TValue);
            }
            return false;
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
