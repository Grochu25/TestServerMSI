using System.Reflection;
using System.Reflection.Emit;
using TestServerMSI.Application.Services;

namespace TestServerMSI.Application.DLLs
{
    public class Encapsulator
    {
        public static object generateCapsule(Type interfaceType, Type injectedType, string methodToEncapsuleName, string? newTypeName = null)
        {
            if(newTypeName == null) newTypeName = injectedType.Name+"Capsule";
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder mb = assemblyBuilder.DefineDynamicModule(assemblyName.Name ?? "DynamicAssembly");

            TypeBuilder typeBuilder = mb.DefineType(newTypeName, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(interfaceType);


            MethodBuilder myMethodBuilder = typeBuilder.DefineMethod(methodToEncapsuleName,
                         MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { typeof(string) });

            ILGenerator myMethodIL = myMethodBuilder.GetILGenerator();


            ConstructorInfo stateWriterCtor = injectedType.GetConstructor(Type.EmptyTypes);
            myMethodIL.Emit(OpCodes.Newobj, stateWriterCtor);
            myMethodIL.Emit(OpCodes.Ldarg_1);
            MethodInfo injectedMethod = injectedType.GetMethod(methodToEncapsuleName);
            myMethodIL.Emit(OpCodes.Callvirt, injectedMethod);
            myMethodIL.Emit(OpCodes.Ret);

            MethodInfo interfaceMethod = interfaceType.GetMethod(methodToEncapsuleName);
            typeBuilder.DefineMethodOverride(myMethodBuilder, interfaceMethod);

            Type resultType = typeBuilder.CreateType();
            var resultObject = Activator.CreateInstance(resultType);

            return resultObject;
        }
    }
}