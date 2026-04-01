using System;
using System.Collections.Generic;
using System.Linq;

public class CSCodeBlock : IDisposable
{
    private readonly CSCodeGenerator generator;

    public CSCodeBlock(CSCodeGenerator generator, string blockPrefix = "")
    {
        this.generator = generator;
        if (!string.IsNullOrEmpty(blockPrefix)) {
            this.generator.AddLine(blockPrefix);
        }
        this.generator.AddLine("{");
        this.generator.lineStartTabs++;
    }

    public void Dispose()
    {
        generator.lineStartTabs--;
        generator.AddLine("}");
    }
}

public class CSCodeGenerator
{
    protected List<string> codeLines = new List<string>();
    public int lineStartTabs = 0;

    public void AddLine(string line = "")
    {
        codeLines.Add(string.Concat(Enumerable.Repeat("\t", lineStartTabs)) + line);
    }

    public string OutputCode()
    {
        return string.Join("\n", codeLines);
    }

    public CSCodeBlock AddBlock(string blockPrefix = "")
    {
        return new CSCodeBlock(this, blockPrefix);
    }
}
