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
        private SerializedProperties _properties;

        private struct SerializedProperties
        {
            public SerializedProperty BreastL;
            public SerializedProperty BreastR;
            public SerializedProperty Colliders;
            public SerializedProperty Floor;
            public SerializedProperty Writedefaults;
            public SerializedProperty Interference;
            public SerializedProperty SquishPB;
            public SerializedProperty InterferenceSquishPB;
            public SerializedProperty BreastScale;
            public SerializedProperty LimitColliderPositionZ;
            public SerializedProperty BreastColliderRadius;
            public SerializedProperty RotationConstraintWeight;
            public SerializedProperty ScaleConstraintWeight;
            public SerializedProperty PhysBonePull;
            public SerializedProperty PhysBoneMomentum;
            public SerializedProperty PhysBoneStiffness;
            public SerializedProperty PhysBoneGravity;
            public SerializedProperty PhysBoneGravityFalloff;
            public SerializedProperty PhysBoneImmobile;
            public SerializedProperty PhysBoneLimitAngle;
            public SerializedProperty PhysBoneCollisionRadius;

            public SerializedProperties(SerializedObject serializedObject)
            {
                BreastL = serializedObject.FindProperty(nameof(Ndmarsh.BreastL));
                BreastR = serializedObject.FindProperty(nameof(Ndmarsh.BreastR));
                Colliders = serializedObject.FindProperty(nameof(Ndmarsh.Colliders));
                Floor = serializedObject.FindProperty(nameof(Ndmarsh.AllowFloorCollider));
                Writedefaults = serializedObject.FindProperty(nameof(Ndmarsh.Writedefaults));
                Interference = serializedObject.FindProperty(nameof(Ndmarsh.Interference));
                SquishPB = serializedObject.FindProperty(nameof(Ndmarsh.SquishPB));
                InterferenceSquishPB = serializedObject.FindProperty(nameof(Ndmarsh.InterferenceSquishPB));
                BreastScale = serializedObject.FindProperty(nameof(Ndmarsh.BreastScale));
                LimitColliderPositionZ = serializedObject.FindProperty(nameof(Ndmarsh.LimitColliderPositionZ));
                BreastColliderRadius = serializedObject.FindProperty(nameof(Ndmarsh.BreastColliderRadius));
                RotationConstraintWeight = serializedObject.FindProperty(nameof(Ndmarsh.RotationConstraintWeight));
                ScaleConstraintWeight = serializedObject.FindProperty(nameof(Ndmarsh.ScaleConstraintWeight));
                PhysBonePull = serializedObject.FindProperty(nameof(Ndmarsh.PhysBonePull));
                PhysBoneMomentum = serializedObject.FindProperty(nameof(Ndmarsh.PhysBoneMomentum));
                PhysBoneStiffness = serializedObject.FindProperty(nameof(Ndmarsh.PhysBoneStiffness));
                PhysBoneGravity = serializedObject.FindProperty(nameof(Ndmarsh.PhysBoneGravity));
                PhysBoneGravityFalloff = serializedObject.FindProperty(nameof(Ndmarsh.PhysBoneGravityFalloff));
                PhysBoneImmobile = serializedObject.FindProperty(nameof(Ndmarsh.PhysBoneImmobile));
                PhysBoneLimitAngle = serializedObject.FindProperty(nameof(Ndmarsh.PhysBoneLimitAngle));
                PhysBoneCollisionRadius = serializedObject.FindProperty(nameof(Ndmarsh.PhysBoneCollisionRadius));
            }
        }

        internal void OnEnable()
        {
            if (_marshmallowPB == null)
            {
                _marshmallowPB = CreateInstance<marshmallow_PB_Setup>();
                var position = _marshmallowPB.position;
                position.size = new Vector2(EditorGUIUtility.labelWidth * 3.5f, position.size.y);
                _marshmallowPB.position = position;
            }
            var ndmarsh = target as Ndmarsh;
            _marshmallowPB.ToAccessor().ImportParameters(ndmarsh);
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

            EditorGUI.indentLevel++;
            try
            {
                EditorGUI.BeginChangeCheck();

                mpb.AsModularAvatarModule = true;
                mpb.OnGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    mpb.ExportParameters(ndmarsh);
                    EditorUtility.SetDirty(ndmarsh);
                }
            }
            finally
            {
                EditorGUI.indentLevel--;
            }

        }
    }
}
