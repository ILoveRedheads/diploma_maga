using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CSService.Blazor.Components;

public partial class TextForm : InputBase<string>
{
    [Parameter]
    public string Placeholder { get; set; } = "Начните вводить текст...";

    [Parameter]
    public string Label { get; set; }

    [Parameter]
    public Expression<Func<string>> ValidationExpression { get; set; } = null;

    [Parameter]
    public bool Disabled { get; set; }

    protected override bool TryParseValueFromString(
        string value,
        [MaybeNullWhen(false)] out string result,
        [NotNullWhen(false)] out string validationErrorMessage) {
        result = value ?? "";
        validationErrorMessage = null;
        return true;
    }

}
