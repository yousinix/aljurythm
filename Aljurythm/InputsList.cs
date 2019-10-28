using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Aljurythm
{
    public class InputsList : List<string>
    {
        public void AddInput(Expression<Func<object>> expression)
        {
            var operand = (expression.Body as UnaryExpression)?.Operand;
            var func = expression.Compile();

            var inputName = (operand as MemberExpression)?.Member.Name;
            var inputValue = func();

            Add($"{inputName} = {inputValue}");
        }

        public override string ToString() => this.Aggregate("", (acc, s) => $"{acc}{s}\n");
    }
}
