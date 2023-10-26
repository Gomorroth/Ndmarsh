using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using Self = wataameya.marshmallow_PB_Setup;

namespace gomoru.su.Ndmarsh
{
    [InitializeOnLoad]
    internal static class MarshmallowMarshal
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        static MarshmallowMarshal()
        {
            _AsModularAvatarModule = CreateProperty<bool>("_modularavatar");
            _Avatar = CreateProperty<GameObject>("_avatar");
            _CopiedAvatar = CreateProperty<GameObject>("_avatar_copy");
            _Language = CreateProperty<int>("_lang");

            _OnGUI = CreateMethod<Action<Self>>("OnGUI");
            _Localize = CreateMethod<Action<Self>>("Localize");


            _SetupWithInitialize = CreateMethod<Func<Self, string>>("SetupWithInitialize", il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, typeof(Self).GetMethod("Initialize", Private));
                il.Emit(OpCodes.Callvirt, typeof(Self).GetMethod("Setup", Private));
                il.Emit(OpCodes.Ret);
            });

            {
                var types = DelegateInfoCache<ParameterIODelegate>.ArgumentTypes;
                var import = new DynamicMethod("ImportParameters", null, types, typeof(Self), true);
                var export = new DynamicMethod("ExportParameters", null, types, typeof(Self), true);
                var ilI = import.GetILGenerator();
                var ilE = export.GetILGenerator();
                foreach(var field in typeof(Ndmarsh).GetFields())
                {
                    var aliases = field.GetCustomAttributes<AliasAttribute>();
                    foreach(var alias in aliases)
                    {
                        ilI.Emit(OpCodes.Ldarg_0);
                        ilI.Emit(OpCodes.Ldarg_1);
                        ilI.Emit(OpCodes.Ldfld, field);
                        ilI.Emit(OpCodes.Stfld, typeof(Self).GetField(alias.Name, Private));
                    }

                    ilE.Emit(OpCodes.Ldarg_1);
                    ilE.Emit(OpCodes.Ldarg_0);
                    ilE.Emit(OpCodes.Ldfld, typeof(Self).GetField(aliases.First().Name, Private));
                    ilE.Emit(OpCodes.Stfld, field);
                }
                ilI.Emit(OpCodes.Ret);
                ilE.Emit(OpCodes.Ret);

                _ImportParameters = import.CreateDelegate<ParameterIODelegate>();
                _ExportParameters = export.CreateDelegate<ParameterIODelegate>();
            }
        }

        private static T CreateMethod<T>(string methodName) where T : Delegate
        {
            return CreateMethod<T>(methodName, il =>
            {
                for (int i = 0; i < DelegateInfoCache<T>.ArgumentTypes.Length; i++)
                {
                    il.Ldarg(i);
                }
                il.Emit(OpCodes.Callvirt, typeof(Self).GetMethod(methodName, Private));
                il.Emit(OpCodes.Ret);
            });
        }

        private static T CreateMethod<T>(string methodName, Action<ILGenerator> generate) where T : Delegate
        {
            var method = new DynamicMethod(methodName, DelegateInfoCache<T>.ReturnType, DelegateInfoCache<T>.ArgumentTypes, typeof(Self), true);
            var il = method.GetILGenerator();
            generate(il);

            return method.CreateDelegate<T>();
        }

        private static (Action<Self, T> Setter, Func<Self, T> Getter) CreateProperty<T>(string fieldName)
        {
            var field = typeof(Self).GetField(fieldName, Private);
            return (CreateSetter<T>(field), CreateGetter<T>(field));
        }

        private static Action<Self, T> CreateSetter<T>(FieldInfo field)
        {
            var method = new DynamicMethod($"set{field.Name}", null, new[] { typeof(Self), typeof(T) }, typeof(Self), true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate<Action<Self, T>>();
        }

        private static Func<Self, T> CreateGetter<T>(FieldInfo field)
        {
            var method = new DynamicMethod($"get{field.Name}", typeof(T), new[] { typeof(Self) }, typeof(Self), true);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate<Func<Self, T>>();
        }

        public static Accessor ToAccessor(this Self @this) => new Accessor(@this);

        private static (Action<Self, bool> Setter, Func<Self, bool> Getter) _AsModularAvatarModule;
        private static (Action<Self, GameObject> Setter, Func<Self, GameObject> Getter) _Avatar;
        private static (Action<Self, GameObject> Setter, Func<Self, GameObject> Getter) _CopiedAvatar;
        private static (Action<Self, int> Setter, Func<Self, int> Getter) _Language;
        private static Action<Self> _OnGUI;
        private static Action<Self> _Localize;
        private static Func<Self, string> _SetupWithInitialize;

        private delegate void ParameterIODelegate(Self @this, Ndmarsh ndmarsh);
        private static ParameterIODelegate _ImportParameters;
        private static ParameterIODelegate _ExportParameters;

        public struct Accessor
        {
            private Self @this;
            public Accessor(Self @this) => this.@this = @this;

            public bool AsModularAvatarModule
            {
                get => _AsModularAvatarModule.Getter(@this);
                set => _AsModularAvatarModule.Setter(@this, value);
            }

            public GameObject Avatar
            {
                get => _Avatar.Getter(@this);
                set => _Avatar.Setter(@this, value);
            }

            public GameObject CopiedAvatar
            {
                get => _CopiedAvatar.Getter(@this);
                set => _CopiedAvatar.Setter(@this, value);
            }

            public int Language
            {
                get => _Language.Getter(@this);
                set => _Language.Setter(@this, value);
            }

            public void OnGUI() => _OnGUI(@this);
            public void Localize() => _Localize(@this);
            public string SetupWithInitialize() => _SetupWithInitialize(@this);
            public void ImportParameters(Ndmarsh source) => _ImportParameters(@this, source);
            public void ExportParameters(Ndmarsh destination) => _ExportParameters(@this, destination);
        }

        private static class DelegateInfoCache<T> where T : Delegate
        {
            public static readonly Type ReturnType;
            public static readonly Type[] ArgumentTypes;

            static DelegateInfoCache()
            {
                var invoke = typeof(T).GetMethod("Invoke");
                ReturnType = invoke.ReturnType;
                ArgumentTypes = invoke.GetParameters().Select(x => x.ParameterType).ToArray();
            }
        }
    }
}