using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aljurythm
{
    public class InputsList : List<string>
    {

        public bool Multiline { get; set; } = true;

        public void AddInput(Expression<Func<object>> expression)
        {
            var operand = (expression.Body as UnaryExpression)?.Operand;
            var func = expression.Compile();

            var inputName = (operand as MemberExpression)?.Member.Name;
            var inputValue = func();

            Add($"{inputName} = {inputValue}");
        }

        public override string ToString() => string.Join(Multiline ? "\n" : ", ", this) + "\n";
    }
}
