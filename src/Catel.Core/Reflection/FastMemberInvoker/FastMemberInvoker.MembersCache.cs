namespace Catel.Reflection;

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Catel.Linq.Expressions;

partial class FastMemberInvoker<TEntity>
{
    private static class MembersCache<TValue>
    {
        private static readonly ConcurrentDictionary<string, Func<TEntity, TValue>?> GettersCache = new ConcurrentDictionary<string, Func<TEntity, TValue>?>();
        private static readonly ConcurrentDictionary<string, Action<TEntity, TValue>?> SettersCache = new ConcurrentDictionary<string, Action<TEntity, TValue>?>();

        private static readonly Func<string, (string, FastMemberInvoker<TEntity>), Action<TEntity, TValue>?> CreatePropertySetter = new Func<string, (string, FastMemberInvoker<TEntity>), Action<TEntity, TValue>?>((key, arg) =>
        {
            var (memberName, parent) = arg;
            var setterExpression = ExpressionBuilder.CreatePropertySetter<TEntity, TValue>(memberName);
            return setterExpression is not null ? parent.Compile(setterExpression) : null;
        });

        private static readonly Func<string, (string, FastMemberInvoker<TEntity>), Func<TEntity, TValue>?> CreatePropertyGetter = new Func<string, (string, FastMemberInvoker<TEntity>), Func<TEntity, TValue>?>((key, arg) =>
        {
            var (memberName, parent) = arg;
            var getterExpression = ExpressionBuilder.CreatePropertyGetter<TEntity, TValue>(memberName);
            return getterExpression is not null ? parent.Compile(getterExpression) : null;
        });

        private static readonly Func<string, (string, FastMemberInvoker<TEntity>), Action<TEntity, TValue>?> CreateFieldSetter = new Func<string, (string, FastMemberInvoker<TEntity>), Action<TEntity, TValue>?>((key, arg) =>
        {
            var (memberName, parent) = arg;
            var setterExpression = ExpressionBuilder.CreateFieldSetter<TEntity, TValue>(memberName);
            return setterExpression is not null ? parent.Compile(setterExpression) : null;
        });

        private static readonly Func<string, (string, FastMemberInvoker<TEntity>), Func<TEntity, TValue>?> CreateFieldGetter = new Func<string, (string, FastMemberInvoker<TEntity>), Func<TEntity, TValue>?>((key, arg) =>
        {
            var (memberName, parent) = arg;
            var getterExpression = ExpressionBuilder.CreateFieldGetter<TEntity, TValue>(memberName);
            return getterExpression is not null ? parent.Compile(getterExpression) : null;
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TEntity, TValue>? GetPropertyGetter(string memberName, FastMemberInvoker<TEntity> parent) =>
            GettersCache.GetOrAdd($"property_{memberName}", CreatePropertyGetter, (memberName, parent));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action<TEntity, TValue>? GetPropertySetter(string memberName, FastMemberInvoker<TEntity> parent) =>
            SettersCache.GetOrAdd($"property_{memberName}", CreatePropertySetter, (memberName, parent));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TEntity, TValue>? GetFieldGetter(string memberName, FastMemberInvoker<TEntity> parent) =>
            GettersCache.GetOrAdd($"field_{memberName}", CreateFieldGetter, (memberName, parent));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action<TEntity, TValue>? GetFieldSetter(string memberName, FastMemberInvoker<TEntity> parent) =>
            SettersCache.GetOrAdd($"field_{memberName}", CreateFieldSetter, (memberName, parent));
    }
}
