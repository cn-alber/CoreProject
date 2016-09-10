// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace CoreWebApi.Middleware
{
    public class CusBaseCookieContext : BaseContext
    {
        public CusBaseCookieContext(
            HttpContext context,
            CusCookieAuthenticationOptions options)
            : base(context)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;
        }

        public CusCookieAuthenticationOptions Options { get; }
    }
}
