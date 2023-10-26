using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.Dynamics;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace gomoru.su.Ndmarsh
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [AddComponentMenu("Marshmallow PB/Non-Destructive Marshmallow PB")]
    public sealed class Ndmarsh : MonoBehaviour, IEditorOnly
    {
        [Alias("_avatar")]
        [Alias("_prevavatar")]
        public GameObject Avatar;

        [Alias("_index")]
        public int Index;

        [Alias("_previndex")]
        public int PrevIndex = -1;

        [Alias("_Breast_L")]
        public GameObject BreastL;

        [Alias("_Breast_R")]
        public GameObject BreastR;

        [Alias("_PhysBone_index")]
        public int PresetIndex = 2;

        [Alias("_prevPhysBone_index")]
        public int PrevPresetIndex = -1;

        [Alias("_breast_blendshape")]
        public float BreastBlendshape;

        [Alias("_PhysBone_collider")]
        public VRCPhysBoneColliderBase[] Colliders = new VRCPhysBoneColliderBase[5];

        [Alias("_floor")]
        public bool AllowFloorCollider = true;
        [Alias("_writedefaults")]
        public bool Writedefaults;
        [Alias("_interference")]
        public bool Interference = true;
        [Alias("_squishPB")]
        public bool SquishPB;
        [Alias("_interference_squishPB")]
        public bool InterferenceSquishPB;

        [Alias("_breast_scale")]
        public float BreastScale = 1;
        [Alias("_limit_collider_position_z")]
        public float LimitColliderPositionZ = 0.135f;
        [Alias("_breast_collider_radius")]
        public float BreastColliderRadius = 0.1f;
        [Alias("_rotation_constraint_weight")]
        public float RotationConstraintWeight = 0.8f;
        [Alias("_scale_constraint_weight")]
        public float ScaleConstraintWeight = 1;

        [Alias("_PhysBone_Pull")]
        public float PhysBonePull;
        [Alias("_PhysBone_Momentum")]
        public float PhysBoneMomentum;
        [Alias("_PhysBone_Stiffness")]
        public float PhysBoneStiffness;
        [Alias("_PhysBone_Gravity")]
        public float PhysBoneGravity;
        [Alias("_PhysBone_GravityFalloff")]
        public float PhysBoneGravityFalloff;
        [Alias("_PhysBone_Immobile")]
        public float PhysBoneImmobile;
        [Alias("_PhysBone_Limit_Angle")]
        public float PhysBoneLimitAngle;
        [Alias("_PhysBone_Collision_Radius")]
        public float PhysBoneCollisionRadius;

        public void Start()
        {
            var avatar = GetComponentInParent<VRCAvatarDescriptor>();
            if (avatar == null)
                return;

            Avatar = avatar.gameObject;

            if (BreastL != null && BreastR != null)
            {
                var x = BreastL.transform.localScale.y;
                var y = BreastR.transform.localScale.y;
                if (x == y)
                {
                    BreastScale = x;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class AliasAttribute : Attribute
    {
        public AliasAttribute(string name) => Name = name;
        public string Name { get; }
    }
}