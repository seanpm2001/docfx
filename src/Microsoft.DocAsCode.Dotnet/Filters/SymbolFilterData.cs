﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Dotnet;

internal class SymbolFilterData
{
    public string Id { get; set; }

    public ExtendedSymbolKind? Kind { get; set;  }

    public IEnumerable<AttributeFilterData> Attributes { get; set; }
}
