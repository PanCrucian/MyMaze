﻿using UnityEngine;
using System.Collections;

namespace DeltaDNA
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
	
		private static object _lock = new object();
	
		public static T Instance
		{
			get
			{
				if (applicationIsQuitting) {
					Logger.LogWarning("[Singleton] Instance '"+ typeof(T) +
					                 "' already destroyed on application quit." +
					                 " Won't create again - returning null.");
					return null;
				}
	
				lock(_lock)
				{
					if (_instance == null) {
						_instance = (T) FindObjectOfType(typeof(T));
	
						if (FindObjectsOfType(typeof(T)).Length > 1) {
							Logger.LogError("[Singleton] Something went really wrong " +
							               " - there should never be more than 1 singleton!" +
							               " Reopening the scene might fix it.");
							return _instance;
						}
	
						if (_instance == null) {
							GameObject singleton = new GameObject();
							_instance = singleton.AddComponent<T>();
							singleton.name = typeof(T).ToString();
	
							DontDestroyOnLoad(singleton);
	
							Logger.LogDebug("[Singleton] An instance of " + typeof(T) +
							          " is needed in the scene, so '" + singleton +
							          "' was created with DontDestroyOnLoad.");
						} else {
							Logger.LogDebug("[Singleton] Using instance already created: " +
							          _instance.gameObject.name);
						}
					}
					return _instance;
				}
			}
		}
	
		private static bool applicationIsQuitting = false;
		///<summary>
		/// When unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed,
		/// it will create a buggy ghost object that will stay on the Editor scene
		/// even after stopping playing the Application.  Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		public virtual void OnDestroy () {
			Logger.LogDebug("[Singleton] Destroying an instance of " + typeof(T));
			applicationIsQuitting = true;
		}
	}
} 
// namespace DeltaDNA

