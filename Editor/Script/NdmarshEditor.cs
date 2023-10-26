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

            var mpb = _marshmallowPB.ToAccessor();
            mpb.ImportParameters(ndmarsh);

            var fontSize = EditorStyles.helpBox.fontSize;
            EditorStyles.helpBox.fontSize = 12;
            try
            {
                if (avatar == null)
                {
                    if (mpb.Text.Count == 0)
                        mpb.Localize();
                    EditorGUILayout.HelpBox(mpb.Language == 0 ? "このコンポーネントが正しく動作するには、アバター内に配置する必要があります。" : "This component needs to be placed inside your avatar to work properly.", MessageType.Warning);
                    return;
                }

                EditorGUI.BeginChangeCheck();

                mpb.AsModularAvatarModule = true;

                var position = _marshmallowPB.position;
                position.size = new Vector2(EditorGUIUtility.labelWidth * 2, position.size.y);
                _marshmallowPB.position = position;

                EditorGUI.indentLevel++;
                mpb.OnGUI();
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox(mpb.Language == 0 ? "設定開始ボタンは押さないでください！！" : "Do not press the start button !!", MessageType.Warning);

                if (EditorGUI.EndChangeCheck())
                {
                    mpb.ExportParameters(ndmarsh);
                    EditorUtility.SetDirty(ndmarsh);
                }
            }
            finally
            {
                EditorStyles.helpBox.fontSize = fontSize;
            }
        }
    }
}
