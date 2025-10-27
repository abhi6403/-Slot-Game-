using System;
using UnityEngine;

namespace SlotGame.Symbols
{
    [Serializable]
    public class SymbolData
    {
        public SymbolType type;
        public Sprite sprite;
        public string name;
        public int value;
    }
}