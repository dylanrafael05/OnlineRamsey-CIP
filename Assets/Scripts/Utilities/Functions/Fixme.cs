using UnityEngine;

namespace Ramsey.Utilities
{
    public class Fixme : MonoBehaviour
    {
        private void Awake()
        {
            Update();
        }

        private void Update()
        {
            Debug.Log(transform.position.z);
        }
    }
}