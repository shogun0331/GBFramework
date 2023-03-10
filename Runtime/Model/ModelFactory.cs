using System;
using System.Collections.Generic;
using UnityEngine;
using GB.Global;

namespace GB
{
    ///-----------------------------------------------------------------------
    /// <remarks>
    /// <copyright file="ModelFactory.cs" company="GB">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2021 GB
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <list type="table">
    /// <listheader>
    /// <term>Revision</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>
    /// <strong>Version:</strong> 0.1.0-exp<br/>
    /// <strong>Date:</strong> 11/28/2021<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>Initial verison.</description>
    /// </item><item>
    /// <term>
    /// <strong>Version:</strong> 0.2.0-exp.1<br/>
    /// <strong>Date:</strong> 3/2/2022<br/>
    /// <strong>Author:</strong> Taro Omiya
    /// </term>
    /// <description>
    /// Changing key from <c>string</c> to <c>object</c>.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Factory that creates and maintains static instances of
    /// <seealso cref="IModel"/>s.
    /// </summary>
    public class ModelFactory : MonoBehaviour
    {
        const HideFlags Flags = HideFlags.HideInHierarchy | HideFlags.DontSave;

        struct KeyPair : IEquatable<KeyPair>, IEqualityComparer<KeyPair>
        {
            public KeyPair(Type type, object key)
            {
                // Setup member properties
                Type = type;
                Key = key;
            }

            public Type Type
            {
                get;
            }
            public object Key
            {
                get;
            }

            public bool Equals(KeyPair other) => Equals(this, other);
            public bool Equals(KeyPair x, KeyPair y) => Equals(x.Type, y.Type) && Equals(x.Key, y.Key);
            public int GetHashCode(KeyPair obj)
            {
                int returnHash = Type.GetHashCode();
                if (Key != null)
                {
                    returnHash ^= Key.GetHashCode();
                }
                return returnHash;
            }
        }

        readonly Dictionary<KeyPair, IModel> keyToModelMap = new Dictionary<KeyPair, IModel>();
        readonly HashSet<IModel> modelSet = new HashSet<IModel>();

        /// <summary>
        /// Gets the sole instance of this factory.
        /// </summary>
        public static ModelFactory Instance => Application.isPlaying ? ComponentSingleton<ModelFactory>.Get(Flags) : null;
        /// <summary>
        /// Gets all the created models. Order is not guaranteed.
        /// </summary>
        public static IEnumerable<IModel> AllModels => ModelSet;
        /// <summary>
        /// Number of models created so far.
        /// </summary>
        public static int NumberOfModels => KeyToModelMap.Count;
        /// <summary>
        /// Set to <c>true</c> to make <seealso cref="Get{T}(object)"/>
        /// lazy-load <see cref="IModel"/>s.
        /// </summary>
        /// <remarks>
        /// Only works on <c>UNITY_EDITOR</c> builds.
        /// </remarks>
        public bool IsGetLazy
        {
#if !UNITY_EDITOR
			get => false;
			set { }
#else
            get;
            set;
#endif
        }
        static Dictionary<KeyPair, IModel> KeyToModelMap => Instance.keyToModelMap;
        static HashSet<IModel> ModelSet => Instance.modelSet;

        /// <summary>
        /// Creates a unique <see cref="IModel"/>,
        /// optionally assoiciated with a <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IModel"/> created.
        /// </typeparam>
        /// <param name="key">
        /// To create multiple instances of the same <see cref="IModel"/>,
        /// supply a unique key to be associated with each of them.
        /// </param>
        /// <returns>
        /// The newly constructed <see cref="IModel"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If a <typeparamref name="T"/> model with the same <see cref="IModel.Key"/>
        /// as <paramref name="key"/> has already been created.
        /// </exception>
        /// <seealso cref="IModel"/>
        /// <seealso cref="Get{T}(object)"/>
        public static T Create<T>(object key = null) where T : Component, IModel
        {
            // Construct a key
            var pair = new KeyPair(typeof(T), key);

            // Check if the key already exists in the dictionary
            if (KeyToModelMap.ContainsKey(pair) == true)
            {
                // Don't let the code proceed
                throw new ArgumentException($"Model \"{typeof(T).Name}\" with key \"{key}\" has already been created.", "key");
            }

            // Create the component
            T returnComponent = Instance.gameObject.AddComponent<T>();

            // Add the component to the map before initializing
            KeyToModelMap.Add(pair, returnComponent);
            ModelSet.Add(returnComponent);
            returnComponent.OnCreate(key, Instance);

            // Return the component
            return returnComponent;
        }

        /// <summary>
        /// Checks if factory created a model with key.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IModel"/>.
        /// </typeparam>
        /// <param name="key">
        /// Optional key associated with model.
        /// </param>
        /// <returns>
        /// <c>true</c> if key is found in the factory.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is <c>null</c>.
        /// </exception>
        public static bool Contains<T>(object key = null)
        {
            return KeyToModelMap.ContainsKey(new KeyPair(typeof(T), key));
        }

