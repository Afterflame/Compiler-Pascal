using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Program;
using static Compiler.Parser;

namespace Compiler
{
    public class SymTable
    {
        public Dictionary<string, Symbol> data;
        public List<Symbol> ordered;

        public SymTable()
        {
            data = new Dictionary<string, Symbol>();
            ordered = new List<Symbol>();
        }
        public SymTable(Symbol symbol)
        {
            data = new Dictionary<string, Symbol>();
            ordered = new List<Symbol>();
            this.Add(symbol);
        }
        public int Count()
        {
            return data.Count();
        }
        public Symbol Get(string name)
        {
            if (!data.ContainsKey(name))
                throw new Exception(String.Format("Identifier not found: \"{0}\"", name));
            return data[name];
        }
        public Symbol GetVar(string name)
        {
            if (!data.ContainsKey(name))
                throw new Exception(String.Format("Identifier not found: \"{0}\"", name));
            return data[name].AsVar();
        }
        public Symbol GetAt(int id)
        {
            return ordered[id];
        }
        public void Add(Symbol value)
        {
            if (data.ContainsKey(value.name))
                throw new Exception(String.Format("Duplicate identifier: \"{0}\"", value.name));
            data[value.name] = value;
            ordered.Add(value);
        }
        public void AddRange(SymTable table)
        {
            foreach (Symbol sym in table.data.Values)
            {
                this.Add(sym);
            }
        }
        public bool Contains(string name)
        {
            return data.ContainsKey(name);
        }
    }
    public class SymTableStack
    {
        List<SymTable> data;
        public SymTable Last { get { return data.Last();  } }
        public SymTableStack()
        {
            data = new List<SymTable>();
        }
        public Symbol Get(string name)
        {
            for (var i = data.Count - 1; i >= 0; i--)
            {
                if (data[i].Contains(name)) return data[i].Get(name);
            }
            throw new Exception(String.Format("Identifier not found: \"{0}\"", name));
        }
        public Symbol Get(Symbol value)
        {
            for (var i = data.Count - 1; i >= 0; i--)
            {
                if (data[i].Contains(value.name)) return data[i].Get(value.name);
            }
            throw new Exception(String.Format("Identifier not found: \"{0}\"", value.name));
        }
        public void Add(Symbol value)
        {
            if (data.Last().Contains(value.name))
                throw new Exception(String.Format("Duplicate identifier: \"{0}\"", value.name));
            else data.Last().Add(value);
        }
        public void AddTable(SymTable table)
        {
            data.Add(table);
        }
        public void PopTable()
        {
            data.RemoveAt(data.Count - 1);
        }
    }
    public abstract class Symbol
    {
        public string name;
        public virtual string GetStrVal() { return name; }
        public virtual SymTable GetChildren(){ return new SymTable(); }
        public Symbol(string name)
        {
            this.name = name;
        }
        public virtual SymVar AsVar()
        {
            throw new Exception(String.Format("{0} is not a variable", name));
        }
        public virtual SymType AsType()
        {
            throw new Exception(String.Format("{0} is not a type", name));
        }
        public virtual SymArray AsArray()
        {
            throw new Exception(String.Format("{0} is not an array", name));
        }
        public virtual SymRecord AsRecord()
        {
            throw new Exception(String.Format("{0} is not a record", name));
        }
    }
    public class SymVar : Symbol
    {
        public SymType type;
        public override SymTable GetChildren() { return new SymTable(type); }

        public SymVar(string name, SymType type) : base(name)
        {
            this.type = type;
            this.type.name = this.type.name ?? "type";
        }
        public override SymVar AsVar()
        {
            return this;
        }
        public virtual bool IsConst()
        {
            return false;
        }
    }

    public class SymConst : SymVar 
    {
        public SymConst(string name, SymType type) : base(name, type) { }

        public override bool IsConst()
        {
            return true;
        }
    }
    public class SymParam : SymVar
    {
        public SymParam(string name, SymType type) : base(name, type) { }
    }
    public class SymParamRef : SymVar
    {
        public SymParamRef(string name, SymType type) : base(name, type) { }
    }
    public class SymType : Symbol
    {
        public override SymTable GetChildren() { return new SymTable(); }
        public SymType(string name) : base(name) { }

        public override SymType AsType()
        {
            return this;
        }
    }
    public class SymBool : SymType
    {
        public SymBool(string name) : base(name) { }
    }
    public class SymInteger : SymType
    {
        public SymInteger(string name) : base(name) { }
    }
    public class SymReal : SymType
    {
        public SymReal(string name) : base(name) { }
    }
    public class SymString : SymType
    {
        public SymString(string name) : base(name) { }
    }
    public class SymLabel : SymType
    {
        public SymLabel(string name) : base(name) { }
    }
    public class SymArray : SymType
    {
        public SymType type;
        public int l;
        public int r;
        public override string GetStrVal() { return String.Format("{0}[{1} , {2}]", name, l, r); }
        public override SymTable GetChildren()
        {
            return new SymTable(type); 
        }
        public SymArray(string name, SymType type, int l, int r) : base(name)
        {
            this.type = type;
            this.l = l;
            this.r = r;
            this.type.name = this.type.name ?? "type";
        }

        public override SymArray AsArray()
        {
            return this;
        }
    }
    public class SymRecord : SymType
    {
        public SymTable fields;
        public override SymTable GetChildren() { return fields; }
        public SymRecord(string name, SymTable fields) : base(name)
        {
            this.fields = fields;
        }
        public override SymRecord AsRecord()
        {
            return this;
        }
    }
    public class SymProc : Symbol
    {
        public SymTable params_;
        public SymTable locals_;
        public CompoundStatementNode body;

        public override SymTable GetChildren()
        {
            SymTable children = new SymTable();
            children.AddRange(params_);
            children.AddRange(locals_);
            return children;
        }

        public SymProc(string name, SymTable params_, SymTable locals_, CompoundStatementNode body) : base(name)
        {
            this.params_ = params_;
            this.locals_ = locals_;
            this.body = body;
        }
    }
    public class SymFunc : SymProc
    {
        public SymType type_;

        public override SymTable GetChildren() 
        {
            SymTable children = new SymTable();
            children.Add(type_);
            children.AddRange(params_);
            children.AddRange(locals_);
            return children;
        }
        public SymFunc(string name, SymTable params_, SymTable locals_, SymType type_, CompoundStatementNode body) : base(name, params_, locals_, body)
        {
            this.type_ = type_;
            this.type_.name = this.type_.name ?? "type";
        }
    }
}
