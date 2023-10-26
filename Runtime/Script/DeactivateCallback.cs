using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.SDKBase;

namespace gomoru.su.Ndmarsh
{
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public sealed class DeactivateCallback : MonoBehaviour, IEditorOnly
    {
        public Action Callback;

        internal void OnDisable()
        {
            Callback?.Invoke();
            Callback = null;
        }
    }
}