        /// <summary>
        /// Checks if the factory has <paramref name="model"/>
        /// stored in its dictionary.
        /// </summary>
        /// <param name="model">
        /// The instance to check if factory contains it.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="model"/> is found in the factory.
        /// </returns>
        public static bool Contains(IModel model)
        {
            return ModelSet.Contains(model);
        }

        /// <summary>
        /// Gets an existing <see cref="IModel"/>,
        /// created by <see cref="Create{T}(object)"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IModel"/> to get.
        /// </typeparam>
        /// <param name="key">
        /// Optional key associated with an <see cref="IModel"/>.
        /// </param>
        /// <returns>
        /// Associated <see cref="IModel"/> with <paramref name="key"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If a <typeparamref name="T"/> model with the same <see cref="IModel.Key"/>
        /// as <paramref name="key"/> has not been created yet.
        /// </exception>
        /// <seealso cref="IModel"/>
        /// <seealso cref="Create{T}(object)"/>
        public static T Get<T>(object key = null) where T : Component, IModel
        {
            // Construct a key
            var pair = new KeyPair(typeof(T), key);

            // Check if the key exists in the dictionary
            if (KeyToModelMap.TryGetValue(pair, out IModel model) == false)
            {
#if !UNITY_EDITOR
				// If not, don't let the code proceed
				throw new ArgumentException($"Model \"{typeof(T).Name}\" with key \"{key}\" has not been created yet.", "key");
#else
                // If not, check if Get is allowed to lazy-load a model
                if (Instance.IsGetLazy)
                {
                    // If so, create a new model
                    model = Create<T>(key);
                    Debug.LogWarning($"Lazy-loaded Model \"{typeof(T).Name}\" with key \"{key}\".\nAs this is not the usual behavior, some strange stuff may occur.");
                }
                else
                {
                    // If not, don't let the code proceed
                    throw new ArgumentException($"Model \"{typeof(T).Name}\" with key \"{key}\" has not been created yet.", "key");
                }
#endif
            }

            // Cast the model
            return (T)model;
        }

        /// <summary>
        /// Attempts to get an existing <see cref="IModel"/>,
        /// created by <see cref="Create{T}(object)"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IModel"/> to get.
        /// </typeparam>
        /// <param name="key">
        /// Key associated with an <see cref="IModel"/>.
        /// </param>
        /// <param name="model">
        /// Associated <see cref="IModel"/> with <paramref name="key"/>,
        /// or <c>null</c> if one isn't found.
        /// </param>
        /// <returns>
        /// <c>true</c> if a created <see cref="IModel"/> was found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <seealso cref="IModel"/>
        /// <seealso cref="Create{T}(object)"/>
        public static bool TryGet<T>(object key, out T model) where T : Component, IModel
        {
            // Construct a key
            var pair = new KeyPair(typeof(T), key);

            // Default return model
            model = null;

            // Check if the key exists in the dictionary
            bool returnFlag = KeyToModelMap.TryGetValue(pair, out IModel returnModel);
            if (returnFlag)
            {
                // Cast the model
                model = (T)returnModel;
            }

            // Return flag
            return returnFlag;
        }

        /// <summary>
        /// Attempts to get an existing <see cref="IModel"/>,
        /// created by <see cref="Create{T}()"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IModel"/> to get.
        /// </typeparam>
        /// <param name="model">
        /// An <see cref="IModel"/> created prior,
        /// or <c>null</c> if one isn't found.
        /// </param>
        /// <returns>
        /// <c>true</c> if a created <see cref="IModel"/> was found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <seealso cref="IModel"/>
        /// <seealso cref="Create{T}(object)"/>
        public static bool TryGet<T>(out T model) where T : Component, IModel => TryGet(null, out model);

        /// <summary>
        /// Attempts to destroy an <see cref="IModel"/>,
        /// created by <see cref="Create{T}(object)"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IModel"/> to destroy.
        /// </typeparam>
        /// <param name="key">
        /// Key associated with an <see cref="IModel"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a created <see cref="IModel"/> was destroyed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <seealso cref="Create{T}(object)"/>
        public static bool Release<T>(object key = null) where T : Component, IModel
        {
            // Construct a key
            var pair = new KeyPair(typeof(T), key);

            // Check if the key exists in the dictionary
            bool returnFlag = KeyToModelMap.TryGetValue(pair, out IModel model);
            if (returnFlag)
            {
                // Remove from records
                KeyToModelMap.Remove(pair);
                ModelSet.Remove(model);

                // Destroy the model
                Helpers.Destroy((T)model);
            }

            // Return flag
            return returnFlag;
        }

        /// <summary>
        /// Destroys all the created <see cref="IModel"/>.
        /// </summary>
        public static void Reset()
        {
            // Just destroy this script (and its member varibles) entirely
            ComponentSingleton<ModelFactory>.Release();
        }
    }
}
