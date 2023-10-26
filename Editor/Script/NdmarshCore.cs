﻿using nadena.dev.ndmf;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using wataameya;

[assembly: ExportsPlugin(typeof(gomoru.su.Ndmarsh.NdmarshCore))]

namespace gomoru.su.Ndmarsh
{
    public sealed class NdmarshCore : Plugin<NdmarshCore>
    {
        public override string DisplayName => "Ndmarsh";
        public override string QualifiedName => "gomoru.su.Ndmarsh";

        protected override void Configure() => InPhase(BuildPhase.Generating).Run("Generate Marshmallow PB", Generate);

        private void Generate(BuildContext context)
        {
            if (context.AvatarRootTransform.Find("marshmallow_PB")  != null)
                return; // Maybe already applied

            var ndmarsh = context.AvatarRootObject.GetComponentInChildren<Ndmarsh>();
            if (ndmarsh == null)
                return;

            var dummy = new GameObject("Dummy") { hideFlags = HideFlags.HideAndDontSave };

            List<Object> toDestroy = new List<Object>();
            toDestroy.Add(ndmarsh);

            marshmallow_PB_Setup marshmallow = null;
            try
            {
                marshmallow = ScriptableObject.CreateInstance<marshmallow_PB_Setup>();
                toDestroy.Add(marshmallow);

                var mpb = marshmallow.ToAccessor();
                bool isReplaced = false;
                dummy.AddComponent<DeactivateCallback>().Callback = () =>
                {
                    if (isReplaced)
                        return;

                    toDestroy.Add(mpb.Avatar);
                    toDestroy.Add(mpb.CopiedAvatar);

                    mpb.Avatar = mpb.CopiedAvatar = context.AvatarRootObject;
                    isReplaced = true;
                };
                mpb.ImportParameters(ndmarsh);
                mpb.AsModularAvatarModule = true;
                mpb.Avatar = dummy;

                mpb.Localize();
                var result = mpb.SetupWithInitialize();

            }
            finally
            {
                foreach(var x in toDestroy)
                {
                    if (x != null)
                        Object.DestroyImmediate(x);
                }
            }

        }
    }
}