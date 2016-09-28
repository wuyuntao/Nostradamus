using System.Collections.Generic;
using UnityEngine;

namespace Nostradamus.Examples
{
	public class ObjectPool : MonoBehaviour
	{
		private static SortedList<string, GameObject> cachedObjects = new SortedList<string, GameObject>();

		private void Awake()
		{
			for( int i = 0; i < transform.childCount; i++ )
			{
				var child = transform.GetChild( i );
				cachedObjects.Add( child.name, child.gameObject );
			}

			gameObject.SetActive( false );
		}

		public static GameObject Instantiate(string name)
		{
			return Instantiate( cachedObjects[name] );
		}

		public static GameObject Instantiate(string name, Transform parent)
		{
			return (GameObject)Instantiate( cachedObjects[name], parent );
		}

		public static GameObject Instantiate(string name, Vector3 position, Quaternion rotation, Transform parent)
		{
			return (GameObject)Instantiate( cachedObjects[name], position, rotation, parent );
		}
	}
}