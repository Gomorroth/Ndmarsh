using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Avatars.Components;
using wataameya;

namespace gomoru.su.Ndmarsh
{
    [CustomEditor(typeof(Ndmarsh))]
    internal sealed class NdmarshEditor : Editor
    {
        private static marshmallow_PB_Setup _marshmallowPB;

        internal void OnEnable()
        {
            if (_marshmallowPB == null)
            {
                _marshmallowPB = CreateInstance<marshmallow_PB_Setup>();
            }
        }

        public override void OnInspectorGUI()
        {
            var ndmarsh = target as Ndmarsh;
            var avatar = ndmarsh.GetComponentInParent<VRCAvatarDescriptor>();
            if (avatar == null)
            {
                EditorGUILayout.HelpBox("🤔", MessageType.Warning);
            }

            var mpb = _marshmallowPB.ToAccessor();
            mpb.ImportParameters(ndmarsh);

            EditorGUI.BeginChangeCheck();

            mpb.AsModularAvatarModule = true;

            var position = _marshmallowPB.position;
            position.size = new Vector2(EditorGUIUtility.labelWidth * 2, position.size.y);
            _marshmallowPB.position = position;

            EditorGUI.indentLevel++;
            mpb.OnGUI();
            EditorGUI.indentLevel--;
            var helpBox = EditorStyles.helpBox;
            var size = helpBox.fontSize;
            helpBox.fontSize = 12;
            EditorGUILayout.HelpBox(mpb.Language == 0 ? "設定開始ボタンは押さないでください！！" : "Do not press the start button !!", MessageType.Warning);
            helpBox.fontSize = size;

            if (EditorGUI.EndChangeCheck())
            {
                mpb.ExportParameters(ndmarsh);
                EditorUtility.SetDirty(ndmarsh);
            }
        }
    }
}
