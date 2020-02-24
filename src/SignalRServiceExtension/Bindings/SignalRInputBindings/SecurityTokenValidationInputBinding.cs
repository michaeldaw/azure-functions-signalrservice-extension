﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.SignalRService
{
    internal class SecurityTokenValidationInputBinding : IBinding
    {
        private const string HttpRequestName = "$request";
        private readonly ISecurityTokenValidator securityTokenValidator;

        public bool FromAttribute { get; }

        public SecurityTokenValidationInputBinding(ISecurityTokenValidator securityTokenValidator)
        {
            this.securityTokenValidator = securityTokenValidator;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            var request = ((BindingContext)value).BindingData[HttpRequestName] as HttpRequest;

            if (request == null)
            {
                throw new NotSupportedException($"Argument {nameof(HttpRequest)} is null. {nameof(SecurityTokenValidationAttribute)} must work with HttpTrigger.");
            }

            if (securityTokenValidator == null)
            {
                return Task.FromResult((IValueProvider)new SignalRValueProvider(null));
            }

            return Task.FromResult((IValueProvider)new SignalRValueProvider(securityTokenValidator.ValidateToken(request)));
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            return BindAsync(context, null);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor();
        }
    }
}