﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DocAsCode.DataContracts.Common;
using Microsoft.DocAsCode.YamlSerialization;

using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Microsoft.DocAsCode.Build.UniversalReference;

[Serializable]
public class ApiInheritanceTreeBuildOutput
{
    [YamlMember(Alias = Constants.PropertyName.Type)]
    [JsonProperty(Constants.PropertyName.Type)]
    public ApiNames Type { get; set; }

    [YamlMember(Alias = Constants.PropertyName.Inheritance)]
    [JsonProperty(Constants.PropertyName.Inheritance)]
    public List<ApiInheritanceTreeBuildOutput> Inheritance { get; set; }

    [YamlMember(Alias = "level")]
    [JsonProperty("level")]
    public int Level { get; set; }

    [ExtensibleMember]
    [JsonExtensionData]
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}
