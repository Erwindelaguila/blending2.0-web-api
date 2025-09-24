using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class SysLog
{
    public Guid Id { get; set; }

    public string NameSpace { get; set; } = null!;

    public string? ClassName { get; set; }

    public string? MethodName { get; set; }

    public string? Username { get; set; }

    public Guid? UserId { get; set; }

    public DateTime DateTime { get; set; }

    public string? Message { get; set; }

    public string? StackTrace { get; set; }

    public string? ExtraInfo { get; set; }

    public Guid? RequestInvocationId { get; set; }

    public Guid? FunctionInvocationId { get; set; }

    public Guid? ExceptionGroupId { get; set; }

    public string Level { get; set; } = null!;
}
