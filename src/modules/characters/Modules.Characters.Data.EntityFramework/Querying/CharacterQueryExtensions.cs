using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.SavingThrows;
using Starlights.Modules.Characters.Domain.Skills;

namespace Starlights.Modules.Characters.Data.EntityFramework.Querying;

internal static class CharacterQueryExtensions
{
    /// <summary>
    /// Includes the full Character aggregate graph (components and derived component collections).
    /// </summary>
    public static IQueryable<Character> IncludeAggregateGraph(this IQueryable<Character> query)
    {
        return query
            .Include(c => c.Components)
            .IncludeComponent<ClassComponent>(x => x.Classes)
            .IncludeComponent<AbilitiesComponent>(x => x.AbilityScores)
            .IncludeComponent<SkillsComponent>(x => x.Skills)
            .IncludeComponent<SavingThrowsComponent>(x => x.SavingThrows);

    }

    private static IQueryable<Character> IncludeComponent<TComponent>(this IQueryable<Character> query, Expression<Func<TComponent, object>> navigation)
        where TComponent : CharacterComponentBase
    {
        var includable = query.Include(c => c.Components);
        var derivedNav = ComposeDerivedNavigation(navigation);
        return includable.ThenInclude(derivedNav);
    }

    private static Expression<Func<CharacterComponentBase, object>> ComposeDerivedNavigation<TComponent>(Expression<Func<TComponent, object>> navigation)
        where TComponent : CharacterComponentBase
    {
        var baseParam = Expression.Parameter(typeof(CharacterComponentBase), "cmp");
        var castParam = Expression.Convert(baseParam, typeof(TComponent));
        var body = new ReplaceParameterVisitor(navigation.Parameters[0], castParam).Visit(navigation.Body)!;

        // ensure the body is typed as object (box if needed)
        var objectBody = body.Type.IsValueType ? Expression.Convert(body, typeof(object)) : body;
        return Expression.Lambda<Func<CharacterComponentBase, object>>(objectBody, baseParam);
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam;
        private readonly Expression _newExpression;

        public ReplaceParameterVisitor(ParameterExpression oldParam, Expression newExpression)
        {
            _oldParam = oldParam;
            _newExpression = newExpression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _oldParam ? _newExpression : base.VisitParameter(node);
    }
}