// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using Microsoft.DocAsCode.Common;

namespace Microsoft.DocAsCode.Build.Engine;

public sealed class CompositeResourceReader : ResourceFileReader, IEnumerable<ResourceFileReader>
{
    private readonly ResourceFileReader[] _readers;

    public override string Name => "Composite";
    public override IEnumerable<string> Names { get; }
    public override bool IsEmpty { get; }

    public CompositeResourceReader(IEnumerable<ResourceFileReader> declaredReaders)
    {
        _readers = declaredReaders.ToArray();
        IsEmpty = _readers.Length == 0;
        Names = _readers.SelectMany(s => s.Names).Distinct();
    }

    public override Stream GetResourceStream(string name)
    {
        for (var i = _readers.Length - 1; i >= 0; i--)
        {
            if (_readers[i].GetResourceStream(name) is {} result)
                return result;
        }

        return null;
    }

    protected override void Dispose(bool disposing)
    {
        foreach (var reader in _readers)
            reader.Dispose();

        base.Dispose(disposing);
    }

    public IEnumerator<ResourceFileReader> GetEnumerator() => ((IEnumerable<ResourceFileReader>)_readers).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _readers.GetEnumerator();
}
