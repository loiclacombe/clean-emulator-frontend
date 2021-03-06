/**
 * Autogenerated by Thrift Compiler (0.9.2)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Collections.Generic;
using System.Text;
using CleanEmulatorFrontend.Cache.Data;
using Thrift.Protocol;

namespace CleanEmulatorFrontend.Cache.Thrift.Data
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class EmulatedSystem : TBase
  {
    private string _ShortName;
    private List<Game> _Games;

    public string ShortName
    {
      get
      {
        return _ShortName;
      }
      set
      {
        __isset.ShortName = true;
        this._ShortName = value;
      }
    }

    public List<Game> Games
    {
      get
      {
        return _Games;
      }
      set
      {
        __isset.Games = true;
        this._Games = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool ShortName;
      public bool Games;
    }

    public EmulatedSystem() {
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.String) {
              ShortName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.List) {
              {
                Games = new List<Game>();
                TList _list4 = iprot.ReadListBegin();
                for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                {
                  Game _elem6;
                  _elem6 = new Game();
                  _elem6.Read(iprot);
                  Games.Add(_elem6);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("EmulatedSystem");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (ShortName != null && __isset.ShortName) {
        field.Name = "ShortName";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(ShortName);
        oprot.WriteFieldEnd();
      }
      if (Games != null && __isset.Games) {
        field.Name = "Games";
        field.Type = TType.List;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, Games.Count));
          foreach (Game _iter7 in Games)
          {
            _iter7.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("EmulatedSystem(");
      bool __first = true;
      if (ShortName != null && __isset.ShortName) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ShortName: ");
        __sb.Append(ShortName);
      }
      if (Games != null && __isset.Games) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Games: ");
        __sb.Append(Games);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
