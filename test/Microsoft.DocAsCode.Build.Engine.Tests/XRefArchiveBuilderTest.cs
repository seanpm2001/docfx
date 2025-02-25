﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace Microsoft.DocAsCode.Build.Engine.Tests;

[Trait("Related", "XRefAtchive")]
public class XRefArchiveBuilderTest
{
    [Fact]
    public async Task TestDownload()
    {
        const string ZipFile = "test.zip";
        var builder = new XRefArchiveBuilder();

        Assert.True(await builder.DownloadAsync(new Uri("http://dotnet.github.io/docfx/xrefmap.yml"), ZipFile));

        using (var xar = XRefArchive.Open(ZipFile, XRefArchiveMode.Read))
        {
            var map = xar.GetMajor();
            Assert.True(map.HrefUpdated);
            Assert.True(map.Sorted);
            Assert.NotNull(map.References);
            Assert.Null(map.Redirections);
        }
        File.Delete(ZipFile);
    }
}
